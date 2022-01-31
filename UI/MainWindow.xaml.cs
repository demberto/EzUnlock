/*
 * SPDX-FileCopyrightText: 2022 demberto <demberto@protonmail.com>
 *
 * SPDX-License-Identifier: MIT
 */

using Microsoft.Win32;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EzUnlock
{
    public partial class MainWindow : Window
    {
        private List<string> _files = new List<string>();
        private readonly NamedPipeServerStream _server;
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;

        private static readonly string unlockImgUri = "pack://application:,,,/EzUnlock;component/Resources/unlock.png";
        private static readonly string deleteImgUri = "pack://application:,,,/EzUnlock;component/Resources/delete.png";
        private static readonly string pauseImgUri = "pack://application:,,,/EzUnlock;component/Resources/pause.png";
        private static readonly string cancelImgUri = "pack://application:,,,/EzUnlock;component/Resources/cancel.png";

        private bool _cancelOperation = false;

        private enum Operation
        {
            Unlock,
            Delete
        }

        public MainWindow()
        {
            if (!File.Exists("Core.dll") || !File.Exists("Helper.exe"))
            {
                _ = MessageBox.Show("Missing dependency. EzUnlock cannot run. Reinstalling will help.", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            _server = new NamedPipeServerStream("EzUnlock", PipeDirection.InOut);
            _reader = new StreamReader(_server);
            _writer = new StreamWriter(_server);

            _ = Process.Start(new ProcessStartInfo("Helper.exe")
            {
                UseShellExecute = true,
                Verb = "runas"
            });

            _server.WaitForConnection();
            _writer.AutoFlush = true;

            InitializeComponent();
        }

        private void OpenInfoWindow(object sender, RoutedEventArgs e)
        {
            new Info().Show();
        }

        private void OnFilesSelected()
        {
            var numFiles = _files.Count;
            if (numFiles == 1)
                dropLbl.Content = "1 file selected";
            else
                dropLbl.Content = numFiles + " files selected";
            unlockPauseBtn.IsEnabled = true;
            deleteCancelBtn.IsEnabled = true;
        }

        private void FileDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                _files = ((string[])e.Data.GetData(DataFormats.FileDrop)).ToList();
                OnFilesSelected();
            }
        }

        private void OpenFilePicker(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Multiselect = true
            };
            if (fileDialog.ShowDialog() == true)
            {
                _files = fileDialog.FileNames.ToList();
                OnFilesSelected();
            }
        }

        private void SetState(bool enabled)
        {
            unlockPauseBtn.IsEnabled = enabled;
            deleteCancelBtn.IsEnabled = enabled;
        }

        private void Pause(object sender, RoutedEventArgs e)
        {
            SetState(false);
            statusBarProg.Foreground = new SolidColorBrush(Colors.Yellow);
            SetState(true);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            SetState(false);
            _cancelOperation = true;
            statusBarProg.Foreground = new SolidColorBrush(Colors.Red);
            SetState(true);
        }

        private void PreOperation()
        {
            statusBarLbl.Content = "";

            statusBarProg.Visibility = Visibility.Visible;
            statusBarProg.Maximum = _files.Count;
            statusBarProg.Foreground = new SolidColorBrush(Colors.Green);
            statusBarProg.Value = 0;

            unlockPauseBtnLbl.Content = "Pause";
            unlockPauseBtnImg.Source = new ImageSourceConverter().ConvertFromString(pauseImgUri) as ImageSource;
            unlockPauseBtn.Click -= Unlock;
            unlockPauseBtn.Click += Pause;

            deleteCancelBtnLbl.Content = "Cancel";
            deleteCancelBtnImg.Source = new ImageSourceConverter().ConvertFromString(cancelImgUri) as ImageSource;
            deleteCancelBtn.Click -= Delete;
            deleteCancelBtn.Click += Cancel;

            _cancelOperation = false;
        }

        private void PostOperation()
        {
            statusBarLbl.Content = "";

            unlockPauseBtnLbl.Content = "Unlock";
            unlockPauseBtnImg.Source = new ImageSourceConverter().ConvertFromString(unlockImgUri) as ImageSource;
            unlockPauseBtn.Click -= Pause;
            unlockPauseBtn.Click += Unlock;

            deleteCancelBtnLbl.Content = "Delete";
            deleteCancelBtnImg.Source = new ImageSourceConverter().ConvertFromString(deleteImgUri) as ImageSource;
            deleteCancelBtn.Click -= Cancel;
            deleteCancelBtn.Click += Delete;

            Title = "EzUnlock";
        }

        private async void DoOperation(Operation operation)
        {
            PreOperation();
            var resultsWnd = new Results();

            string operationStr;
            if (operation == Operation.Unlock)
            {
                operationStr = "Unlocking";
                Title = "EzUnlock (Unlocking)";
                resultsWnd.Title += " (Unlock)";
                _writer.WriteLine("UNLOCK");
            }
            else
            {
                operationStr = "Deleting";
                Title = "EzUnlock (Deleting)";
                resultsWnd.Title += " (Delete)";
                _writer.WriteLine("DELETE");
            }
            _server.WaitForPipeDrain();

            List<string> failed = new List<string>();
            List<string> successful = new List<string>();

            foreach (string file in _files)
            {
                if (_cancelOperation)
                    break;
                else
                {
                    statusBarLbl.Content = $"{operationStr} {file}";
                    _writer.WriteLine(file);
                    _server.WaitForPipeDrain();
                    string result = await _reader.ReadLineAsync();
                    if (result == "SUCCESS")
                        successful.Add(file);
                    else
                        failed.Add(file);
                    statusBarProg.Value++;
                }
            }

            if (successful.Count == 0)
            {
                resultsWnd.successExp.Visibility = Visibility.Hidden;
                Grid.SetRow(resultsWnd.failureExp, 0);
                Grid.SetRowSpan(resultsWnd.failureExp, 2);
            }
            else if (failed.Count == 0)
            {
                resultsWnd.failureExp.Visibility = Visibility.Hidden;
                Grid.SetRowSpan(resultsWnd.successExp, 2);
            }
            foreach (string fail in failed)
                _ = resultsWnd.failureBox.Items.Add(fail);
            foreach (string success in successful)
                _ = resultsWnd.successBox.Items.Add(success);
            resultsWnd.Show();

            PostOperation();
        }

        private void Unlock(object sender, RoutedEventArgs e)
        {
            DoOperation(Operation.Unlock);
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            DoOperation(Operation.Delete);
            _files.Clear();
            dropLbl.Content = "Drop or select files";
        }

        private void NotImplemented(object sender, RoutedEventArgs e)
        {
            _ = MessageBox.Show("That's under construction", "FYI", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
