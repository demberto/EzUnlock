using EzUnlock.UI.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EzUnlock.UI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void DeleteItem(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            ItemViewModel? item = button?.DataContext! as ItemViewModel;
            MainWindowViewModel? vm = DataContext as MainWindowViewModel;
            _ = vm?.Items.Remove(item!);
        }

        private async void UnlockItemsAsync(object sender, RoutedEventArgs e)
        {
            addBtn.IsEnabled = deleteBtn.IsEnabled = unlockBtn.IsEnabled = false;
            var items = (DataContext as MainWindowViewModel)?.Items!;
            foreach (ItemViewModel? item in items)
            {
                item.IsProcessing = true;
            }

            var tmp = items.ToList();
            foreach (ItemViewModel? item in tmp)
            {
                if (await Task.Run(() => Unlocker.Unlock(item.Location)))
                {
                    _ = items.Remove(item);
                }
            }

            foreach (ItemViewModel? item in items)
            {
                item.IsProcessing = false;
            }
            addBtn.IsEnabled = deleteBtn.IsEnabled = unlockBtn.IsEnabled = true;
        }

        private async void DeleteItemsAsync(object sender, RoutedEventArgs e)
        {
            addBtn.IsEnabled = deleteBtn.IsEnabled = unlockBtn.IsEnabled = false;
            var items = (DataContext as MainWindowViewModel)?.Items!;
            foreach (ItemViewModel? item in items)
            {
                item.IsProcessing = true;
            }

            var tmp = items.ToList();
            foreach (ItemViewModel? item in tmp)
            {
                if (await Task.Run(() => Unlocker.Delete(item.Location)))
                {
                    _ = items.Remove(item);
                }
            }

            foreach (ItemViewModel? item in items)
            {
                item.IsProcessing = false;
            }
            addBtn.IsEnabled = deleteBtn.IsEnabled = unlockBtn.IsEnabled = true;
        }
    }
}
