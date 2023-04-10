using ROBO_Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace ROBO_Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OtherInCasePage : ContentPage
    {
        public OtherInCasePage()
        {
            InitializeComponent();
            this.BindingContext = ViewModel = new OtherInCaseViewModel();
        }

        public OtherInCaseViewModel ViewModel;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //ViewModel.OnAppearing();
        }
    }
}