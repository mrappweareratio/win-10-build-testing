using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace CR.Logic.ViewModels
{
    public class MainPageViewModel : ViewModel
    {
        public MainPageViewModel()
        {
            Title = "Hello World";
        }

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);
        }
        public string Title { get; set; }

    }
}
