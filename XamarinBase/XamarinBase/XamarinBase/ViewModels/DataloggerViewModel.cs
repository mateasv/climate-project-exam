using Server.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinBase.ViewModels
{
    public class DataloggerViewModel : BaseViewModel
    {
        private Datalogger _datalogger;

        public Datalogger Datalogger
        {
            get { return _datalogger; }
            set { _datalogger = value; OnPropertyChanged(); }
        }



    }
}
