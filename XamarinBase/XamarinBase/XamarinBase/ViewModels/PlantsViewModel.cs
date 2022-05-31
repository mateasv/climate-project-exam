using Server.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinBase.Exstensions;
using XamarinBase.Services;

namespace XamarinBase.ViewModels
{
    public class PlantsViewModel : BaseViewModel
    {
        private readonly ISignalRService _signalRService;
        private readonly IDatabaseService _databaseService;

        private ObservableCollection<PlantViewModel> _plantViewModels;
        private bool _canOnLoadExecute;

        public bool CanExecuteGetPlants
        {
            get { return _canOnLoadExecute; }
            set { _canOnLoadExecute = value; (GetPlantsCmd as Command).ChangeCanExecute(); }
        }

        public ObservableCollection<PlantViewModel> PlantViewModels
        {
            get { return _plantViewModels; }
            set { _plantViewModels = value; }
        }

        public ICommand GetPlantsCmd { get; set; }
        public ICommand ViewPlantDetailsCmd { get; set; }

        

        public PlantsViewModel(ISignalRService signalRService, IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            _signalRService = signalRService;

            PlantViewModels = new ObservableCollection<PlantViewModel>();

            GetPlantsCmd = new Command(async () => await GetPlants(), () => CanExecuteGetPlants);
            ViewPlantDetailsCmd = new Command(async (plantViewModel) => await ViewPlantDetails(plantViewModel));

            CanExecuteGetPlants = true;
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
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", $"HTTP error: {ex.Message}", "Cancel", "ok");
            }
        }

        public async Task ViewPlantDetails(object obj)
        {
            //await (Application.Current.MainPage as NavigationPage).PushAsync(new PlantDetailsView());
        }
    }
}
