using Microcharts;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using XamarinBase.Exstensions;
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

        private Chart _chart;

        public Chart Chart
        {
            get { return _chart; }
            set { _chart = value; }
        }


        public MainViewModel(IAppInfoService appInfoService, IDataService dataService, ChartService chartService)
        {
            DataModel = new DataModel { Data = "This is Data" };


            // chart test
            var list = new List<Measurement>()
            {
                new Measurement
                {
                    MeasurementId = 1,
                    AirTemerature = 11.11F,
                },
                new Measurement
                {
                    MeasurementId = 2,
                    AirTemerature = 21.11F,
                }
            };
            
            var chart = chartService.CreateChart<Measurement>(
                list,
                value: (m) => m.AirTemerature,
                label: (m) => m.MeasurementId.ToString()
            );

            Chart = chart;
        }
    }
}
