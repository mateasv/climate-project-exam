using Microcharts;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinBase.Services;

namespace XamarinBase.ViewModels
{
    public class ChartViewModel : BaseViewModel
    {
        private readonly IChartService _chartService;


        private Chart _airHumidityChart;
        private Chart _airTemperatureChart;
        private ObservableCollection<Measurement> _measurements;

        public ObservableCollection<Measurement> Measurements
        {
            get { return _measurements; }
            set { _measurements = value; OnPropertyChanged(); }
        }


        public Chart AirTemperatureChart
        {
            get { return _airTemperatureChart; }
            set { _airTemperatureChart = value; OnPropertyChanged(); }
        }



        public Chart AirHumidityChart
        {
            get { return _airHumidityChart; }
            set { _airHumidityChart = value; OnPropertyChanged(); }
        }

        public ICommand GenerateChartCmd { get; set; }


        public ChartViewModel(IChartService chartService)
        {
            _chartService = chartService;

            GenerateChartCmd = new Command(async () => await GenerateChart());


            Measurements = new ObservableCollection<Measurement>();
        }

        public async Task GenerateChart()
        {
            AirHumidityChart = _chartService.CreateChart(Measurements,m => m.AirHumidity, m => m.MeasurementDate.ToString());
            AirTemperatureChart = _chartService.CreateChart(Measurements,m => m.AirTemperature, m => m.MeasurementDate.ToString());
        }

    }
}
