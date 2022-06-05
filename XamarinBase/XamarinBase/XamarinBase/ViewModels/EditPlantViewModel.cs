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

                if(value != null && value.PlantType.PlantTypeId != 0)
                {
                    PlantViewModel.Plant.PlantTypeId = SelectedPlantTypeViewModel.PlantType.PlantTypeId;
                }
                else
                {
                    PlantViewModel.Plant.PlantTypeId = null;
                }
                OnPropertyChanged();
            }
        }


        public ObservableCollection<PlantTypeViewModel> PlantTypeViewModels
        {
            get { return _plantTypeViewModels; }
            set { _plantTypeViewModels = value; OnPropertyChanged(); }
        }

        public PlantViewModel PlantViewModel
        {
            get { return _plantViewModel; }
            set 
            { 
                _plantViewModel = value; 

                SelectedPlantTypeViewModel = PlantTypeViewModels.FirstOrDefault(ptvm => ptvm.PlantType.PlantTypeId == PlantViewModel.Plant.PlantTypeId);

                OnPropertyChanged(); 
            }
        }

        public ICommand GetPlantTypesCmd { get; set; }
        public ICommand TakePhotoCmd { get; set; }

        

        
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
                var res = await _databaseService.GetAsync<PlantType>();

                if (!res.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert("Alert", $"HTTP error: {res.StatusCode} Error getting plant types", "Ok", "Cancel");
                    await (Application.Current.MainPage as NavigationPage).PopAsync();
                    return;
                }

                var plantTypes = await res.ContentToCollectionAsync<PlantType>();
                plantTypes.ToList().ForEach(pt => PlantTypeViewModels.Add(new PlantTypeViewModel { PlantType = pt }));

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", $"HTTP error in getting plant types: {ex.Message}", "Ok", "Cancel");
            }
        }

        public void Reset()
        {
            PlantViewModel = new PlantViewModel() { Plant = new Plant() };
            SelectedPlantTypeViewModel = new PlantTypeViewModel() { PlantType = new PlantType() };
        }

    }
}
