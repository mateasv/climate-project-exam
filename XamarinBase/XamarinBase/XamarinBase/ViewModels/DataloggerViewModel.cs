using Server.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinBase.ViewModels
{
    public class DataloggerViewModel : BaseViewModel
    {
        private int _dataloggerId;

        public int DataloggerId
        {
            get { return _dataloggerId; }
            set { _dataloggerId = value; OnPropertyChanged(); }
        }


        public Datalogger Datalogger { get; set; }


    }
}
