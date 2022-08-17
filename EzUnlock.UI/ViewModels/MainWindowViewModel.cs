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
        private readonly IFilePickerService _filePickerService = new FilePickerService();

        [ObservableProperty]
        private bool? isProcessing;

        private bool CanExecuteAction()
        {
            return Items.Count > 0 && !(IsProcessing ?? true);
        }

        [RelayCommand(CanExecute = nameof(CanExecuteAction))]
        private async Task UnlockItemsAsync()
        {
            isProcessing = true;
            var tmp = Items.ToList();
            foreach (ItemViewModel? item in tmp)
            {
                if (await Task.Run(() => Unlocker.Unlock(item.Location)))
                {
                    _ = Items.Remove(item);
                }
            }
            isProcessing = false;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteAction))]
        private async Task DeleteItemsAsync()
        {
            isProcessing = false;
            var tmp = Items.ToList();
            foreach (ItemViewModel? item in tmp)
            {
                if (await Task.Run(() => Unlocker.Delete(item.Location)))
                {
                    _ = Items.Remove(item);
                }
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
