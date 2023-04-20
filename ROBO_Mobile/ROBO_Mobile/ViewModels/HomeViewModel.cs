using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ROBO_Mobile.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public const string ViewName = "HomePage";

        // 로그인 일자, 사용자정보
        private string loginDt;
        private string userInfo;

        private string HTTP_URL = Application.Current.Properties["URL"] as string;
        private string CHNL_CD = Application.Current.Properties["CHNL_CD"] as string;
        private string USR_NM = Application.Current.Properties["USR_NM"] as string;

        public HomeViewModel()
        {
            Title = "Home";
            // OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://www.devexpress.com/xamarin/"));

            Page01Command = new Command(async () =>
            {
                (App.Current.MainPage.BindingContext as MainViewModel).SelectedMenuItem = (App.Current.MainPage.BindingContext as MainViewModel).MenuItems[1];
                await Navigation.NavigateToAsync<DataGridViewModel> ();
                //await (App.Current.MainPage.BindingContext as MainViewModel).Navigation.NavigateToAsync<ImportOrderMViewModel>();
            }, () => true);

            Page03Command = new Command(async () =>
            {
                (App.Current.MainPage.BindingContext as MainViewModel).SelectedMenuItem = (App.Current.MainPage.BindingContext as MainViewModel).MenuItems[2];
                await Navigation.NavigateToAsync<OtherInCaseViewModel>();
                //await (App.Current.MainPage.BindingContext as MainViewModel).Navigation.NavigateToAsync<ImportOrderMViewModel>(); 
            }, () => true);

            Page05Command = new Command(async () =>
            {
                (App.Current.MainPage.BindingContext as MainViewModel).SelectedMenuItem = (App.Current.MainPage.BindingContext as MainViewModel).MenuItems[3];
                await (App.Current.MainPage.BindingContext as MainViewModel).Navigation.NavigateToAsync<MtrlReturnInViewModel>();
            }, () => true);

            Page02Command = new Command(async () =>
            {
                (App.Current.MainPage.BindingContext as MainViewModel).SelectedMenuItem = (App.Current.MainPage.BindingContext as MainViewModel).MenuItems[4];
                await (App.Current.MainPage.BindingContext as MainViewModel).Navigation.NavigateToAsync<MtrlOutViewModel>();
            }, () => true);

            ////await Navigation.NavigateToAsync<DataGridViewModel>());
            //Page04Command = new Command(async () => {
            //    (App.Current.MainPage.BindingContext as MainViewModel).SelectedMenuItem = (App.Current.MainPage.BindingContext as MainViewModel).MenuItems[2];
            //    await (App.Current.MainPage.BindingContext as MainViewModel).Navigation.NavigateToAsync<DataGridViewModel>();
            //}, () => true);

            Refresh();
        }

        public void Refresh()
        {
            loginDt  = DateTime.Today.ToString("yyyy년  MM월 dd일");
            userInfo = "[" + USR_NM + "] 님이 \n" + "접속하셨습니다.";
        }

        public ICommand OpenWebCommand { get; }

        public ICommand Page01Command { get; }
        public ICommand Page02Command { get; }
        public ICommand Page03Command { get; }
        public ICommand Page04Command { get; }
        public ICommand Page05Command { get; }

        public string LoginDT
        {
            get => this.loginDt;
            set => SetProperty(ref this.loginDt, value);
        }

        public string LoginUserInfo
        {
            get => this.userInfo;
            set => SetProperty(ref this.userInfo, value);
        }
    }
}
