using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
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
            foreach (string? file in _filePickerService.PickFiles())
            {
                Items.Add(new ItemViewModel(file));
            }
        }
    }
}
