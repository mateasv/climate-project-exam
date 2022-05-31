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

        private ObservableCollection<PlantViewModel> _plantViewModels;

        public ObservableCollection<PlantViewModel> PlantViewModels
        {
            get { return _plantViewModels; }
            set { _plantViewModels = value; }
        }




        public ICommand GetPlantsCmd { get; set; }
        public ICommand ViewPlantDetailsCmd { get; set; }

        public MainViewModel(ISignalRService signalRService, IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            _signalRService = signalRService;

            _plantViewModels = new ObservableCollection<PlantViewModel>();

            GetPlantsCmd = new Command(async () => await GetPlants());
            ViewPlantDetailsCmd = new Command(async (plantViewModel) => await ViewPlantDetails(plantViewModel));
        }

        public async Task GetPlants()
        {
            try
            {
                var res = await _databaseService.GetAsync<Plant>();

                if (res.IsSuccessStatusCode)
                {
                    var plants = await res.ContentToCollectionAsync<Plant>();

                    PlantViewModels.Clear();

                    plants.ToList().ForEach(plant => PlantViewModels.Add(new PlantViewModel { Plant = plant }));
                    await Application.Current.MainPage.DisplayAlert("Alert", $"{res.StatusCode}", "Cancel", "ok");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Alert", $"HTTP error: {res.StatusCode}", "Cancel", "ok");
                }
            }
            catch(Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", $"HTTP error: {ex.Message}", "Cancel", "ok");
                await (Application.Current.MainPage as NavigationPage).PushAsync(new ConnectionView());
            }
        }

        public async Task ViewPlantDetails(object obj)
        {
            await (Application.Current.MainPage as NavigationPage).PushAsync(new PlantDetailsView());
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
