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
    /// <summary>
    /// View model for edditing or creating a plant
    /// </summary>
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



        /// <summary>
        /// Two-way binding of the selected plant type in the view
        /// </summary>
        public PlantTypeViewModel SelectedPlantTypeViewModel
        {
            get { return _selectedPlantTypeViewModel; }
            set 
            { 
                _selectedPlantTypeViewModel = value;

                // if the value is not null and the value's PlantTypeId is not 0
                if(value != null && value.PlantType.PlantTypeId != 0)
                {
                    // set the PlantViewModels PlantTypeId to the SelectedPlantTypeViewModels PlantTypeId
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

                // Update the selected plant type in the UI whenever PlantViewModel is changed, ex when we click on an existing plant in the PlatsView
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

        /// <summary>
        /// Photo method. Responsible for taking pictures and transforming them into
        /// a byte array and ImageSource.
        /// </summary>
        /// <returns></returns>
        public async Task TakePhoto()
        {
            // wawait for the user to take a picture
            var photo = await MediaPicker.CapturePhotoAsync();

            // if no picture is taken, then return
            if (photo is null) return;

            // open the photo as a stream
            using (var stream = await photo.OpenReadAsync())
            {
                // create a memory stream
                using (var ms = new MemoryStream())
                {
                    // copy the photo stream into the memory stream
                    stream.CopyTo(ms);

                    // convert the memory stream to a byte array and assign it to the plant models Image property
                    PlantViewModel.Plant.Image = ms.ToArray();

                    // create an ImageSource using the photo memory stream and assign it to the plnt view model's Image property
                    PlantViewModel.Image = ImageSource.FromStream(() => new MemoryStream(PlantViewModel.Plant.Image));
                }
            }

            // get the path of the picture
            PlantViewModel.PhotoPath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
        }

        /// <summary>
        /// Method used for requesting the available plant types from the database
        /// </summary>
        /// <returns></returns>
        public async Task GetPlantTypes()
        {
            // return if we already have requested the plant types
            if (PlantTypeViewModels.Count != 0) return;

            try
            {
                // get plant types
                var res = await _databaseService.GetAsync<PlantType>();

                // if not succes then display error and return
                if (!res.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert("Alert", $"HTTP error: {res.StatusCode} Error getting plant types", "Ok", "Cancel");
                    await (Application.Current.MainPage as NavigationPage).PopAsync();
                    return;
                }

                // convert the content into a collection
                var plantTypes = await res.ContentToCollectionAsync<PlantType>();

                // for each found plant types, create a new PlantTypeViewModel, set the plant type and add the PlantTypeViewModel to the plantTypes collection
                plantTypes.ToList().ForEach(pt => PlantTypeViewModels.Add(new PlantTypeViewModel { PlantType = pt }));

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", $"HTTP error in getting plant types: {ex.Message}", "Ok", "Cancel");
            }
        }

        /// <summary>
        /// Method used for reseting the view model
        /// </summary>
        public void Reset()
        {
            PlantViewModel = new PlantViewModel() { Plant = new Plant() };
            SelectedPlantTypeViewModel = new PlantTypeViewModel() { PlantType = new PlantType() };
        }

    }
}
