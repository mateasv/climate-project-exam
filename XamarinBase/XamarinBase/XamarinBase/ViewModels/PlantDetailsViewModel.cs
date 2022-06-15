using Server.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinBase.Services;
using XamarinBase.Views;
using XamarinBase.Exstensions;
using System.Net.Http;
using System.Collections.ObjectModel;

namespace XamarinBase.ViewModels
{
    /// <summary>
    /// View model of the PlantDetailsView. Used when adding or editting a plant, 
    /// viewing charts and pairing.
    /// </summary>
    public class PlantDetailsViewModel : BaseViewModel
    {
        private readonly IDatabaseService _databaseService;
        private readonly EditDataloggerViewModel _editDataloggerViewModel;
        private readonly EditPlantViewModel _editPlantViewModel;
        private readonly ChartViewModel _chartViewModel;
        


        private ContentView _currentContentView;
        private bool _isPlantChanged;

        public bool IsPlantChanged
        {
            get { return _isPlantChanged; }
            set { _isPlantChanged = value; }
        }



        public ContentView CurrentContentView
        {
            get { return _currentContentView; }
            set { _currentContentView = value; OnPropertyChanged(); }
        }

        public ICommand PlantDetailsCmd { get; set; }
        public ICommand DataloggerDetailsCmd { get; set; }
        public ICommand ConfirmCmd { get; set; }
        public ICommand ChartCmd { get; set; }
        

        public PlantDetailsViewModel(
            IDatabaseService databaseService,
            EditDataloggerViewModel editDataloggerViewModel, 
            EditPlantViewModel editPlantViewModel,
            ChartViewModel chartViewModel)
        {
            _databaseService = databaseService;
            _editDataloggerViewModel = editDataloggerViewModel;
            _editPlantViewModel = editPlantViewModel;
            _chartViewModel = chartViewModel;


            PlantDetailsCmd = new Command(async () => await PlantDetails());
            DataloggerDetailsCmd = new Command(async () => await DataloggerDetails());
            ConfirmCmd = new Command(async () => await Confirm());
            ChartCmd = new Command(async () => await Chart());

            // Restore the old plant values if the edit view is canceled
            (Application.Current.MainPage as NavigationPage).Popped += RestoreOldPlant;
        }

        public async Task PlantDetails()
        {
            CurrentContentView = new EditPlantView();
        }

        public async Task DataloggerDetails()
        {
            CurrentContentView = new EditDataloggerView();
        }

        /// <summary>
        /// Gets the measurements for a plant, generate chart and change current view
        /// </summary>
        /// <returns></returns>
        public async Task Chart()
        {
            // get the current plant
            var plant = _editPlantViewModel.PlantViewModel.Plant;

            // if plant does not have a valid id / is not inserted in the database, then return
            if(plant.PlantId == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", $"No chart data", "Ok", "Cancel");
                return;
            }

            // get the measurements for this plant by plant id
            var res = await _databaseService.GetAsync<Measurement>("plant",plant.PlantId);
            if (!res.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", $"No chart data", "Ok", "Cancel");
                return;
            }

            // get the measurements as a collection
            var measurements = await res.ContentToCollectionAsync<Measurement>();
            _chartViewModel.Measurements = new ObservableCollection<Measurement>(measurements);

            // generate chart
            await _chartViewModel.GenerateChart();

            // change view
            CurrentContentView = new ChartView();
        }

        /// <summary>
        /// Confirms a create or edit of a plant
        /// </summary>
        /// <returns></returns>
        public async Task Confirm()
        {
            // get current datalogger and plant
            var datalogger = _editDataloggerViewModel.DataloggerViewModel.Datalogger;
            var plant = _editPlantViewModel.PlantViewModel.Plant;

            try
            {
                // if plant is not inserted in the database
                if(plant.PlantId == 0)
                {
                    // if create is success
                    if (await Create(plant, datalogger))
                    {
                        // pop current page, navigate back to PlantsView
                        await (Application.Current.MainPage as NavigationPage).PopAsync();
                    }
                    else
                    {
                        // unsuccessfull creation of plant
                        await Application.Current.MainPage.DisplayAlert("Alert", $"{ErrorMessage}", "Ok", "Cancel");
                    }
                }
                else
                {
                    // if edit is success
                    if (await Edit(plant, datalogger))
                    {
                        // plant is now changed
                        IsPlantChanged = true;

                        // return to PlantsView
                        await (Application.Current.MainPage as NavigationPage).PopAsync();
                    }
                    else
                    {
                        // error
                        await Application.Current.MainPage.DisplayAlert("Alert", $"{ErrorMessage}", "Ok", "Cancel");
                    }

                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", $"HTTP error in confirm new plant and datalogger: {ex.Message}", "Ok", "Cancel");
            }

        }

        /// <summary>
        /// Create a plant
        /// </summary>
        /// <param name="plant"></param>
        /// <param name="datalogger"></param>
        /// <returns>true if success</returns>
        private async Task<bool> Create(Plant plant, Datalogger datalogger)
        {
            return await CreateOrEdit(plant, datalogger, CreatePlant);
        }

        
        /// <summary>
        /// Edit a plant
        /// </summary>
        /// <param name="plant"></param>
        /// <param name="datalogger"></param>
        /// <returns>true if success</returns>
        private async Task<bool> Edit(Plant plant, Datalogger datalogger)
        {
            return await CreateOrEdit(plant, datalogger, EditPlant);
        }


        /// <summary>
        /// Creates or edits a plant. Checks if pairing or unpairing is needed.
        /// </summary>
        /// <param name="plant"></param>
        /// <param name="datalogger"></param>
        /// <param name="crud"></param>
        /// <returns></returns>
        private async Task<bool> CreateOrEdit(Plant plant, Datalogger datalogger, Func<Plant, Task<bool>> crud)
        {
            var doPair = datalogger.DataloggerId != 0;

            // if datalogger does not have an id, then we are creating or editing a plant without pairing
            if (!doPair)
            {
                // create plant and return
                return await crud(plant);
            }

            // check if the datalogger is already paired
            var res = await _databaseService.GetAsync<Plant>("dataloggerplant", datalogger.DataloggerId);

            // if this datalogger is not paired then create and pair the plant
            if (!res.IsSuccessStatusCode)
            {
                plant.DataloggerId = datalogger.DataloggerId;

                return await crud(plant);
            }


            // if the datalogger is paired, then ask the user if they want to overwrite the existing pair
            var removePair = await Application.Current.MainPage.DisplayAlert("Alert", $"This datalogger is already paired. Remove existing pair and make new pair?", "Yes", "No");

            // if user want to overwrite existing pair
            if (removePair)
            {
                // get the plant and datalogger pair as an object
                var pair = await res.ContentToObjectAsync<Plant>();

                // remove the pair
                if (!await RemovePair(pair))
                {
                    return false;
                }

                // assign the new pair
                plant.DataloggerId = datalogger.DataloggerId;
            }

            // create or edit plant
            return await crud(plant);
        }


        /// <summary>
        /// Removes a pair
        /// </summary>
        /// <param name="plant">plant with a paired datalogger</param>
        /// <returns>true if pairing is successfull</returns>
        private async Task<bool> RemovePair(Plant plant)
        {
            // calls the remove pair endpoint in the server
            var res = await _databaseService.PutAsync("dataloggerpair/removepair",plant.PlantId, plant);
            
            // unpairing is unsuccessfull
            if (!res.IsSuccessStatusCode)
            {
                ErrorMessage = $"Http Error: {res.ReasonPhrase} Error unpairing existing plant";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a plant with a pair
        /// </summary>
        /// <param name="plant"></param>
        /// <returns>true if successfull</returns>
        private async Task<bool> CreatePlant(Plant plant)
        {
            var res = await _databaseService.PostAsync($"{_databaseService.APIUrl}/dataloggerpair",plant);
            if (!res.IsSuccessStatusCode)
            {
                ErrorMessage = $"Http Error: {res.ReasonPhrase} Error creating plant";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Edits a plant with a pair
        /// </summary>
        /// <param name="plant"></param>
        /// <returns></returns>
        private async Task<bool> EditPlant(Plant plant)
        {
            var res = await _databaseService.PutAsync("dataloggerpair", plant.PlantId,plant);
            if (!res.IsSuccessStatusCode)
            {
                ErrorMessage = $"Http Error: {res.ReasonPhrase} Error editing plant";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Handler for restoring the old plant if the edit operation is cancelled
        /// by the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RestoreOldPlant(object sender, NavigationEventArgs e)
        {
            // if plant is changed and confirmed then return
            if (IsPlantChanged)
            {
                IsPlantChanged = false;
                return;
            }

            // if a plant is created and the id is known, then return
            var currentPlant = _editPlantViewModel.PlantViewModel.Plant;

            if (currentPlant.PlantId == 0)
            {
                return;
            }

            // call the database for the old plant
            var res = await _databaseService.GetAsync<Plant>(currentPlant.PlantId);

            // if error then return
            if (!res.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", $"Http Error: {res.ReasonPhrase} Error restoring unmodified plant", "Ok", "Cancel");
                return;
            }

            // get the old plant values
            var restoredPlant = await res.ContentToObjectAsync<Plant>();

            // restore the old plant
            _editPlantViewModel.PlantViewModel.Plant = restoredPlant;
        }
    }
}
