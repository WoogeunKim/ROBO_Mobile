using System;

using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.Content;
using FFImageLoading.Forms.Platform;
using Xam.Plugin.AutoUpdate.Droid;

namespace ROBO_Mobile.Droid
{
    [Activity(Label = "ROBOCON", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AutoUpdate.Init(this, "com.JSmartSolution.robo_mobile.fileProvider");

            CachedImageRenderer.Init(true);
            Rg.Plugins.Popup.Popup.Init(this);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) != (int)Permission.Granted)
            {
                RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage, Manifest.Permission.RequestInstallPackages }, 0);
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        // 뒤로가기 시 종료 버그 방지
        public override void OnBackPressed()
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
            builder.SetPositiveButton("확인", (senderAlert, args) => {
                Finish();
            });

            builder.SetNegativeButton("취소", (senderAlert, args) => {
                return;
            });

            Android.App.AlertDialog alterDialog = builder.Create();
            alterDialog.SetTitle("알림 - ROBOCON");
            alterDialog.SetMessage("프로그램 종료 하시겠습니까?");
            alterDialog.Show();
        }
    }
}