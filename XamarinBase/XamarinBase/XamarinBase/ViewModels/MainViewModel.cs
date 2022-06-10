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
    /// <summary>
    /// View model of the Main View. Shows the plants, and navigation to connection 
    /// edit view
    /// </summary>
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
            

            // change the current view of the page to the PlantsView
            PlantsView();
        }

        /// <summary>
        /// Handler for the dataloggers warning message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void _signalRService_OnReceiveWarning(object sender, WarningEventArgs e)
        {
            // return if there is no warning
            if (!e.IsWarning) return;

            // get the measurement
            var measurement = e.Measurement;

            // send notification to the app
            CrossLocalNotifications.Current.Show("Tree Warning", $"Tree {measurement.PlantId} with datalogger {measurement.DataloggerId} has dry soil or too low temperature");
        }

        /// <summary>
        /// Connects to signalR
        /// </summary>
        /// <returns></returns>
        public async Task ConnectSignalR()
        {
            // return if already connected
            if (_signalRService.IsConnected)
            {
                return;
            }

            try
            {
                // start connecting
                await _signalRService.StartAsync();

                // register the mobile app in the signalR server
                await _signalRService.RegisterApp();

                await Application.Current.MainPage.DisplayAlert("Alert", $"SignalR Success", "Ok", "Cancel");

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", $"SignalR error: {ex.Message}", "Ok", "Cancel");
            }
        }

        /// <summary>
        /// Navigates to the create/edit PlantDetailsView
        /// </summary>
        /// <returns></returns>
        public async Task CreatePlant()
        {
            await (App.Current.MainPage as NavigationPage).PushAsync(new PlantDetailsView());
        }

        /// <summary>
        /// Changes the current view of the page to the ConnectionView
        /// </summary>
        /// <returns></returns>
        public async Task ConnectionView()
        {
            CurrentContentView = new ConnectionView();
        }

        /// <summary>
        /// Changes the current view of the page to the PlantsView
        /// </summary>
        /// <returns></returns>
        public async Task PlantsView()
        {
            CurrentContentView = new PlantsView();
        }
    }
}
