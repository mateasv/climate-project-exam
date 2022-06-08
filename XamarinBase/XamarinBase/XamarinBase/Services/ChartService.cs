//https://github.com/microcharts-dotnet/Microcharts

using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace XamarinBase.Services
{
    public class ChartService : IChartService
    {
        public ChartService()
        {

        }

        /// <summary>
        /// Creates a chart
        /// </summary>
        /// <typeparam name="T">the type of the enumerable data collection</typeparam>
        /// <param name="data">enumerable data of the type</param>
        /// <param name="value">function that returns the value to be used on the Y-axis of the chart</param>
        /// <param name="label">function that returns a string used on the X-axis of the chart</param>
        /// <returns>new chart</returns>
        public Chart CreateChart<T>(IEnumerable<T> data, Func<T, float> value, Func<T, string> label)
        {
            // Instantiate a new list
            var entries = new List<ChartEntry>();

            // for each item in the enumerable data add a new entry to the entries list
            data.ToList().ForEach(item => entries.Add(new ChartEntry(value(item))
            {
                Label = label(item), // used on the X-axis
                ValueLabel = value(item).ToString() // used on the Y-axis
            }));

            // instantiate a new chart with the created entries
            var chart = new LineChart { Entries = entries };

            // setup basic formatting of the chart
            chart.LabelTextSize = 30;
            chart.LabelOrientation = Orientation.Horizontal;
            chart.ValueLabelOrientation = Orientation.Horizontal;

            return chart;
        }
    }
}
