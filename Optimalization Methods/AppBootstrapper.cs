using Caliburn.Micro;
using Optimalization_Methods.ViewModels;
using System.Windows;

namespace Optimalization_Methods
{

    public class AppBootstrapper : BootstrapperBase
    {
        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainWindowViewModel>();
        }
    }
}
