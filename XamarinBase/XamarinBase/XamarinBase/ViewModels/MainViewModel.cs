using Microcharts;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinBase.Exstensions;
using XamarinBase.Services;
using System.Linq;
using XamarinBase.Views;
using XamarinBase.EventArguments;
using Plugin.LocalNotifications;

namespace XamarinBase.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ISignalRService _signalRService;
        private readonly IDatabaseService _databaseService;

        

        private ContentView _currentContentView;


        public ContentView CurrentContentView
        {
            get { return _currentContentView; }
            set { _currentContentView = value; OnPropertyChanged(); }
        }


        public ICommand PlantsViewCmd { get; set; }
        public ICommand ConnectionViewCmd { get; set; }
        public ICommand ConnectSignalRCmd { get; set; }


        public MainViewModel(ISignalRService signalRService, IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            _signalRService = signalRService;

            PlantsViewCmd = new Command(async () => await PlantsView());
            ConnectionViewCmd = new Command(async () => await ConnectionView());
            ConnectSignalRCmd = new Command(async () => await ConnectSignalR());

            _signalRService.OnReceiveWarning += _signalRService_OnReceiveWarning;
            _signalRService.Build();
            


            PlantsView();
        }

        private async void _signalRService_OnReceiveWarning(object sender, WarningEventArgs e)
        {
            if (!e.IsWarning) return;

            var measurement = e.Measurement;

            CrossLocalNotifications.Current.Show("Tree Warning", $"Tree {measurement.PlantId} with datalogger {measurement.DataloggerId} has dry soil or too low temperature");
        }

        public async Task ConnectSignalR()
        {
            if (_signalRService.IsConnected)
            {
                return;
            }

            try
            {
                await _signalRService.StartAsync();
                await _signalRService.RegisterApp();
                await Application.Current.MainPage.DisplayAlert("Alert", $"SignalR Success", "Ok", "Cancel");

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", $"SignalR error: {ex.Message}", "Ok", "Cancel");
            }
        }

        public async Task CreatePlant()
        {
            await (App.Current.MainPage as NavigationPage).PushAsync(new PlantDetailsView());
        }
        public async Task ConnectionView()
        {
            CurrentContentView = new ConnectionView();
        }


        public async Task PlantsView()
        {
            CurrentContentView = new PlantsView();
        }
    }
}
