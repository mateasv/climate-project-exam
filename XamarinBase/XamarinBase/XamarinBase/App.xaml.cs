using Microsoft.Extensions.DependencyInjection;
using System;
using Xamarin.Forms;
using XamarinBase.Services;
using XamarinBase.ViewModels;

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

            MainPage = new MainPage();
        }

        void SetupServices(Action<IServiceCollection> addPlatformServices = null)
        {
            var services = new ServiceCollection();

            // Add platform specific services
            addPlatformServices?.Invoke(services);

            // Add ViewModels
            services.AddTransient<MainViewModel>();

            // Add core services
            services.AddSingleton<IDataService, SampleDataService>();

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
