using System;
using ROBO_Mobile.Models;
using System.Net.Http;
using Xamarin.Forms;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace ROBO_Mobile.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string HTTP_URL = Application.Current.Properties["URL"] as string;
        private string CHNL_CD = Application.Current.Properties["CHNL_CD"] as string;

        private HttpResponseMessage response;
        private HttpClient client;

        string userName;
        string password;
        string verNm;

        public LoginViewModel()
        {
            //LoginCommand = new Command(OnLoginClicked, ValidateLogin);
            LoginCommand = new Command(OnLoginClicked);
            PropertyChanged +=
                (_, __) => LoginCommand.ChangeCanExecute();

            // 현재 버전 확인
            VerNm = "현재 버전 v" + VersionTracking.CurrentVersion;
        }


        public string UserName
        {
            get => this.userName;
            set => SetProperty(ref this.userName, value);
        }

        public string Password
        {
            get => this.password;
            set => SetProperty(ref this.password, value);
        }
        public string VerNm
        {
            get => this.verNm;
            set => SetProperty(ref this.verNm, value);
        }
        

        public Command LoginCommand { get; }


        private async void OnLoginClicked()
        {
            //await Navigation.NavigateToAsync<MainViewModel>();
            try
            {
                //공백 체크
                if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
                {
                    await App.Current.MainPage.DisplayAlert("로그인 실패", "아이디/패스워드를 입력하세요", "OK");
                    return;
                }

                using (client = new HttpClient())
                {
                    client.BaseAddress = new Uri(HTTP_URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    using (response = await client.PostAsync("mobile/login", new StringContent(JsonConvert.SerializeObject(new MobileVo() { CHNL_CD = this.CHNL_CD, USR_ID = UserName, DELT_FLG = "N" }), System.Text.Encoding.UTF8, "application/json")))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            MobileVo resultVo = JsonConvert.DeserializeObject<MobileVo>(await response.Content.ReadAsStringAsync());


                            if (resultVo == null)
                            {
                                await App.Current.MainPage.DisplayAlert("로그인 실패", "아이디가 존재하지 않습니다", "Ok");
                                return;
                            }

                            var sHA256Managed = new System.Security.Cryptography.SHA256Managed();
                            if (Convert.ToBase64String(sHA256Managed.ComputeHash(System.Text.Encoding.UTF8.GetBytes(this.Password))).Equals(resultVo.USR_PWD))
                            {
                                Application.Current.Properties["USR_NM"] = resultVo.USR_NM;
                                Application.Current.Properties["USR_ID"] = UserName;
                                await Navigation.NavigateToAsync<MainViewModel>();
                            }
                            else
                            {
                                await App.Current.MainPage.DisplayAlert("로그인 실패", "패스워드가 일치하지 않습니다", "Ok");
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception eLog)
            {
                await App.Current.MainPage.DisplayAlert("연결실패 - " + response.StatusCode, eLog.Message, "OK");
                return;
            }
        }

        //bool ValidateLogin()
        //{
        //    return !String.IsNullOrWhiteSpace(UserName)
        //        && !String.IsNullOrWhiteSpace(Password);
        //}
    }
}
