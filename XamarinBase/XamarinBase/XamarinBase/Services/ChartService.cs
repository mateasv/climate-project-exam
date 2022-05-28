//https://github.com/microcharts-dotnet/Microcharts

using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace XamarinBase.Services
{
    public class ChartService
    {
        public ChartService()
        {

        }

        public Chart CreateChart<T>(IEnumerable<T> data, Func<T,float> value, Func<T,string> label) {
            var entries = new List<ChartEntry>();

            data.ToList().ForEach(item => entries.Add(new ChartEntry(value(item))
            {
                Label = label(item),
                ValueLabel = value(item).ToString()
            }));
            
            var chart = new LineChart { Entries = entries };
            chart.LabelOrientation = Orientation.Horizontal;
            chart.ValueLabelOrientation = Orientation.Horizontal;

            return chart;
        }
    }
}
