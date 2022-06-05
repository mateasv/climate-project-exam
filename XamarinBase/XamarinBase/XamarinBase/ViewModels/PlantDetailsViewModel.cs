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

namespace XamarinBase.ViewModels
{
    public class PlantDetailsViewModel : BaseViewModel
    {
        private readonly IDatabaseService _databaseService;
        private readonly EditDataloggerViewModel _editDataloggerViewModel;
        private readonly EditPlantViewModel _editPlantViewModel;

        private ContentView _currentContentView;

        public ContentView CurrentContentView
        {
            get { return _currentContentView; }
            set { _currentContentView = value; OnPropertyChanged(); }
        }

        public ICommand PlantDetailsCmd { get; set; }
        public ICommand DataloggerDetailsCmd { get; set; }
        public ICommand ConfirmCmd { get; set; }


        public PlantDetailsViewModel(
            IDatabaseService databaseService, 
            EditDataloggerViewModel editDataloggerViewModel, 
            EditPlantViewModel editPlantViewModel)
        {
            _databaseService = databaseService;
            _editDataloggerViewModel = editDataloggerViewModel;
            _editPlantViewModel = editPlantViewModel;

            PlantDetailsCmd = new Command(async () => await PlantDetails());
            DataloggerDetailsCmd = new Command(async () => await DataloggerDetails());
            ConfirmCmd = new Command(async () => await Confirm());
        }

        public async Task PlantDetails()
        {
            CurrentContentView = new EditPlantView();
        }

        public async Task DataloggerDetails()
        {
            CurrentContentView = new EditDataloggerView();
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
            bool doPair = datalogger.DataloggerId == 0 ? false : true;

            if (!doPair)
            {
                if (!await CreatePlant(plant))
                {
                    return false;
                }
                return true;
            }

            Plant pair;
            var res = await _databaseService.GetAsync<Plant>("datalogger", datalogger.DataloggerId);

            if (!res.IsSuccessStatusCode)
            {
                plant.DataloggerId = datalogger.DataloggerId;

                if (!await CreatePlant(plant))
                {
                    return false;
                }
                return true;
            }

            pair = await res.ContentToObjectAsync<Plant>();

            bool overwritePair = await Application.Current.MainPage.DisplayAlert("Alert", $"This datalogger is already paired. Overwrite existing pair?", "Yes", "No");

            if (overwritePair)
            {
                if (!await OverwritePair(pair))
                {
                    return false;
                }

                plant.DataloggerId = datalogger.DataloggerId;
            }

            if (!await CreatePlant(plant))
            {
                return false;
            }

            return true;
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
        
        private async Task<bool> Edit(Plant plant, Datalogger datalogger)
        {



            return true;
        }


    }
}
