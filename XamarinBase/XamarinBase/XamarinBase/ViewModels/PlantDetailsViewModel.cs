using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinBase.ViewModels
{
    public class PlantDetailsViewModel : BaseViewModel
    {
        private PlantViewModel _selectedPlanViewModel;

        public PlantViewModel SelectedPlantViewModel
        {
            get { return _selectedPlanViewModel; }
            set { _selectedPlanViewModel = value; }
        }


        public PlantDetailsViewModel()
        {

        }
    }
}
