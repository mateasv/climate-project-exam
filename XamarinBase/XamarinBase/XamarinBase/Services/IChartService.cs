//https://github.com/microcharts-dotnet/Microcharts

using Microcharts;
using System;
using System.Collections.Generic;

namespace XamarinBase.Services
{
    public interface IChartService
    {
        Chart CreateChart<T>(IEnumerable<T> data, Func<T, float> value, Func<T, string> label);
    }
}