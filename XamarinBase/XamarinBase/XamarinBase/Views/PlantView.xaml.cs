using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinBase.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlantView : ContentPage
    {
        public PlantView()
        {
            InitializeComponent();

            // Oldest readings first (1hr interval)
            float[] airTempHistory = { 5, 9, 2, 5, 7, 4, 6, 1, 3, 8, 12, 9 };

            ChartEntry[] entries = new ChartEntry[airTempHistory.Length];

            for (int i = 0; i < airTempHistory.Length; i++)
            {
                ChartEntry entry = new ChartEntry(airTempHistory[i]);
                entry.Label = i == airTempHistory.Length - 1 ? "Now" : $"-{(airTempHistory.Length - 1 - i).ToString()}hr";
                entry.ValueLabel = $"{airTempHistory[i]}°C";
                entry.Color = SKColor.Parse("#2c3e50");
                entries[i] = entry;
            }


            var chart = new LineChart { Entries = entries };

            chart.LabelTextSize = 48;

            plantChart.Chart = chart;
        }
    }
}