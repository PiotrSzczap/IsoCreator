using System.Windows;

namespace IsoCreator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow _window;
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 0)
            {
                _window = new MainWindow(MainViewModel.CreateEmptyViewModel());
                _window.Show();
            }
            else
            {
                var viewModel = MainViewModel.CreateViewModel(e.Args[0]);
                _window = new MainWindow(viewModel);
                _window.Show();
                viewModel.RunExecute();
            }
            
        }
    }
}
