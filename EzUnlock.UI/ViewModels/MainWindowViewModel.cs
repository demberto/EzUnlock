using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using EzUnlock.UI.Services;

namespace EzUnlock.UI.ViewModels
{
    internal partial class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            isProcessing = false;
        }

        public ObservableCollection<ItemViewModel> Items { get; } = new();
        // public IAsyncRelayCommand UnlockItemsAsyncCommand { get; }
        // public IAsyncRelayCommand DeleteItemsAsyncCommand { get; }
        private readonly IFilePickerService _filePickerService = new FilePickerService();

        [ObservableProperty]
        private bool isProcessing;

        [RelayCommand]
        private async Task UnlockItemsAsync()
        {
            isProcessing = true;
            foreach (ItemViewModel? item in Items)
            {
                item.IsProcessing = true;
            }

            var tmp = Items.ToList();
            foreach (ItemViewModel? item in tmp)
            {
                if (await Task.Run(() => Unlocker.Unlock(item.Location)))
                {
                    _ = Items.Remove(item);
                }
            }

            foreach (ItemViewModel? item in Items)
            {
                item.IsProcessing = false;
            }
            isProcessing = false;
        }

        [RelayCommand]
        private async Task DeleteItemsAsync()
        {
            isProcessing = false;
            foreach (ItemViewModel? item in Items)
            {
                item.IsProcessing = true;
            }

            var tmp = Items.ToList();
            foreach (ItemViewModel? item in tmp)
            {
                if (await Task.Run(() => Unlocker.Delete(item.Location)))
                {
                    _ = Items.Remove(item);
                }
            }

            foreach (ItemViewModel? item in Items)
            {
                item.IsProcessing = false;
            }
            isProcessing = true;
        }

        [RelayCommand]
        private void OpenFilePicker()
        {
            var files = _filePickerService.PickFiles()
                                          .Select(x => (Short: x, Full: Path.GetFullPath(x)))
                                          .Where(x => Items.All(y => y.Location != x.Full));

            foreach (var file in files)
            {
                Items.Add(new ItemViewModel(file.Full));
            }
        }

        [RelayCommand]
        private void OpenFolderPicker()
        {
            string? folder = _filePickerService.PickFolder();

            if (folder is not null)
            {
                Items.Add(new ItemViewModel(folder));
            }
        }
    }
}
