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
    }
}
