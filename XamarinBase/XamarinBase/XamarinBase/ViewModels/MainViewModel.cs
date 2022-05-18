using System;
using System.Collections.Generic;
using System.Text;
using XamarinBase.Models;
using XamarinBase.Services;

namespace XamarinBase.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private DataModel _dataModel;

        public DataModel DataModel
        {
            get { return _dataModel; }
            set { _dataModel = value; }
        }



        public MainViewModel(IAppInfoService appInfoService, IDataService dataService)
        {
            DataModel = new DataModel { Data = "This is Data" };
        }
    }
}
