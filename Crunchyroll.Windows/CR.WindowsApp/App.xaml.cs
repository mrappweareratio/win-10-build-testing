using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Microsoft.ApplicationInsights;
using Prism.Mvvm;
using Prism.Windows.Store.Unity;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=402347&clcid=0x409

namespace CR.WindowsApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : UnityMvvmAppBase
    {
        /// <summary>
        /// Allows tracking page views, exceptions and other telemetry through the Microsoft Application Insights service.
        /// </summary>
        public static TelemetryClient TelemetryClient;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            TelemetryClient = new TelemetryClient();
      
            this.InitializeComponent();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(type =>
            {
                var name = type.Name;
                var viewModelName = name.EndsWith("View") ? name.Substring(0, name.LastIndexOf("View", StringComparison.OrdinalIgnoreCase)) : name;
                var viewModelTypeName = string.Format("CR.Logic.ViewModels.{0}ViewModel, CR.Logic", viewModelName);
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
