using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using Android;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using static Android.Resource;
using Plugin.Geolocator;

namespace Fourplaces.Droid
{
    [Activity(Label = "Fourplaces", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        readonly string[] Permissions =
           {
              Manifest.Permission.AccessCoarseLocation,
              Manifest.Permission.AccessFineLocation,
              Manifest.Permission.Camera,
              Manifest.Permission.WriteExternalStorage

            };

        const int RequestPermissionId = 0;

        public View l;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::Xamarin.FormsMaps.Init(this, savedInstanceState); //to use Maps

            GetLocationCompatAsync(); //to get permission granted for location

            LoadApplication(new App());
        }


        public void GetLocationCompatAsync()
        {
            const string permission = Manifest.Permission.AccessFineLocation;
            if (ContextCompat.CheckSelfPermission(this, permission) == (int)Permission.Granted)
            {
                Console.WriteLine("Dev_LocationGranted");


                return;
            }

            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, permission))
            {

                Snackbar.Make(l, "Permissions access is required to get the best experience.", Snackbar.LengthIndefinite)
                        .SetAction("OK", v => ActivityCompat.RequestPermissions(this, Permissions, RequestPermissionId))
                        .Show();

                return;
            }

            ActivityCompat.RequestPermissions(this, Permissions, RequestPermissionId);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


    }
}