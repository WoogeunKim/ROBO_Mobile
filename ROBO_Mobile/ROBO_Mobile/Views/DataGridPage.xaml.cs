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
    public partial class DataGridPage : ContentPage
    {
        public DataGridPage()
        {
            InitializeComponent();
            this.BindingContext = ViewModel = new DataGridViewModel();
        }

        public DataGridViewModel ViewModel;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //ViewModel.OnAppearing();
        }
    }
}