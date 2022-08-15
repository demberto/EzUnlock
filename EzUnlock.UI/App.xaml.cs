using System.Windows;
using EzUnlock.UI.Views;
using EzUnlock.UI.ViewModels;

namespace EzUnlock.UI
{
    public partial class App : Application
    {
        private void OnStart(object sender, StartupEventArgs e)
        {
            var window = new MainWindow();
            var viewModel = new MainWindowViewModel();
            window.DataContext = viewModel;

            window.Show();
        }
    }
}
