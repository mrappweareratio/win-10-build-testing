using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Prism.Mvvm;
using Prism.Windows.Store.Unity;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=402347&clcid=0x409

namespace CR.WinApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : UnityMvvmAppBase
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {                  
            this.InitializeComponent();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(type =>
            {
                var name = type.Name;
                var viewModelName = name.EndsWith("View") ? name.Substring(0, name.LastIndexOf("View", StringComparison.OrdinalIgnoreCase)) : name;
                var viewModelTypeName = $"CR.ViewModels.{viewModelName}ViewModel, CR.Core";
                var viewModelType = Type.GetType(viewModelTypeName);
               return viewModelType;
            });
        }

        protected override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            await base.OnInitializeAsync(args);  
        }

        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            NavigationService.Navigate("Main", null);
            return Task.FromResult<object>(null);
        }
    }
}
