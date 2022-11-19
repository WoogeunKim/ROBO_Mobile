using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ROBO_Mobile.ViewModels;

namespace ROBO_Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            BindingContext = new HomeViewModel();
        }
    }
}