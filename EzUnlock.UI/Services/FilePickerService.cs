﻿using System.Windows.Forms;

namespace EzUnlock.UI.Services
{
    internal class FilePickerService : IFilePickerService
    {
        public string[] PickFiles()
        {
            OpenFileDialog ofd = new() { Multiselect = true };
            _ = ofd.ShowDialog();
            return ofd.FileNames;
        }

        public string PickFolder()
        {
            FolderBrowserDialog ofd = new() { ShowNewFolderButton = false };
            _ = ofd.ShowDialog();
            return ofd.SelectedPath;
        }
    }
}