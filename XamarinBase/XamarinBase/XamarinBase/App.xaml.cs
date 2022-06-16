using Microsoft.Extensions.DependencyInjection;
using Server.Models;
using System;
using Xamarin.Forms;
using XamarinBase.Services;
using XamarinBase.ViewModels;
using XamarinBase.Views;
using ZXing.Net.Mobile.Forms;
using XamarinBase.Exstensions;
using System.Linq;

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

            var navigationPage = new NavigationPage();
            MainPage = navigationPage;

            SetupServices(addPlatformServices);

            
            navigationPage.PushAsync(new MainView());
        }

        void SetupServices(Action<IServiceCollection> addPlatformServices = null)
        {
            var services = new ServiceCollection();

            // Add platform specific services
            addPlatformServices?.Invoke(services);

            // Add ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<ConnectionViewModel>();
            services.AddSingleton<PlantDetailsViewModel>();
            services.AddSingleton<PlantsViewModel>();
            services.AddSingleton<EditPlantViewModel>();
            services.AddSingleton<EditDataloggerViewModel>();
            services.AddSingleton<ChartViewModel>();

            // Add core services
            services.AddSingleton<IDatabaseService,DatabaseService>();
            services.AddSingleton<ISignalRService,SignalRService>();
            services.AddSingleton<IChartService,ChartService>();

            // Setup of services that are dependant on specific platforms
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    // add Android implementation of IHttpClientHandler service
                    services.AddSingleton(DependencyService.Get<IHTTPClientHandlerCreationService>());
                    break;
                case Device.iOS:
                    // add iOS implementation of IHttpClientHandler service
                    //services.AddSingleton(DependencyService.Get<IHTTPClientHandlerCreationServiceIOS>());
                    break;
                default:
                    // add other implementation of IHttpClientHandler service
                    //services.AddSingleton(DependencyService.Get<IHTTPClientHandlerCreationServiceOtherPlatform>());
                    break;
            }

            

            ServiceProvider = services.BuildServiceProvider();
        }


        protected override async void OnStart()
        {
            var mainViewModel = GetViewModel<MainViewModel>() as MainViewModel;
            await mainViewModel.ConnectSignalR();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        
    }
}
