using Server.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using XamarinBase.Exstensions;
using XamarinBase.Services;

namespace XamarinBase.ViewModels
{
    public class EditPlantViewModel : BaseViewModel
    {
        private readonly IDatabaseService _databaseService;

        private PlantViewModel _plantViewModel;
        private ObservableCollection<PlantTypeViewModel> _plantTypeViewModels;
        private PlantTypeViewModel _selectedPlantTypeViewModel;
        private DateTime _minDate;
        private DateTime _maxDate;
        private DateTime _selectedDate;

        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set { _selectedDate = value; OnPropertyChanged(); }
        }


        public DateTime MaxDate
        {
            get { return _maxDate; }
            set { _maxDate = value; OnPropertyChanged(); }
        }


        public DateTime MinDate
        {
            get { return _minDate; }
            set { _minDate = value; OnPropertyChanged(); }
        }




        public PlantTypeViewModel SelectedPlantTypeViewModel
        {
            get { return _selectedPlantTypeViewModel; }
            set 
            { 

                _selectedPlantTypeViewModel = value;

                if(SelectedPlantTypeViewModel != null)
                {
                    PlantViewModel.Plant.PlantTypeId = SelectedPlantTypeViewModel.PlantType.PlantTypeId;
                }
            }
        }


        public ObservableCollection<PlantTypeViewModel> PlantTypeViewModels
        {
            get { return _plantTypeViewModels; }
            set { _plantTypeViewModels = value; }
        }

        public ICommand GetPlantTypesCmd { get; set; }
        public ICommand TakePhotoCmd { get; set; }

        public PlantViewModel PlantViewModel
        {
            get { return _plantViewModel; }
            set { _plantViewModel = value; OnPropertyChanged(); }
        }

        
        public EditPlantViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            MinDate = new DateTime(2022,1,1);
            MaxDate = new DateTime(DateTime.Now.AddYears(4).Ticks);

            PlantTypeViewModels = new ObservableCollection<PlantTypeViewModel>();

            GetPlantTypesCmd = new Command(async () => await GetPlantTypes());
            TakePhotoCmd = new Command(async () => await TakePhoto());
        }

        public async Task TakePhoto()
        {
            var photo = await MediaPicker.CapturePhotoAsync();

            if (photo is null) return;

            using (var stream = await photo.OpenReadAsync())
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);

                    PlantViewModel.Plant.Image = ms.ToArray();
                    PlantViewModel.Image = ImageSource.FromStream(() => new MemoryStream(PlantViewModel.Plant.Image));
                }
            }

            PlantViewModel.PhotoPath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
        }

        public async Task GetPlantTypes()
        {
            if (PlantTypeViewModels.Count != 0) return;

            try
            {
                var response = await _databaseService.GetAsync<PlantType>();

                if (response.IsSuccessStatusCode)
                {
                    var plantTypes = await response.ContentToCollectionAsync<PlantType>();

                    plantTypes.ToList().ForEach(pt => PlantTypeViewModels.Add(new PlantTypeViewModel { PlantType = pt }));
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Alert", $"HTTP error: {response.StatusCode}", "Cancel", "ok");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", $"HTTP error: {ex.Message}", "Cancel", "ok");
            }
        }

        public void Reset()
        {
            PlantViewModel = new PlantViewModel() { Plant = new Plant() };
            SelectedPlantTypeViewModel = null;
        }

    }
}
