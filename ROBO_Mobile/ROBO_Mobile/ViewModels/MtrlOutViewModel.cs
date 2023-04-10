using System;
using ROBO_Mobile.Models;
using System.Net.Http;
using Xamarin.Forms;
using Newtonsoft.Json;
using Xamarin.Essentials;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Internals;
using DevExpress.XamarinForms.DataGrid;
using System.Threading.Tasks;
using ROBO_Mobile.Views.Dialog;
using System.Threading;
using ROBO_Mobile.Services;

namespace ROBO_Mobile.ViewModels
{

    public class MtrlOutViewModel : BaseViewModel
    {
        private string _title = "원자재 출고";

        private string HTTP_URL = Application.Current.Properties["URL"] as string;
        private string CHNL_CD = Application.Current.Properties["CHNL_CD"] as string;
        private string USER_ID = Application.Current.Properties["USR_ID"] as string;

        private HttpClient client;


        public MtrlOutViewModel()
        {
            Title = "원자재 출고";


            BarCodeCommand = new Command(async () => await ExecuteBarCodeCommand());
            // 시작 동시에 바코드 호출
            //Task.Run(async () => await ExecuteBarCodeCommand());
        }

        /// <summary>
        /// 입력 동작을 할 때 해당 정보를 서버에 저장합니다.
        /// </summary>
        private Command okButtonCommand;
        public Command OKButtonCommand
        {
            get { return okButtonCommand ?? (this.okButtonCommand = new Command(this.InpOkCommand)); }
        }
        private async void InpOkCommand()
        {
            try
            {
                // 유효검사 (문제 없을 경우 true)
                if (await ValueCheckd() == true)
                {
                    var updateDao = GetDomain();

                    int _Num = 0;

                    // 발주 조회
                    using (client = httpClient())
                    {
                        using (HttpResponseMessage response = await client.PostAsync("mobile/out/mtrl"
                                                                                    , new StringContent(JsonConvert.SerializeObject(updateDao), System.Text.Encoding.UTF8, "application/json")))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                string resultMsg = await response.Content.ReadAsStringAsync();
                                if (int.TryParse(resultMsg, out _Num) == false)
                                {
                                    //실패
                                    await App.Current.MainPage.DisplayAlert(Title + " - 출고 탭 등록 오류", resultMsg, "OK");
                                    return;
                                }
                            }
                            // 성공
                            await App.Current.MainPage.DisplayAlert(Title + " - 출고 등록", "출고되었습니다", "OK");
                        }
                    }

                    // 메인창으로 이동합니다.
                    InpCancelCommand();
                }

            }
            catch (Exception eLog)
            {
                await App.Current.MainPage.DisplayAlert(Title + " - 등록 오류", eLog.Message, "OK");
                return;
            }
        }


        /// <summary>
        /// 입력시 유효검사를 진행합니다.
        /// </summary>
        /// <returns></returns>
        private async Task<Boolean> ValueCheckd()
        {
            var ret = default(Boolean);

            try
            {
                if (this.MstItmDao.LOT_NO == null)
                {
                    await App.Current.MainPage.DisplayAlert(Title + " - 유효검사", "등록된 바코드가 없습니다.", "OK");
                    ret = false;
                }
                else if (this.MstItmDao.ITM_QTY == null || Double.Parse(this.MstItmDao.ITM_QTY.ToString()) <= 0)
                {
                    await App.Current.MainPage.DisplayAlert(Title + " - 유효검사", "재고가 없습니다.", "OK");
                    ret = false;
                }
                else
                {
                    // 유효검사 문제 없을 경우 True
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                ret = false;
            }

            return ret;
        }


        private MobileVo GetDomain()
        {
            MobileVo ret = new MobileVo();

            try
            {
                // 품목코드, 원청바코드, 바코드, 창고코드, 출고수량
                ret.ITM_CD = this.MstItmDao.ITM_CD;
                ret.ORD_NO = this.MstItmDao.ORD_NO;
                ret.LOT_NO = this.MstItmDao.LOT_NO;
                ret.LOC_CD = this.MstItmDao.LOC_CD;
                ret.ITM_QTY = this.MstItmDao.ITM_QTY;

                // 수불유형 : 생산출고
                ret.INAUD_CD = "IGP";

                // 거래처 = 로보콘
                ret.CO_NO = "999";

                ret.CHNL_CD = this.CHNL_CD;
                ret.AREA_CD = "002";
                ret.CRE_USR_ID = USER_ID;
            }
            catch (Exception ex)
            {

            }

            return ret;
        }


        /// <summary>
        /// 입고 취소 할 때 해당 창을 닫습니다.
        /// </summary>
        private Command cancelButtonCommand;
        public Command CancelButtonCommand
        {
            get { return cancelButtonCommand ?? (this.cancelButtonCommand = new Command(this.InpCancelCommand)); }
        }
        private async void InpCancelCommand()
        {
            try
            {
                if (IsOpenPopup == true) IsOpenPopup = false;

                // 메인창으로 이동합니다.
                var navigationService = DependencyService.Get<INavigationService>();
                await navigationService.NavigateToAsync<MainViewModel>();
            }
            catch (Exception eLog)
            {
                await App.Current.MainPage.DisplayAlert(Title + " - 입고 뒤로 오류", eLog.Message, "OK");
                return;
            }
        }

        public Command BarCodeCommand { get; }
        //바코드 호출
        private async Task ExecuteBarCodeCommand()
        {
            // 바코드 스캔 시 입력창 잠시 닫음.
            if (IsOpenPopup == true) IsOpenPopup = false;

            ////바코드 저장
            //IsAnalyzing = false;
            //IsScanning = false;

            //Device.BeginInvokeOnMainThread(async () =>
            //{
            //    Barcode = Result.Text;
            //    Qty = "0";
            //});

            //IsAnalyzing = true;
            //IsScanning = true;



            //Device.BeginInvokeOnMainThread(async () =>
            //{
            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            FullScreenScanning scanning = new FullScreenScanning();
            scanning.Disappearing += async (sender2, e2) =>
            {
                if (string.IsNullOrEmpty(scanning.BARCODE))
                {
                    // 바코드 스캔 종료 후 
                    // 메인창으로 이동합니다.
                    InpCancelCommand();
                    return;
                }
                else
                {
                    // 바코드 스캔 종료 후 입력창 활성화.
                    MstItmDao = await PostMtrlLotData<MobileVo>("mobile/itm/qty", new MobileVo() { CHNL_CD = this.CHNL_CD, LOT_NO = scanning.BARCODE }); // scanning.BARCODE 

                    //해당 바코드가 존재하지 않을 경우 메인화면으로 돌아갑니다.
                    if (MstItmDao == null)
                    {
                        await App.Current.MainPage.DisplayAlert(Title + " - 유효검사", "등록된 바코드가 없습니다.", "OK");

                        // 메인창으로 이동합니다.
                        InpCancelCommand();
                    }
                    if (MstItmDao.LOT_NO == null)
                    {
                        await App.Current.MainPage.DisplayAlert(Title + " - 유효검사", "등록된 바코드가 없습니다.", "OK");

                        // 메인창으로 이동합니다.
                        InpCancelCommand();
                    }
                    else
                    {
                        // 바코드 스캔 종료 후 입력창 활성화.
                        if (IsOpenPopup == false) IsOpenPopup = true;
                    }

                    // 바코드 스캔 종료 후 입력창 활성화.
                    //Task.Run(async () => MstItmDao = await PostMtrlLotData<MobileVo>("itm/qty", new MobileVo() { CHNL_CD = this.CHNL_CD, LOT_NO = scanning.BARCODE }));

                    // 해당 바코드가 존재하지 않을 경우 메인화면으로 돌아갑니다.
                    //if (MstItmDao == null)
                    //{
                    //    Task.Run(async () => await WarnMessageBox("등록된 바코드가 없습니다."));
                    //    //Task.Run(() =>
                    //    //{
                    //    //    App.Current.MainPage.DisplayAlert(Title + " - 유효검사", "등록된 바코드가 없습니다.", "OK");
                    //    //    return Task.FromResult(0);
                    //    //});

                    //    // 메인창으로 이동합니다.
                    //    InpCancelCommand();
                    //}
                    //else
                    //{
                    //    // 바코드 스캔 종료 후 입력창 활성화.
                    //    if (IsOpenPopup == false) IsOpenPopup = true;
                    //}


                    //Func<Task> isItmData = async () =>
                    //{
                    //    if (await WarnMessageBox(scanning.BARCODE) == true)
                    //    {
                    //        // 성공 
                    //        // 바코드 스캔 종료 후 입력창 활성화.
                    //        if (IsOpenPopup == false) IsOpenPopup = true;
                    //    }
                    //    else
                    //    {
                    //        // 실패
                    //        // 메인창으로 이동합니다.
                    //        InpCancelCommand();
                    //    }
                    //};
                    //
                }
            };
            await App.Current.MainPage.Navigation.PushModalAsync(scanning);
            await Task.Run(() => waitHandle.WaitOne());

        }

        //private async Task<bool> WarnMessageBox(string barcode)
        //{
        //    var ret = default(bool);

        //    try
        //    {
        //        MstItmDao = await PostMtrlLotData<MobileVo>("itm/qty", new MobileVo() { CHNL_CD = this.CHNL_CD, LOT_NO = barcode });

        //        if (MstItmDao == null)
        //        {
        //            await App.Current.MainPage.DisplayAlert(Title + " - 유효검사", "등록된 바코드가 없습니다.", "OK");

        //        }
        //        else
        //        {
        //            // 성공
        //            ret = true;
        //        }
        //    }
        //    catch (Exception eLog)
        //    {
        //        await App.Current.MainPage.DisplayAlert(Title + " - 입고 뒤로 오류", eLog.Message, "OK");
        //    }

        //    return ret;
        //}


        /// <summary>
        /// JSON 통해 서버로부터 정보를 가져옵니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Path"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<T> PostMtrlLotData<T>(string Path, object obj)
        {
            var ret = default(T);

            try
            {
                using (client = httpClient())
                {
                    using (HttpResponseMessage response = await client.PostAsync(Path
                                                                                , new StringContent(JsonConvert.SerializeObject(obj), System.Text.Encoding.UTF8, "application/json")))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            // 성공
                            ret = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                        }
                    }
                }
            }
            catch (Exception eLog)
            {
                await App.Current.MainPage.DisplayAlert(Title + " - 바코드 정보 조회 오류", eLog.Message, "OK");
            }

            return ret;
        }



        /// <summary>
        /// JSON 초기화를 진행합니다.
        /// </summary>
        /// <returns></returns>
        public HttpClient httpClient()
        {
            var ret = new HttpClient();

            try
            {
                ret.BaseAddress = new Uri(HTTP_URL);
                ret.DefaultRequestHeaders.Accept.Clear();
                ret.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }
            catch (Exception ex)
            {

            }

            return ret;
        }



        /// <summary>
        /// MVVM 방식으로 View 필요한 데이터를 바인딩합니다.
        /// </summary>
        #region MVVM 방식으로 View 필요한 데이터를 바인딩합니다.
        private MobileVo _mstItem;
        public MobileVo MstItmDao
        {
            get => this._mstItem;
            set => SetProperty(ref this._mstItem, value);
        }

        // 입고 탭 여부
        bool isOpenPopup = false;
        public bool IsOpenPopup
        {
            get => this.isOpenPopup;
            set => SetProperty(ref this.isOpenPopup, value);
        }
        #endregion
    }
}