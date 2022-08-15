using System.Drawing;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EzUnlock.UI.ViewModels
{
    internal partial class ItemViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? location;

        public readonly Icon? Icon;
        public readonly string? Name;

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
                Icon = Icon.ExtractAssociatedIcon(fullpath);
            }
        }
    }
}
