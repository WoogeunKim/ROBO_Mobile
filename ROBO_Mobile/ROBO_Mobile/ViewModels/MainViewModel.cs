using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ROBO_Mobile.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        CustomDrawerMenuItem selectedMenuItem;

        public MainViewModel()
        {
            IsBusy = true;
            MenuItems = new ObservableCollection<CustomDrawerMenuItem>();
            MenuItems.Add(new CustomDrawerMenuItem() { Name = "Home", ViewModelType = typeof(HomeViewModel), ImageName = "ic_info" });
            MenuItems.Add(new CustomDrawerMenuItem() { Name = "원자재 입고", ViewModelType = typeof(DataGridViewModel), ImageName = "ic_browse" });
            MenuItems.Add(new CustomDrawerMenuItem() { Name = "기타 입고", ViewModelType = typeof(OtherInCaseViewModel), ImageName = "ic_browse" });
            MenuItems.Add(new CustomDrawerMenuItem() { Name = "기존 바코드 입고", ViewModelType = typeof(MtrlReturnInViewModel), ImageName = "ic_browse" });
            MenuItems.Add(new CustomDrawerMenuItem() { Name = "원자재 출고", ViewModelType = typeof(MtrlOutViewModel), ImageName = "ic_scheduler" });
            //MenuItems.Add(new CustomDrawerMenuItem() { Name = "Charts", ViewModelType = typeof(ChartsViewModel), ImageName = "ic_charts" });
            //MenuItems.Add(new CustomDrawerMenuItem() { Name = "Popup", ViewModelType = typeof(PopupViewModel), ImageName = "ic_popup" });
            MenuItems.Add(new CustomDrawerMenuItem() { Name = "Logout", ViewModelType = typeof(LoginViewModel), ImageName = "ic_logout" });
            SelectedMenuItem = MenuItems[0];
            IsBusy = false;
        }

        public ObservableCollection<CustomDrawerMenuItem> MenuItems { get; }

        public CustomDrawerMenuItem SelectedMenuItem
        {
            get => this.selectedMenuItem;
            set
            {
                if (SelectedMenuItem == value || value == null)
                    return;
                SetProperty(ref this.selectedMenuItem, value);
                if (SelectedMenuItem != null && !IsBusy)
                    OnMenuItemSelected(SelectedMenuItem.ViewModelType);
            }
        }


        async void OnMenuItemSelected(Type pageType)
        {
            await Navigation.NavigateToAsync(SelectedMenuItem.ViewModelType);
        }
    }

    public class CustomDrawerMenuItem
    {
        public string Name { get; set; }
        public Type ViewModelType { get; set; }
        public string ImageName { get; set; }
        public bool NavigateToRoot { get; set; }
    }
}
