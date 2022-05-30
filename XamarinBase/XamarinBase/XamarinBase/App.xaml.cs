using Microsoft.Extensions.DependencyInjection;
using System;
using Xamarin.Forms;
using XamarinBase.Services;
using XamarinBase.ViewModels;
using XamarinBase.Views;
using ZXing.Net.Mobile.Forms;

namespace XamarinBase
{
    public partial class App : Application
    {
        protected static IServiceProvider ServiceProvider { get; set; }

        public static BaseViewModel GetViewModel<TViewModel>() where TViewModel : BaseViewModel
            => ServiceProvider.GetService<TViewModel>();

        public App(Action<IServiceCollection> addPlatformServices = null)
        {
            InitializeComponent();

            SetupServices(addPlatformServices);

            var navigationPage = new NavigationPage();
            //navigationPage.PushAsync(new BarcodeView());
            navigationPage.PushAsync(new MainPage());

            //MainPage = navigationPage;
            //MainPage = new CameraView();
            MainPage = navigationPage;
        }

        void SetupServices(Action<IServiceCollection> addPlatformServices = null)
        {
            var services = new ServiceCollection();

            // Add platform specific services
            addPlatformServices?.Invoke(services);

            // Add ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<ConnectionViewModel>();

            // Add core services
            services.AddSingleton<IDatabaseService,DatabaseService>();
            services.AddSingleton<ISignalRService,SignalRService>();
            services.AddSingleton<IChartService,ChartService>();

            ServiceProvider = services.BuildServiceProvider();
        }


        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
