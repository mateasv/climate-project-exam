using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinBase.Views;

namespace XamarinBase.ViewModels
{
    public class PlantDetailsViewModel : BaseViewModel
    {
        private ContentView _currentContentView;

        public ContentView CurrentContentView
        {
            get { return _currentContentView; }
            set { _currentContentView = value; OnPropertyChanged(); }
        }

        public ICommand PlantDetailsCmd { get; set; }
        public ICommand DataloggerDetailsCmd { get; set; }

        public PlantDetailsViewModel()
        {
            PlantDetailsCmd = new Command(async () => await PlantDetails());
            DataloggerDetailsCmd = new Command(async () => await DataloggerDetails());
        }

        public async Task PlantDetails()
        {
            CurrentContentView = new EditPlantView();

            var editPlantViewModel = App.GetViewModel<EditPlantViewModel>() as EditPlantViewModel;
            editPlantViewModel.Reset();
            await editPlantViewModel.GetPlantTypes();
        }

        public async Task DataloggerDetails()
        {
            
        }

    }
}
