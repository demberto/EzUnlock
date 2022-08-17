using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EzUnlock.UI.ViewModels
{
    internal partial class ItemViewModel : ObservableObject
    {
        [ObservableProperty]
        private string location;

        [ObservableProperty]
        public bool isProcessing;

        public BitmapSource? IconSource { get; }
        public string? Name { get; }

        public ItemViewModel(string location)
        {
            isProcessing = false;
            this.location = location;
            if (Directory.Exists(location))
            {
                Name = new DirectoryInfo(location).Name;
            }
            else if (File.Exists(location))
            {
                Name = Path.GetFileName(location)!;
                var icon = Icon.ExtractAssociatedIcon(location);
                if (icon is not null)
                {
                    IconSource = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }
    }
}
