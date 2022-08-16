using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using EzUnlock.UI.Services;

namespace EzUnlock.UI.ViewModels
{
    internal partial class MainWindowViewModel : ObservableObject
    {
        public ObservableCollection<ItemViewModel> Items { get; } = new();

        private readonly IFilePickerService _filePickerService = new FilePickerService();

        [RelayCommand]
        private void OpenPicker()
        {
            var files = _filePickerService.PickFiles()
                                          .Select(x => (Short: x, Full: Path.GetFullPath(x)))
                                          .Where(x => Items.All(y => y.Location != x.Full));

            foreach (var file in files)
            {
                Items.Add(new ItemViewModel(file.Full));
            }
        }
    }
}
