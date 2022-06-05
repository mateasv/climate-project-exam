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

            (Application.Current.MainPage as NavigationPage).Popped += PlantDetailsViewModel_Popped;
        }

        public async Task PlantDetails()
        {
            CurrentContentView = new EditPlantView();
        }

        public async Task DataloggerDetails()
        {
            CurrentContentView = new EditDataloggerView();
        }

        public async Task Chart()
        {
            var plant = _editPlantViewModel.PlantViewModel.Plant;

            if(plant.PlantId == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", $"No chart data", "Ok", "Cancel");
                return;
            }

            var res = await _databaseService.GetAsync<Measurement>("plant",plant.PlantId);
            if (!res.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", $"No chart data", "Ok", "Cancel");
                return;
            }

            var measurements = await res.ContentToCollectionAsync<Measurement>();
            _chartViewModel.Measurements = new ObservableCollection<Measurement>(measurements);

            await _chartViewModel.GenerateChart();

            CurrentContentView = new ChartView();
        }

        public async Task Confirm()
        {
            var datalogger = _editDataloggerViewModel.DataloggerViewModel.Datalogger;
            var plant = _editPlantViewModel.PlantViewModel.Plant;

            try
            {
                if(plant.PlantId == 0)
                {
                    if (await Create(plant, datalogger))
                    {
                        await (Application.Current.MainPage as NavigationPage).PopAsync();
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Alert", $"{ErrorMessage}", "Ok", "Cancel");
                    }
                }
                else
                {
                    if (await Edit(plant, datalogger))
                    {
                        IsPlantChanged = true;
                        await (Application.Current.MainPage as NavigationPage).PopAsync();
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Alert", $"{ErrorMessage}", "Ok", "Cancel");
                    }

                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", $"HTTP error in confirm new plant and datalogger: {ex.Message}", "Ok", "Cancel");
            }

        }

        
        private async Task<bool> Create(Plant plant, Datalogger datalogger)
        {
            return await CreateOrEdit(plant, datalogger, CreatePlant);
        }

        
        private async Task<bool> Edit(Plant plant, Datalogger datalogger)
        {
            return await CreateOrEdit(plant, datalogger, EditPlant);
        }



        private async Task<bool> CreateOrEdit(Plant plant, Datalogger datalogger, Func<Plant, Task<bool>> crud)
        {
            var doPair = datalogger.DataloggerId != 0;

            if (!doPair)
            {
                return await crud(plant);
            }


            var res = await _databaseService.GetAsync<Plant>("datalogger", datalogger.DataloggerId);

            if (!res.IsSuccessStatusCode)
            {
                plant.DataloggerId = datalogger.DataloggerId;

                return await crud(plant);
            }


            var overwritePair = await Application.Current.MainPage.DisplayAlert("Alert", $"This datalogger is already paired. Overwrite existing pair?", "Yes", "No");

            if (overwritePair)
            {
                var pair = await res.ContentToObjectAsync<Plant>();

                if (!await OverwritePair(pair))
                {
                    return false;
                }

                plant.DataloggerId = datalogger.DataloggerId;
            }

            return await crud(plant);
        }



        private async Task<bool> OverwritePair(Plant plant)
        {
            plant.DataloggerId = null;

            var res = await _databaseService.PutAsync<Plant>(plant.PlantId, plant);
            if (!res.IsSuccessStatusCode)
            {
                ErrorMessage = $"Http Error: {res.ReasonPhrase} Error unpairing existing plant";
                return false;
            }

            return true;
        }
        private async Task<bool> CreatePlant(Plant plant)
        {
            var res = await _databaseService.PostAsync<Plant>(plant);
            if (!res.IsSuccessStatusCode)
            {
                ErrorMessage = $"Http Error: {res.ReasonPhrase} Error creating plant";
                return false;
            }

            return true;
        }

        private async Task<bool> EditPlant(Plant plant)
        {
            var res = await _databaseService.PutAsync<Plant>(plant.PlantId,plant);
            if (!res.IsSuccessStatusCode)
            {
                ErrorMessage = $"Http Error: {res.ReasonPhrase} Error editing plant";
                return false;
            }

            return true;
        }

        private async void PlantDetailsViewModel_Popped(object sender, NavigationEventArgs e)
        {
            if (IsPlantChanged)
            {
                IsPlantChanged = false;
                return;
            }

            var currentPlant = _editPlantViewModel.PlantViewModel.Plant;

            if (currentPlant.PlantId == 0)
            {
                return;
            }

            var res = await _databaseService.GetAsync<Plant>(currentPlant.PlantId);

            if (!res.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", $"Http Error: {res.ReasonPhrase} Error restoring unmodified plant", "Ok", "Cancel");
                return;
            }

            var restoredPlant = await res.ContentToObjectAsync<Plant>();

            _editPlantViewModel.PlantViewModel.Plant = restoredPlant;
        }
    }
}
