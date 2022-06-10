using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Microsoft.Extensions.DependencyInjection;
using XamarinBase.Services;
using Javax.Net.Ssl;
using Xamarin.Android.Net;
using Android.Net;
using System.Net.Http;
using Xamarin.Forms;
using XamarinBase.Droid;
using Android;
using Plugin.LocalNotifications;


// Add Android implementation of the HttpClientHandler into the common dependency container
[assembly: Dependency(typeof(HTTPClientHandlerCreationService_Android))]
namespace XamarinBase.Droid
{
    /// <summary>
    /// Android implementation of the Http client handler used in the DatabaseService
    /// to ignore SSL verification
    /// </summary>
    public class HTTPClientHandlerCreationService_Android : IHTTPClientHandlerCreationService
    {
        /// <summary>
        /// Gets the android http client handler
        /// </summary>
        /// <returns></returns>
        public HttpClientHandler GetInsecureHandler()
        {
            return new IgnoreSSLClientHandler();
        }
    }

    /// <summary>
    /// Class extending the AndroidClientHandler and used to override necessary SSL
    /// responsible methods.
    /// </summary>
    internal class IgnoreSSLClientHandler : AndroidClientHandler
    {
        
        protected override SSLSocketFactory ConfigureCustomSSLSocketFactory(HttpsURLConnection connection)
        {
            return SSLCertificateSocketFactory.GetInsecure(1000, null);
        }

        protected override IHostnameVerifier GetSSLHostnameVerifier(HttpsURLConnection connection)
        {
            return new IgnoreSSLHostnameVerifier();
        }
    }

    internal class IgnoreSSLHostnameVerifier : Java.Lang.Object, IHostnameVerifier
    {
        public bool Verify(string hostname, ISSLSession session)
        {
            return true;
        }
    }

    [Activity(Label = "XamarinBase", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        static void AddServices(IServiceCollection services)
        {
            //services.AddSingleton<IAppInfoService, AppInfoService>();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            askPermission();

            //LocalNotificationsImplementation.NotificationIconId = Resource.Drawable.tmouse;
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();

            LoadApplication(new App(AddServices));
        }

        protected void askPermission()
        {
            //base.OnStart();

            if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessNotificationPolicy) == (int)Permission.Granted
                 && Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this, Manifest.Permission.SystemAlertWindow) == (int)Permission.Granted)
            {
                System.Diagnostics.Debug.WriteLine("Permissions Granted!!!");
            }
            else
            {
                Android.Support.V4.App.ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.AccessNotificationPolicy,
                                                                                            Manifest.Permission.SystemAlertWindow,
                                                                                            }, 0);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}