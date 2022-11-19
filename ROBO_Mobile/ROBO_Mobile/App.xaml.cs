using ROBO_Mobile.Services;
using ROBO_Mobile.ViewModels;
using ROBO_Mobile.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xam.Plugin.AutoUpdate;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ROBO_Mobile
{
    public partial class App : Application
    {
        public App()
        {
            DevExpress.XamarinForms.Charts.Initializer.Init();
            DevExpress.XamarinForms.CollectionView.Initializer.Init();
            DevExpress.XamarinForms.Scheduler.Initializer.Init();
            DevExpress.XamarinForms.DataGrid.Initializer.Init();
            DevExpress.XamarinForms.Editors.Initializer.Init();
            DevExpress.XamarinForms.Navigation.Initializer.Init();
            DevExpress.XamarinForms.DataForm.Initializer.Init();
            DevExpress.XamarinForms.Popup.Initializer.Init();

            DependencyService.Register<MockDataStore>();
            DependencyService.Register<NavigationService>();

            InitializeComponent();

            var navigationService = DependencyService.Get<INavigationService>();
            // Use the NavigateToAsync<ViewModelName> method
            // to display the corresponding view.
            // Code lines below show how to navigate to a specific page.
            // Comment out all but one of these lines
            // to open the corresponding page.
            //navigationService.NavigateToAsync<LoginViewModel>();


            // 서버 접속
            Application.Current.Properties["URL"] = "http://210.217.42.139:8888/";
            Application.Current.Properties["CHNL_CD"] = "C210914-ROBO";

            // 로그인 시작화면
            navigationService.NavigateToAsync<MainViewModel>();
            //navigationService.NavigateToAsync<LoginViewModel>();
        }

        protected override void OnStart()
        {
            try
            {
                var current = Xamarin.Essentials.Connectivity.NetworkAccess;
                if (current == Xamarin.Essentials.NetworkAccess.Internet)
                {

                    #region 업데이트 실행 Ver2

                    string HTTP_URL = Application.Current.Properties["URL"] as string;

                    //downloadUrl = "http://192.168.5.99:8888/pda/co.kr.kcg.pdakcgapp_v1.0.apk";
                    string downloadUrl = string.Empty;
                    UpdateManagerParameters parameters = new UpdateManagerParameters
                    {
                        Title = "ROBO - 업데이트",
                        Message = "새 버전을 사용할 수 있습니다." + Environment.NewLine + "업데이트하십시오!"
                                                                  + Environment.NewLine + "(업데이트 후 사용을 권장 합니다)",
                        Confirm = "업데이트 완료 → 강제 종료",
                        Cancel = " ",
                        // choose how often to check when opening the app to avoid spamming the user every time
                        // RunEvery = TimeSpan.FromDays(1),
                        CheckForUpdatesFunction = async () =>
                        {
                            using (HttpClient _client = new HttpClient())
                            {
                                _client.BaseAddress = new Uri(HTTP_URL);
                                _client.DefaultRequestHeaders.Accept.Clear();
                                _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                                using (HttpResponseMessage _response = await _client.PostAsync("mobile/ver", new StringContent(JsonConvert.SerializeObject(new MobileVo()), System.Text.Encoding.UTF8, "application/json")))
                                {
                                    if (_response.IsSuccessStatusCode)
                                    {
                                        MobileVo _resultVo = JsonConvert.DeserializeObject<MobileVo>(await _response.Content.ReadAsStringAsync());
                                        if (_resultVo != null)
                                        {
                                            Version _curVer = new Version(VersionTracking.CurrentVersion);
                                            Version _updVer = new Version(_resultVo.VER_NM);

                                            if (_updVer.CompareTo(_curVer) > 0)
                                            {
                                                downloadUrl = _resultVo.ST_NM + _resultVo.FL_NM;

                                                //
                                                System.Diagnostics.Process.GetCurrentProcess().WaitForExit(1000 * 15);
                                                // check for updates from external url ...
                                                await Task.Delay(500);
                                                return new UpdatesCheckResponse(true, downloadUrl);
                                            }
                                            else
                                            {
                                                //업데이트 X
                                                return new UpdatesCheckResponse(false);
                                            }
                                        }
                                    }
                                }
                            }

                            return new UpdatesCheckResponse(false);
                        }
                    };

                    UpdateManager.Initialize(parameters, UpdateMode.AutoInstall);

                    #endregion
                }
            }
            catch (Exception eLog)
            {
                //await App.Current.MainPage.DisplayAlert("업데이트 - v" + VersionTracking.CurrentVersion, eLog.Message, "Ok");
                return;
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
