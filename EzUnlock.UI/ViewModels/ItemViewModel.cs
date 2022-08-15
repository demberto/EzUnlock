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
        private string? location;

        public BitmapSource? IconSource { get; }
        public string? Name { get; }

        public ItemViewModel(string location)
        {
            if (!string.IsNullOrEmpty(location))
            {
                var fullpath = this.location = Path.GetFullPath(location);

                if (Directory.Exists(fullpath))
                {
                    Name = Path.GetDirectoryName(fullpath)!;
                }
                else if (File.Exists(fullpath))
                {
                    Name = Path.GetFileName(fullpath)!;
                }

                var icon = Icon.ExtractAssociatedIcon(fullpath);
                if (icon is not null)
                {
                    IconSource = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }
    }
}
