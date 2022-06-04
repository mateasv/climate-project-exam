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
using XamarinBase.Views;

namespace XamarinBase.ViewModels
{
    public class PlantsViewModel : BaseViewModel
    {
        private readonly ISignalRService _signalRService;
        private readonly IDatabaseService _databaseService;
        private readonly PlantDetailsViewModel _plantDetailsViewModel;
        private readonly EditPlantViewModel _editPlantViewModel;
        private readonly EditDataloggerViewModel _editDataloggerViewModel;

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
        public ICommand CreatePlantCmd { get; set; }


        public PlantsViewModel(ISignalRService signalRService, IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            _signalRService = signalRService;

            
        }

        public PlantsViewModel(ISignalRService signalRService, 
            IDatabaseService databaseService, 
            PlantDetailsViewModel plantDetailsViewModel, 
            EditPlantViewModel editPlantViewModel, 
            EditDataloggerViewModel editDataloggerViewModel)
        {
            _signalRService = signalRService;
            _databaseService = databaseService;
            _plantDetailsViewModel = plantDetailsViewModel;
            _editPlantViewModel = editPlantViewModel;
            _editDataloggerViewModel = editDataloggerViewModel;

            PlantViewModels = new ObservableCollection<PlantViewModel>();

            GetPlantsCmd = new Command(async () => await GetPlants(), () => CanExecuteGetPlants);
            ViewPlantDetailsCmd = new Command(async (plantViewModel) => await ViewPlantDetails(plantViewModel));
            CreatePlantCmd = new Command(async () => await CreatePlant());


            CanExecuteGetPlants = true;
        }

        public async Task CreatePlant()
        {
            _editPlantViewModel.Reset();
            _editDataloggerViewModel.Reset();

            _editPlantViewModel.GetPlantTypesCmd.Execute(this);
            _plantDetailsViewModel.PlantDetailsCmd.Execute(this);

            await (Application.Current.MainPage as NavigationPage).PushAsync(new PlantDetailsView());
        }

        public async Task ViewPlantDetails(object obj)
        {
            if (obj is null) return;

            var plantViewModel = obj as PlantViewModel;

            _editPlantViewModel.Reset();
            _editDataloggerViewModel.Reset();

            //_editPlantViewModel.GetPlantTypesCmd.Execute(this);
            //_plantDetailsViewModel.PlantDetailsCmd.Execute(this);

            await _editPlantViewModel.GetPlantTypes();
            await _plantDetailsViewModel.PlantDetails();

            _editPlantViewModel.PlantViewModel = plantViewModel;

            try
            {
                if (plantViewModel.Plant.DataloggerId != null)
                {
                    var res = await _databaseService.GetAsync<Datalogger>(plantViewModel.Plant.DataloggerId.GetValueOrDefault());
                    if (!res.IsSuccessStatusCode)
                    {
                        await Application.Current.MainPage.DisplayAlert("Alert", $"HTTP error: {res.StatusCode} Error in getting paired datalogger", "Ok", "Cancel");
                    }
                    else
                    {
                        var datalogger = await res.ContentToObjectAsync<Datalogger>();

                        _editDataloggerViewModel.DataloggerViewModel = new DataloggerViewModel() { Datalogger = datalogger };
                    }

                }
            }
            catch(Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", $"HTTP error: {ex.Message} Error in getting plant view details", "Ok", "Cancel");
            }

            await (Application.Current.MainPage as NavigationPage).PushAsync(new PlantDetailsView());
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
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Alert", $"HTTP error: {res.StatusCode}", "Ok", "Cancel");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", $"HTTP error: {ex.Message} Error in getting plants", "Ok", "Cancel");
            }
        }

        
    }
}
