using ROBO_Mobile.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ROBO_Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupPage : ContentPage
    {
        public PopupPage()
        {
            InitializeComponent();
            BindingContext = new PopupViewModel();
        }

        void OnButtonClicked(object sender, EventArgs e)
        {
            Popup.IsOpen = true;
        }
    }
}