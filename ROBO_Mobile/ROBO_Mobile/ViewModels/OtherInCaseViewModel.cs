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

    public class OtherInCaseViewModel : BaseViewModel
    {
        private string _title = "원자재 기타입고";

        private string HTTP_URL = Application.Current.Properties["URL"] as string;
        private string CHNL_CD = Application.Current.Properties["CHNL_CD"] as string;
        private string USER_ID = Application.Current.Properties["USR_ID"] as string;

        private HttpResponseMessage response;
        private HttpClient client; 



        public OtherInCaseViewModel()
        {
            Title = "원자재 기타입고";

            SYSTEM_CODE_VO();

            //바코드 호출
            //BarCodeCommand = new Command(async () => await ExecuteBarCodeCommand());
        }

        public async void SYSTEM_CODE_VO()
        {
            try
            {
                // 품목구분 조회
                using (client = httpClient())
                {
                    using (response = await client.PostAsync("mobile/itm/grp", new StringContent(JsonConvert.SerializeObject(new MobileVo() { ITM_GRP_CLSS_CD = "M", CHNL_CD = this.CHNL_CD, DELT_FLG = "N" }), System.Text.Encoding.UTF8, "application/json")))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            // 품목구분 한번만 조회
                            this.ItmGrpList = JsonConvert.DeserializeObject<IEnumerable<MobileVo>>(await response.Content.ReadAsStringAsync()).Cast<MobileVo>().ToList();

                            if (ItmGrpList.Count <= 0)
                            {
                                await App.Current.MainPage.DisplayAlert("품목구분 실패", "[품목구분] 정보가 존재하지 않습니다", "Ok");
                                return;
                            }

                            // 강종
                            ComboN1stGrpList = ItmGrpList.Where(x=>x.PRNT_ITM_GRP_CD.Equals("T")).ToList<MobileVo>();
                            if (ComboN1stGrpList.Count <= 0)
                            {
                                await App.Current.MainPage.DisplayAlert("강종 실패", "[강종] 정보가 존재하지 않습니다", "Ok");
                                return;
                            }
                        }
                    }
                }

                // 수불유형
                using (client = httpClient())
                {
                    using (response = await client.PostAsync("mobile/inp/sys", new StringContent(JsonConvert.SerializeObject(new MobileVo() { CLSS_TP_CD = "L-007", CLSS_CD ="R", CHNL_CD = this.CHNL_CD, DELT_FLG = "N" }), System.Text.Encoding.UTF8, "application/json")))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var INAUDList = new List<MobileVo>();
                            INAUDList = JsonConvert.DeserializeObject<IEnumerable<MobileVo>>(await response.Content.ReadAsStringAsync()).Cast<MobileVo>().ToList();

                            // 구매입고 삭제
                            this.ComboInaudList = INAUDList.Where(x => x.CLSS_CD != "RGU").ToList<MobileVo>();

                            if (ComboInaudList.Count <= 0)
                            {
                                await App.Current.MainPage.DisplayAlert("수불유형 실패", "[수불유형] 정보가 존재하지 않습니다", "Ok");
                                return;
                            }
                        }
                    }
                }

                // 창고
                using (client = httpClient())
                {
                    using (response = await client.PostAsync("mobile/inp/sys", new StringContent(JsonConvert.SerializeObject(new MobileVo() { CLSS_TP_CD = "P-008", CHNL_CD = this.CHNL_CD, DELT_FLG = "N" }), System.Text.Encoding.UTF8, "application/json")))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            this.ComboLocList = JsonConvert.DeserializeObject<IEnumerable<MobileVo>>(await response.Content.ReadAsStringAsync()).Cast<MobileVo>().ToList();

                            if (ComboLocList.Count <= 0)
                            {
                                await App.Current.MainPage.DisplayAlert("창고 실패", "[창고] 정보가 존재하지 않습니다", "Ok");
                                return;
                            }

                            this.LocItem = this.ComboLocList.Where(x => x.CLSS_CD.Equals("100")).FirstOrDefault<MobileVo>();
                        }
                    }
                }

                // (DateEdit) 입고일자 = 현재
                this.InpDt = DateTime.Now.Date;
                // (NumericEdit) 입고가닥수량
                this.InpQty = 0;
                // (NumericEdit) 길이
                this.ItmLen = 0;
            }
            catch (Exception eLog)
            {
                await App.Current.MainPage.DisplayAlert(Title + " - 기타입고 조회 오류", eLog.Message, "OK");
                return;
            }
        }

        private async void N2ndChangeFunc()
        {
            try
            {
                if (N1stGrpItem == null) return;

                // 규격 
                ComboN2ndGrpList = this.ItmGrpList.Where(x => x.PRNT_ITM_GRP_CD.Equals(N1stGrpItem.ITM_GRP_CD)).ToList<MobileVo>();

            }
            catch (Exception eLog)
            {
                await App.Current.MainPage.DisplayAlert(Title + " - 규격 조회 오류", eLog.Message, "OK");
                return;
            }
        }

        private async void ItmLenChangeFunc()
        {
            try
            {
                if (N2ndGrpItem == null) return;

                //// 길이조회
                //using (client = httpClient())
                //{
                //    using (response = await client.PostAsync("mobile/itm/len"
                //                                            , new StringContent(JsonConvert.SerializeObject(new MobileVo() { N1ST_ITM_GRP_CD = this.N1stGrpItem.ITM_GRP_CD, N2ND_ITM_GRP_CD = this.N2ndGrpItem.ITM_GRP_CD, CHNL_CD = this.CHNL_CD, DELT_FLG = "N" }), System.Text.Encoding.UTF8, "application/json")))
                //    {
                //        if (response.IsSuccessStatusCode)
                //        {
                //            this.ComboLenList = JsonConvert.DeserializeObject<IEnumerable<MobileVo>>(await response.Content.ReadAsStringAsync()).Cast<MobileVo>().ToList();
                //        }
                //    }
                //}

            }
            catch (Exception eLog)
            {
                await App.Current.MainPage.DisplayAlert(Title + " - 품목(길이) 조회 오류", eLog.Message, "OK");
                return;
            }
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
                if (this.IsM_INSERT)
                {
                    // 유효검사 (문제 없을 경우 true)
                    if (await ValueCheckd() == true)
                    {
                        var updateDao = GetDomain();

                        int _Num = 0;

                        // 발주 조회
                        using (client = httpClient())
                        {
                            using (HttpResponseMessage response = await client.PostAsync("mobile/inp/m/i", new StringContent(JsonConvert.SerializeObject(updateDao), System.Text.Encoding.UTF8, "application/json")))
                            {
                                if (response.IsSuccessStatusCode)
                                {
                                    string resultMsg = await response.Content.ReadAsStringAsync();
                                    if (int.TryParse(resultMsg, out _Num) == false)
                                    {
                                        //실패
                                        await App.Current.MainPage.DisplayAlert(Title + " - 입고 탭 등록 오류", resultMsg, "OK");
                                        return;
                                    }
                                }
                                // 성공
                                await App.Current.MainPage.DisplayAlert(Title + " - 입고 등록", "입고되었습니다", "OK");
                            }
                        }

                        // 초기화 
                        // (NumericEdit.HasError) 오류 유효검사
                        this.ActualHasError = false;
                        // 규격
                        this.N2ndGrpItem = null;
                        // 길이
                        this.ItmLen = 0;
                        // 품목
                        //this.LenList = new List<MobileVo>();           
                        this.LenItem = null;
                        // (DateEdit) 입고일자 = 현재
                        this.InpDt = DateTime.Now.Date;
                        // 가닥수량
                        this.InpQty = 0;


                    }
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
        private async Task<bool> ValueCheckd()
        {
            var ret = default(bool);

            try
            {
                int _Num = 0;

                if(this.N1stGrpItem == null)
                {
                    await App.Current.MainPage.DisplayAlert(Title + " - 유효검사", "강종을 선택하세요.", "OK");
                    ret = false;
                }
                else if(this.N2ndGrpItem == null)
                {
                    await App.Current.MainPage.DisplayAlert(Title + " - 유효검사", "규격을 선택하세요.", "OK");
                    ret = false;
                }
                else if (this.LenItem == null)
                {
                    await App.Current.MainPage.DisplayAlert(Title + " - 유효검사", "길이를 선택하세요.", "OK");
                    ret = false;
                }
                else if (this.InaudItem == null)
                {
                    await App.Current.MainPage.DisplayAlert(Title + " - 유효검사", "수불유형을 선택하세요.", "OK");
                    ret = false;
                }
                else if (this.LocItem == null)
                {
                    await App.Current.MainPage.DisplayAlert(Title + " - 유효검사", "창고를 선택하세요.", "OK");
                    ret = false;
                }
                else if (this.InpQty == null)
                {
                    await App.Current.MainPage.DisplayAlert(Title + " - 유효검사", "가닥수량을 입력하세요.", "OK");
                    ret = false;
                }
                else if (int.TryParse(this.InpQty.ToString(), out _Num) == false)
                {
                    await App.Current.MainPage.DisplayAlert(Title + " - 유효검사", "가닥수량을 정수형태로 입력하세요.", "OK");
                    ret = false;
                }
                else if (int.Parse(this.InpQty.ToString()) <= 0)
                {
                    await App.Current.MainPage.DisplayAlert(Title + " - 유효검사", "가닥수량을 다시 입력하세요.", "OK");
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
                // 품목코드
                ret.ITM_CD = this.LenItem.ITM_CD;

                // 수불유형
                ret.INAUD_CD = this.InaudItem.CLSS_CD;

                // 창고
                ret.LOC_CD = this.LocItem.CLSS_CD;

                // 입고일자
                ret.INAUD_DT = this.InpDt.ToString("yyyy-MM-dd");

                // 입고수량
                ret.ITM_QTY = this.InpQty;
                ret.ITM_TMP_QTY = this.InpQty;

                // 거래처 = 로보콘
                ret.CO_NO = "999";

                ret.CHNL_CD = this.CHNL_CD;
                ret.AREA_CD = "002";
                ret.CRE_USR_ID = USER_ID;
            }
            catch(Exception ex)
            {

            }

            return ret;
        }

        /// <summary>
        /// 입고 취소 할 때 해당 창을 닫습니다.
        /// </summary>
        private Command refrushCommand;
        public Command RefrushCommand
        {
            get { return refrushCommand ?? (this.refrushCommand = new Command(this.ItmSelectCommand)); }
        }
        private async void ItmSelectCommand()
        {
            try
            {
                // 유효검사
                if (this.N1stGrpItem == null) return;
                if (this.N1stGrpItem.ITM_GRP_CD == null) return;
                if (this.N2ndGrpItem == null) return;
                if (this.N2ndGrpItem.ITM_GRP_CD == null) return;
                if (this.ItmLen <= 0) return;

                var obj = new MobileVo()
                {
                    CHNL_CD = this.CHNL_CD,
                    N1ST_ITM_GRP_CD = this.N1stGrpItem.ITM_GRP_CD,
                    N2ND_ITM_GRP_CD = this.N2ndGrpItem.ITM_GRP_CD,
                    DELT_FLG = "N",
                    ITM_GRP_CLSS_CD = "M",
                    USR_ID = USER_ID,
                    ITM_LEN = this.ItmLen
                };


                // 조회
                MobileVo itmObj = await PostJsonItm<MobileVo>("mobile/itm/len", obj);

                if (itmObj == null)
                {
                    // 품목등록 질문
                    if(await App.Current.MainPage.DisplayAlert(Title + "- 신규 품목 등록", "길이 : " + this.ItmLen.ToString() + " 등록하겠습니까?", "YES", "NO") == false)
                    {
                        // 취소
                        return;
                    }

                    // 등록
                    using (client = httpClient())
                    {
                        using (HttpResponseMessage response = await client.PostAsync("mobile/itm/i", new StringContent(JsonConvert.SerializeObject(obj), System.Text.Encoding.UTF8, "application/json")))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                int _Num = 0;
                                string resultMsg = await response.Content.ReadAsStringAsync();
                                if (int.TryParse(resultMsg, out _Num) == false)
                                {
                                    //실패
                                    await App.Current.MainPage.DisplayAlert(Title + " - 품목 등록 오류", resultMsg, "OK");
                                    return;
                                }
                            }
                            // 성공
                            await App.Current.MainPage.DisplayAlert(Title + " - 품목 등록", "등록 되었습니다", "OK");

                            // 다시 조회
                            itmObj = await PostJsonItm<MobileVo>("mobile/itm/len", obj);
                        }
                    }
                }

                LenItem = itmObj;
            }
            catch (Exception eLog)
            {
                await App.Current.MainPage.DisplayAlert(Title + " - 품목 조회 오류", eLog.Message, "OK");
                return;
            }
        }

        private async Task<T> PostJsonItm<T>(string Path, object obj)
        {
            var ret = default(T);

            try
            {
                using (client = httpClient())
                {
                    using (response = await client.PostAsync(Path
                                                           , new StringContent(JsonConvert.SerializeObject(obj), System.Text.Encoding.UTF8, "application/json")))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var retList = JsonConvert.DeserializeObject<IEnumerable<T>>(await response.Content.ReadAsStringAsync()).Cast<T>().ToList();

                            if (retList.Count >= 1)
                            {
                                ret = retList[0];
                            }
                        }
                    }
                }

            }
            catch (Exception eLog)
            {
                await App.Current.MainPage.DisplayAlert(Title + " - 품목 조회 오류", eLog.Message, "OK");
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

        /// <summary>
        /// 길이 값을 유효검사 합니다.
        /// </summary>
        public async void ItmLenChanged()
        {
            try
            {
                int _Num = 0;

                // 입력한 수량이 정수가 아닐 경우
                if (int.TryParse(ItmLen.ToString(), out _Num) == false)
                {
                    this.LenActualErrorText = "길이 값을 정수형태로 입력하세요";
                    this.LenActualHasError = true;
                }
                else if (int.Parse(ItmLen.ToString()) <= 0)
                {
                    this.LenActualErrorText = "길이 값을 다시 입력하세요";
                    this.LenActualHasError = true;
                }
                else
                {
                    this.LenActualHasError = false;
                }
            }
            catch (Exception eLog)
            {
                await App.Current.MainPage.DisplayAlert(Title + " - 길이 입력 오류", eLog.Message, "OK");
                return;
            }
        }


        /// <summary>
        /// 입고수량 유효검사 합니다.
        /// </summary>
        public async void InpQtyChanged()
        {
            try
            {
                int _Num = 0;

                // 입력한 수량이 정수가 아닐 경우
                if (int.TryParse(InpQty.ToString(), out _Num) == false)
                {
                    this.ActualErrorText = "가닥수량을 정수형태로 입력하세요";
                    this.ActualHasError = true;
                    // 입고확인 버튼 비활성화
                    this.IsM_INSERT = false;
                }
                else if (int.Parse(InpQty.ToString()) <= 0)
                {
                    this.ActualErrorText = "가닥수량을 다시 입력하세요";
                    this.ActualHasError = true;
                    // 입고확인 버튼 비활성화
                    this.IsM_INSERT = false;
                }
                else
                {
                    this.ActualHasError = false;
                    // 입고확인 버튼 활성화
                    this.IsM_INSERT = true;
                }
            }
            catch (Exception eLog)
            {
                await App.Current.MainPage.DisplayAlert(Title + " - 입고 탭 등록 오류", eLog.Message, "OK");
                return;
            }
        }

        public Command BarCodeCommand { get; }

        ////바코드 호출
        //private async Task ExecuteBarCodeCommand()
        //{
        //    // 바코드 스캔 시 입력창 잠시 닫음.
        //    if (IsOpenPopup == true) IsOpenPopup = false;

        //    ////바코드 저장
        //    //IsAnalyzing = false;
        //    //IsScanning = false;

        //    //Device.BeginInvokeOnMainThread(async () =>
        //    //{
        //    //    Barcode = Result.Text;
        //    //    Qty = "0";
        //    //});

        //    //IsAnalyzing = true;
        //    //IsScanning = true;



        //    //Device.BeginInvokeOnMainThread(async () =>
        //    //{
        //    var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        //    FullScreenScanning scanning = new FullScreenScanning();
        //    scanning.Disappearing += (sender2, e2) =>
        //    {
        //        if (string.IsNullOrEmpty(scanning.BARCODE))
        //        {
        //            // 바코드 스캔 종료 후 입력창 활성화.
        //            if (IsOpenPopup == false) IsOpenPopup = true;
        //            return;
        //        }
        //        else
        //        {
        //            //App.Current.MainPage.DisplayAlert("스캔 바코드 - 원청 바코드", scanning.BARCODE, "Ok");
        //            LotBarCo = scanning.BARCODE;

        //            // 바코드 스캔 종료 후 입력창 활성화.
        //            if (IsOpenPopup == false) IsOpenPopup = true;
        //        }
        //    };

        //    await App.Current.MainPage.Navigation.PushModalAsync(scanning);
        //    await Task.Run(() => waitHandle.WaitOne());
        //    ///App.Current.MainPage.DisplayAlert("", scanning.BARCODE, "Ok");
        //    //await Shell.Current.GoToAsync(nameof(FullScreenScanning));
        //    //Barcode = Application.Current.Properties["BARCODE_1"] as string;
        //    //Qty = "0";
        //    //});

        //}




        /// <summary>
        /// MVVM 방식으로 View 필요한 데이터를 바인딩합니다.
        /// </summary>
        #region MVVM 방식으로 View 필요한 데이터를 바인딩합니다.

        // 품묵구분
        private IList<MobileVo> ItmGrpList = new List<MobileVo>();

        // 강종
        private IList<MobileVo> N1stGrpList = new List<MobileVo>();
        public IList<MobileVo> ComboN1stGrpList
        {
            get => this.N1stGrpList;
            set => SetProperty(ref this.N1stGrpList, value);
        }

        private MobileVo _n1stGrpItem = new MobileVo();
        public MobileVo N1stGrpItem
        {
            get => this._n1stGrpItem;
            set
            {
                SetProperty(ref this._n1stGrpItem, value);
                N2ndChangeFunc();
            }
        }

        // 규격
        private IList<MobileVo> N2ndGrpList = new List<MobileVo>();
        public IList<MobileVo> ComboN2ndGrpList
        {
            get => this.N2ndGrpList;
            set => SetProperty(ref this.N2ndGrpList, value);
        }

        private MobileVo _n2ndGrpItem = new MobileVo();
        public MobileVo N2ndGrpItem
        {
            get => this._n2ndGrpItem;
            set
            {
                SetProperty(ref this._n2ndGrpItem, value);
                ItmLenChangeFunc();
            }
        }

        // 길이
        //private IList<MobileVo> LenList = new List<MobileVo>();
        //public IList<MobileVo> ComboLenList
        //{
        //    get => this.LenList;
        //    set => SetProperty(ref this.LenList, value);
        //}

        private MobileVo _lenItem = new MobileVo();
        public MobileVo LenItem
        {
            get => this._lenItem;
            set => SetProperty(ref this._lenItem, value);
        }

        // 길이
        int itmLen;
        public int ItmLen
        {
            get => this.itmLen;
            set
            {
                SetProperty(ref this.itmLen, value);
                ItmLenChanged();
            }
        }
        // 길이 입력 오류 여부
        bool lenActualHasError = false;
        public bool LenActualHasError
        {
            get => this.lenActualHasError;
            set => SetProperty(ref this.lenActualHasError, value);
        }
        // 길이 입력 오류 문구
        string lenActualErrorText;
        public string LenActualErrorText
        {
            get => this.lenActualErrorText;
            set => SetProperty(ref this.lenActualErrorText, value);
        }




        // 수불유형
        private IList<MobileVo> InaudList = new List<MobileVo>();
        public IList<MobileVo> ComboInaudList
        {
            get => this.InaudList;
            set => SetProperty(ref this.InaudList, value);
        }

        private MobileVo _inaudItem = new MobileVo();
        public MobileVo InaudItem
        {
            get => this._inaudItem;
            set => SetProperty(ref this._inaudItem, value);
        }

        // 창고
        private IList<MobileVo> LocList = new List<MobileVo>();
        public IList<MobileVo> ComboLocList
        {
            get => this.LocList;
            set => SetProperty(ref this.LocList, value);
        }

        private MobileVo _locItem = new MobileVo();
        public MobileVo LocItem
        {
            get => this._locItem;
            set => SetProperty(ref this._locItem, value);
        }

        // 입고일자
        DateTime inpDt;
        public DateTime InpDt
        {
            get => this.inpDt;
            set => SetProperty(ref this.inpDt, value);
        }
        // 입고수량
        object inpQty;
        public object InpQty
        {
            get => this.inpQty;
            set
            {
                SetProperty(ref this.inpQty, value);
                InpQtyChanged();
            }
        }
        // 입고수량 입력 오류 여부
        bool actualHasError = false;
        public bool ActualHasError
        {
            get => this.actualHasError;
            set => SetProperty(ref this.actualHasError, value);
        }
        // 입고수량 입력 오류 문구
        string actualErrorText;
        public string ActualErrorText
        {
            get => this.actualErrorText;
            set => SetProperty(ref this.actualErrorText, value);
        }

        private bool _isM_INSERT;
        public bool IsM_INSERT
        {
            get => this._isM_INSERT;
            set => SetProperty(ref this._isM_INSERT, value);
        }

        #endregion



        public HttpClient httpClient()
        {
            var ret = new HttpClient();

            try
            {
                ret.BaseAddress = new Uri(HTTP_URL);
                ret.DefaultRequestHeaders.Accept.Clear();
                ret.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }
            catch(Exception ex)
            {

            }

            return ret;
        }
    }
}
