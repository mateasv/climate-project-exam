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
    /// <summary>
    /// View model for the plants view. Responsible for creating plants, list all plants,
    /// navigate to the view showing details of a specific plant.
    /// </summary>
    public class PlantsViewModel : BaseViewModel
    {
        private readonly ISignalRService _signalRService;
        private readonly IDatabaseService _databaseService;
        private readonly PlantDetailsViewModel _plantDetailsViewModel;
        private readonly EditPlantViewModel _editPlantViewModel;
        private readonly EditDataloggerViewModel _editDataloggerViewModel;

        private ObservableCollection<PlantViewModel> _plantViewModels;

        public ObservableCollection<PlantViewModel> PlantViewModels
        {
            get { return _plantViewModels; }
            set { _plantViewModels = value; }
        }

        public ICommand GetPlantsCmd { get; set; }
        public ICommand ViewPlantDetailsCmd { get; set; }
        public ICommand CreatePlantCmd { get; set; }

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

            GetPlantsCmd = new Command(async () => await GetPlants());
            ViewPlantDetailsCmd = new Command(async (plantViewModel) => await ViewPlantDetails(plantViewModel));
            CreatePlantCmd = new Command(async () => await CreatePlant());
        }

        /// <summary>
        /// Create a new plant, navigate to the PlantDetailsView
        /// </summary>
        /// <returns></returns>
        public async Task CreatePlant()
        {
            // reset the plant and datalogger viewmodel in the EditPlantViewodel and EditDataloggerViewModel
            _editPlantViewModel.Reset();
            _editDataloggerViewModel.Reset();

            // Get the available plant types from the database
            _editPlantViewModel.GetPlantTypesCmd.Execute(this);

            // change the current view of the Plant Details page to the EditPlantView
            _plantDetailsViewModel.PlantDetailsCmd.Execute(this);

            // navigate to the plant details view
            await (Application.Current.MainPage as NavigationPage).PushAsync(new PlantDetailsView());
        }

        /// <summary>
        /// Prepares the ViewPlantDetails view and navigates to it.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task ViewPlantDetails(object obj)
        {
            // return if obj is null
            if (obj is null) return;

            // cast the obj as an PlantViewModel
            var plantViewModel = obj as PlantViewModel;

            // prepare the edit viewmodels of the datalogger and plant view
            _editPlantViewModel.Reset();
            _editDataloggerViewModel.Reset();

            // get the available plant types from the database
            await _editPlantViewModel.GetPlantTypes();

            // change the current view of the page to the PlantDetailsView
            await _plantDetailsViewModel.PlantDetails();

            // set the PlantViewModel of the EditPlantViewModel to the clicked PlantViewModel
            _editPlantViewModel.PlantViewModel = plantViewModel;

            try
            {
                // if the datalogger of the PlantViewModel is not null
                if (plantViewModel.Plant.DataloggerId != null)
                {
                    // get the datalogger of the plant
                    var res = await _databaseService.GetAsync<Datalogger>(plantViewModel.Plant.DataloggerId.GetValueOrDefault());

                    // if datalogger is not found
                    if (!res.IsSuccessStatusCode)
                    {
                        await Application.Current.MainPage.DisplayAlert("Alert", $"HTTP error: {res.StatusCode} Error in getting paired datalogger", "Ok", "Cancel");
                    }
                    else
                    {
                        // prepare the datalogger for the EditDataloggerViewModel
                        var datalogger = await res.ContentToObjectAsync<Datalogger>();

                        _editDataloggerViewModel.DataloggerViewModel = new DataloggerViewModel() { Datalogger = datalogger };
                    }
                }
            }
            catch(Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", $"HTTP error: {ex.Message} Error in getting plant view details", "Ok", "Cancel");
            }

            // navigate to the Plant Details View
            await (Application.Current.MainPage as NavigationPage).PushAsync(new PlantDetailsView());
        }

        /// <summary>
        /// Gets the plants from the database and shows them in the view
        /// </summary>
        /// <returns></returns>
        public async Task GetPlants()
        {
            try
            {
                // call the database to get the plants
                var res = await _databaseService.GetAsync<Plant>();

                // if the response is successfull
                if (res.IsSuccessStatusCode)
                {
                    // put the plants into a collection
                    var plants = await res.ContentToCollectionAsync<Plant>();

                    // clear the old list of plants
                    PlantViewModels.Clear();

                    // insert the plants in the
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
