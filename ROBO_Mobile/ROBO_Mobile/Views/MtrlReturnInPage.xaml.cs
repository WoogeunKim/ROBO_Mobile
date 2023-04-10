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
    public partial class MtrlReturnInPage : ContentPage
    {
        public MtrlReturnInPage()
        {
            InitializeComponent();
            this.BindingContext = ViewModel = new MtrlReturnInViewModel();
        }

        public MtrlReturnInViewModel ViewModel;

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}