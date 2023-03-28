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

namespace ROBO_Mobile.ViewModels
{

    public class DataGridViewModel : BaseViewModel
    {
        private string _title = "원자재 입고";

        private string HTTP_URL = Application.Current.Properties["URL"] as string;
        private string CHNL_CD = Application.Current.Properties["CHNL_CD"] as string;
        private string USER_ID = Application.Current.Properties["USR_ID"] as string;

        private HttpResponseMessage response;
        private HttpClient client;



        public DataGridViewModel()
        {
            Title = "원자재 입고";
            ComboCoList = new List<MobileVo>();

            SYSTEM_CODE_VO();


            //바코드 호출
            BarCodeCommand = new Command(async () => await ExecuteBarCodeCommand());
        }

        public async void SYSTEM_CODE_VO()
        {
            try
            {
                // 매입처 조회
                using (client = httpClient())
                {
                    using (response = await client.PostAsync("mobile/inp/m/co", new StringContent(JsonConvert.SerializeObject(new MobileVo() { CHNL_CD = this.CHNL_CD, DELT_FLG = "N" }), System.Text.Encoding.UTF8, "application/json")))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            List<MobileVo> resultVo = JsonConvert.DeserializeObject<IEnumerable<MobileVo>>(await response.Content.ReadAsStringAsync()).Cast<MobileVo>().ToList();

                            if (resultVo == null)
                            {
                                await App.Current.MainPage.DisplayAlert("매입처 실패", "매입처가 존재하지 않습니다", "Ok");
                                return;
                            }

                            ComboCoList = resultVo;
                            //ComboCoItem = resultVo[0];
                        }
                    }
                }
            }
            catch (Exception eLog)
            {
                await App.Current.MainPage.DisplayAlert(Title + " - 매입처 오류", eLog.Message, "OK");
                return;
            }
        }

        private async void CoChangeInMst()
        {
            try
            {
                if (ComboCoItem == null) return;

                IsOpenPopup = false;

                // 발주 조회
                using (client = httpClient())
                {
                    using (response = await client.PostAsync("mobile/inp/m/ord", new StringContent(JsonConvert.SerializeObject(ComboCoItem), System.Text.Encoding.UTF8, "application/json")))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            InItmMstList = JsonConvert.DeserializeObject<IEnumerable<MobileVo>>(await response.Content.ReadAsStringAsync()).Cast<MobileVo>().ToList();

                            if (InItmMstList.Count > 0)
                            {

                                InItemMst = InItmMstList[0];
                            }
                        }
                    }
                }
            }
            catch (Exception eLog)
            {
                await App.Current.MainPage.DisplayAlert(Title + " - 발주 오류", eLog.Message, "OK");
                return;
            }
        }

        /// <summary>
        /// 발주목록에서 해당 발주건에 대해 입고 선택(슬라이드) 할 경우 (입고일자&입고수량 입력할 수 있는) 탭을 활성화 합니다. 
        /// </summary>
        private Command insertItemContactCommand;
        public Command InsertItemContactCommand
        {
            get { return insertItemContactCommand ?? (this.insertItemContactCommand = new Command(this.GridTapped)); }
        }
        private async void GridTapped()
        {
            try
            {
                // (Label) 발주번호 + 순번 + 제품명
                this.PopupTitle = InItemMst.PUR_ORD_NO + " / " + InItemMst.PUR_ORD_SEQ + " : " + InItemMst.ITM_NM;
                // (DateEdit) 입고일자 = 현재
                this.InpDt = DateTime.Now.Date;
                // (NumericEdit) 일고수량 = 발주수량
                //this.InpQty = InItemMst.ITM_QTY;
                // 한개씩 원청바코드 입력
                this.InpQty = 1.0;
                // (TextEdit) 원청바코드
                this.LotBarCo = "";


                // 탭 활성화 "true"
                if (this.IsOpenPopup == false) this.IsOpenPopup = true;
            }
            catch (Exception eLog)
            {
                await App.Current.MainPage.DisplayAlert(Title + " - 입고 등록 오류", eLog.Message, "OK");
                return;
            }
        }


        /// <summary>
        /// 입고 탭에서 취소 할 때 해당 정보를 초기화 후 탭을 닫습니다/.
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
                // (DateEdit) 입고일자 초기화
                this.InpDt = DateTime.Now.Date;
                // (NumericEdit) 초기화
                this.InpQty = 0.0;
                // (NumericEdit.HasError) 오류 유효검사
                this.ActualHasError = false;
                // 탭 비활성화 "true"
                this.IsOpenPopup = false;
            }
            catch (Exception eLog)
            {
                await App.Current.MainPage.DisplayAlert(Title + " - 입고 탭 취소 오류", eLog.Message, "OK");
                return;
            }
        }
        /// <summary>
        /// 입고 탭에서 (입고일자&입고수량) 입력 동작을 할 때 해당 정보를 서버에 저장합니다.
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
                    if (String.IsNullOrEmpty(LotBarCo))
                    {
                        //실패
                        await App.Current.MainPage.DisplayAlert(Title + " - 유효검사 ", "[원청바코드] 를 입력하세요", "OK");
                    }
                    else
                    {
                        // 원청바코드 중복검사 (없을 경우 true)
                        if (await ValueLotValidate(LotBarCo) == true)
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


                                    // 발주 다시 조회
                                    CoChangeInMst();
                                }
                            }

                            // 초기화
                            // (DateEdit) 입고일자 초기화
                            //this.InpDt = DateTime.Now.Date;
                            // (NumericEdit) 초기화
                            //this.InpQty = 0.0;
                            // (TextEdit) 원청바코드
                            //this.LotBarCo = "";
                            // 탭 (활성화) 유지 "true"
                            //this.IsOpenPopup = true;

                            // (NumericEdit.HasError) 오류 유효검사
                            this.ActualHasError = false;
                            // 초기화 & 추가 입력
                            GridTapped();
                        }
                    }
                }                
            }
            catch (Exception eLog)
            {
                await App.Current.MainPage.DisplayAlert(Title + " - 입고 탭 등록 오류", eLog.Message, "OK");
                return;
            }
        }

        /// <summary>
        /// 원청바코드 중복검사를 진행합니다.
        /// JSON 통해 서버로부터 정보를 조회합니다.
        /// </summary>
        /// <param name="barCd"></param>
        /// <returns></returns>
        private async Task<bool> ValueLotValidate(string barCd)
        {
            var ret = false;

            try
            {
                // 바코드 입력을 안할 경우
                if (String.IsNullOrEmpty(LotBarCo))
                {
                    // 경고
                    await App.Current.MainPage.DisplayAlert(Title + " - 유효검사 ", "[원청바코드] 를 입력하세요", "OK");
                }
                else
                {
                    using (client = httpClient())
                    {
                        using (HttpResponseMessage response = await client.PostAsync("mobile/inp/lot/count"
                                                                              , new StringContent(JsonConvert.SerializeObject(new MobileVo() { LOT_NO = barCd, CHNL_CD = this.CHNL_CD }), System.Text.Encoding.UTF8, "application/json")))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var retNum = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());

                                // 동일한 바코드가 없을 경우
                                if (retNum < 1)
                                {
                                    ret = true;
                                }
                                else
                                {
                                    // 경고
                                    await App.Current.MainPage.DisplayAlert(Title + " - 유효검사", "동일한 [원청바코드] 가 존재합니다", "OK");
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception eLog)
            {
                //실패
                await App.Current.MainPage.DisplayAlert(Title + " - 원청바코드 유효 검사", eLog.Message, "OK");
            }

            return ret;
        }


        private MobileVo GetDomain()
        {
            MobileVo ret = new MobileVo();

            ret.PUR_ORD_NO = InItemMst.PUR_ORD_NO;
            ret.PUR_ORD_SEQ = InItemMst.PUR_ORD_SEQ;
            ret.CO_NO = InItemMst.CO_NO;
            ret.ITM_CD = InItemMst.ITM_CD;

            ret.ITM_QTY = InpQty;
            ret.LOT_NO = LotBarCo;
            ret.INAUD_DT = InpDt.ToString("yyyy-MM-dd");

            ret.CHNL_CD = this.CHNL_CD;
            ret.AREA_CD = "001";
            ret.CRE_USR_ID = USER_ID;

            return ret;
        }


        /// <summary>
        /// 입고수량 유효검사 합니다.
        /// </summary>
        //private Command inpQtyValueChangedCommand;
        //public Command InpQtyValueChangedCommand
        //{
        //    get { return inpQtyValueChangedCommand ?? (this.inpQtyValueChangedCommand = new Command(this.InpQtyChanged)); }
        //}
        public async void InpQtyChanged()
        {
            try
            {
                int _Num = 0;

                // 입력한 수량이 정수가 아닐 경우
                if (int.TryParse(InpQty.ToString(), out _Num) == false)
                {
                    this.ActualErrorText = "입고수량을 정수형태로 입력하세요";
                    this.ActualHasError = true;
                    // 입고확인 버튼 비활성화
                    this.IsM_INSERT = false;
                }
                else if (int.Parse(InpQty.ToString()) <= 0)
                {
                    this.ActualErrorText = "수량을 다시 입력하세요";
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
            scanning.Disappearing += (sender2, e2) =>
            {
                if (string.IsNullOrEmpty(scanning.BARCODE))
                {
                    // 바코드 스캔 종료 후 입력창 활성화.
                    if (IsOpenPopup == false) IsOpenPopup = true;
                    return;
                }
                else
                {
                    //App.Current.MainPage.DisplayAlert("스캔 바코드 - 원청 바코드", scanning.BARCODE, "Ok");
                    LotBarCo = scanning.BARCODE;

                    // 바코드 스캔 종료 후 입력창 활성화.
                    if (IsOpenPopup == false) IsOpenPopup = true;
                }
            };

            await App.Current.MainPage.Navigation.PushModalAsync(scanning);
            await Task.Run(() => waitHandle.WaitOne());
            ///App.Current.MainPage.DisplayAlert("", scanning.BARCODE, "Ok");
            //await Shell.Current.GoToAsync(nameof(FullScreenScanning));
            //Barcode = Application.Current.Properties["BARCODE_1"] as string;
            //Qty = "0";
            //});

        }

        //async public void OnAppearing()
        //{
        //    //IEnumerable<Item> items = await DataStore.GetItemsAsync(true);
        //    //Items.Clear();
        //    //foreach (Item item in items)
        //    //{
        //    //    Items.Add(item);
        //    //}
        //}


        /// <summary>
        /// MVVM 방식으로 View 필요한 데이터를 바인딩합니다.
        /// </summary>
        #region MVVM 방식으로 View 필요한 데이터를 바인딩합니다.

        private IList<MobileVo> CoList = new List<MobileVo>();
        public IList<MobileVo> ComboCoList
        {
            get => this.CoList;
            set => SetProperty(ref this.CoList, value);
        }

        private MobileVo CoItem = new MobileVo();
        public MobileVo ComboCoItem
        {
            get => this.CoItem;
            set
            {
                SetProperty(ref this.CoItem, value);
                CoChangeInMst();
            }
        }

        private IList<MobileVo> MstList = new List<MobileVo>();
        public IList<MobileVo> InItmMstList
        {
            get => this.MstList;
            set => SetProperty(ref this.MstList, value);
        }

        private MobileVo InItem = new MobileVo();
        public MobileVo InItemMst
        {
            get => this.InItem;
            set => SetProperty(ref this.InItem, value);
        }

        // 입고 탭 여부
        bool isOpenPopup = false;
        public bool IsOpenPopup
        {
            get => this.isOpenPopup;
            set => SetProperty(ref this.isOpenPopup, value);
        }
        // 입고품목 정보
        string popupTitle;
        public string PopupTitle
        {
            get => this.popupTitle;
            set => SetProperty(ref this.popupTitle, value);
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
        // 입고확인 버튼 활성화 여부
        bool isM_INSERT;
        public bool IsM_INSERT
        {
            get => this.isM_INSERT;
            set => SetProperty(ref this.isM_INSERT, value);
        }
        // 원청바코드
        string lotBarCo;
        public string LotBarCo
        {
            get => this.lotBarCo;
            set => SetProperty(ref this.lotBarCo, value);
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
