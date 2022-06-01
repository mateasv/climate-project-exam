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
using XamarinBase.Models;
using XamarinBase.Services;
using System.Linq;
using XamarinBase.Views;

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
        


        public MainViewModel(ISignalRService signalRService, IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            _signalRService = signalRService;

            PlantsViewCmd = new Command(async () => await PlantsView());
            ConnectionViewCmd = new Command(async () => await ConnectionView());


            CurrentContentView = new PlantsView();
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

    











    //// chart test
    //var list = new List<Measurement>()
    //{
    //    new Measurement
    //    {
    //        MeasurementId = 1,
    //        AirTemerature = 11.11F,
    //    },
    //    new Measurement
    //    {
    //        MeasurementId = 2,
    //        AirTemerature = 21.11F,
    //    }
    //};

    //var chart = chartService.CreateChart<Measurement>(
    //    list,
    //    value: (m) => m.AirTemerature,
    //    label: (m) => m.MeasurementId.ToString()
    //);

    //Chart = chart;
}
