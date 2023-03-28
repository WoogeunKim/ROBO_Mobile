using System;
using Xamarin.Forms;
using ZXing;
using ZXing.Net.Mobile.Forms;

namespace ROBO_Mobile.Views.Dialog
{
	public partial class FullScreenScanning : ZXingScannerPage
	{
		public FullScreenScanning()
		{
			InitializeComponent();

			InitScanner();

            //this.btn_Cancel.Clicked += new System.EventHandler(Btn_Cancel_Clicked);

        }

  //      private async void Btn_Cancel_Clicked(object sender, EventArgs e)
		//{
  //          // This will pop the current page off the navigation stack
  //          await Shell.Current.GoToAsync("..");
  //      }

		void InitScanner()
		{
			IsAnalyzing = true;
			IsScanning = true;
		}

		public async void Handle_OnScanResult(Result result)
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				IsScanning = false;
				IsAnalyzing = false;

				BARCODE = result.Text;

				await Navigation.PopModalAsync();
				//await DisplayAlert("Scanned result", result.Text, "OK");

				//async void OnItemSelected(PdaVo item)
				//{
				//	if (item == null)
				//		return;

				// This will push the ItemDetailPage onto the navigation stack
				//	await Shell.Current.GoToAsync($"{nameof(P01DetailPage)}?{nameof(P01DetailViewModel.ItemId)}={(item.PUR_ORD_NO + "_" + item.PUR_ORD_SEQ)}");
				//}
				//Application.Current.Properties["BARCODE_1"] = result.Text;

				//await Shell.Current.GoToAsync($"..");
				//await Shell.Current.GoToAsync($"..?{ nameof(P01DetailViewModel.ItemId)}={(result.Text)}");
			});
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			IsScanning = true;
			IsAnalyzing = true;
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			IsScanning = false;
			IsAnalyzing = false;
		}


        public string BARCODE { get; set; }

    }
}