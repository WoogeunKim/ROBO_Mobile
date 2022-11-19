using System;
using ROBO_Mobile.Models;
using System.Net.Http;
using Xamarin.Forms;
using Newtonsoft.Json;
using Xamarin.Essentials;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Xamarin.Forms.Internals;
using DevExpress.XamarinForms.DataGrid;

namespace ROBO_Mobile.ViewModels
{

    public class DataGridViewModel : BaseViewModel
    {
        private string HTTP_URL = Application.Current.Properties["URL"] as string;
        private string CHNL_CD = Application.Current.Properties["CHNL_CD"] as string;
        private HttpResponseMessage response;
        private HttpClient client;


        private IList<MobileVo> CoList = new List<MobileVo>();
        private MobileVo CoItem = new MobileVo();
        private IList<MobileVo> MstList = new List<MobileVo>();
        private MobileVo InItem = new MobileVo();

        public DataGridViewModel()
        {
            Title = "원자재 입고";
            ComboCoList = new List<MobileVo>();

            SYSTEM_CODE_VO();
        }

        public async void SYSTEM_CODE_VO()
        {
            try
            {
                // 매입처 조회
                using (client = new HttpClient())
                {
                    client.BaseAddress = new Uri(HTTP_URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    using (response = await client.PostAsync("mobile/m/co", new StringContent(JsonConvert.SerializeObject(new MobileVo() { CHNL_CD = this.CHNL_CD, DELT_FLG = "N" }), System.Text.Encoding.UTF8, "application/json")))
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

                // 발주 조회
                using (client = new HttpClient())
                {
                    client.BaseAddress = new Uri(HTTP_URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    using (response = await client.PostAsync("mobile/m/ord", new StringContent(JsonConvert.SerializeObject(ComboCoItem), System.Text.Encoding.UTF8, "application/json")))
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

        //InsertItemContactTap
        private Command insertItemContactCommand;
        public Command InsertItemContactCommand
        {
            get { return insertItemContactCommand ?? (this.insertItemContactCommand = new Command(this.GridTapped)); }
        }
        private async void GridTapped()
        {
            try
            {
               
            }
            catch (Exception eLog)
            {
                await App.Current.MainPage.DisplayAlert(Title + " - 입고 오류", eLog.Message, "OK");
                return;
            }
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


        public IList<MobileVo> ComboCoList
        {
            get => this.CoList;
            set => SetProperty(ref this.CoList, value);
        }

        public MobileVo ComboCoItem
        {
            get => this.CoItem;
            set
            {
                SetProperty(ref this.CoItem, value); 
                CoChangeInMst();
            }
        }

        public IList<MobileVo> InItmMstList
        {
            get => this.MstList;
            set => SetProperty(ref this.MstList, value);
        }

        public MobileVo InItemMst
        {
            get => this.InItem;
            set => SetProperty(ref this.InItem, value);
        }

    }
}
