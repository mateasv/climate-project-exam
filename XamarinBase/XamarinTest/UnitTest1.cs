using Xunit;
using XamarinBase.Exstensions;
using System.Net.Http;
using Server.Models;
using System.Threading.Tasks;
using XamarinBase.Services;

namespace XamarinTest
{
    public class UnitTest1
    {

        [Fact]
        public async Task CreateChart_should_return_nonempty_collection()
        {
            //arrange

            var cs = new ChartService();
            var hrm = new HttpResponseMessage
            {
                Content = new StringContent("[ { \"measurementId\": 1, \"dataloggerId\": 1, \"plantId\": 1, \"soilHumidity\": 22.22, \"airHumidity\": 33.33, \"airTemerature\": 44.44, \"soilIsDry\": true, \"datalogger\": null, \"plant\": null }, { \"measurementId\": 2, \"dataloggerId\": 1, \"plantId\": 1, \"soilHumidity\": 123, \"airHumidity\": 44, \"airTemerature\": 22, \"soilIsDry\": true, \"datalogger\": null, \"plant\": null } ]")
            };

            var list = await hrm.ContentToCollectionAsync<Measurement>();

            // act

            var chart = cs.CreateChart<Measurement>(
                list,
                value: (m) => m.AirTemerature, 
                label: (m) => m.MeasurementId.ToString()
            );

            // assert

            Assert.NotEmpty(chart.Entries);

        }
        

        [Fact]
        public async Task HttpResponseMessage_ContentToCollection_should_return_Collection()
        {
            // arrange

            var hrm = new HttpResponseMessage
            {
                Content = new StringContent("[ { \"measurementId\": 1, \"dataloggerId\": 1, \"plantId\": 1, \"soilHumidity\": 22.22, \"airHumidity\": 33.33, \"airTemerature\": 44.44, \"soilIsDry\": true, \"datalogger\": null, \"plant\": null }, { \"measurementId\": 2, \"dataloggerId\": 1, \"plantId\": 1, \"soilHumidity\": 123, \"airHumidity\": 44, \"airTemerature\": 22, \"soilIsDry\": true, \"datalogger\": null, \"plant\": null } ]")
            };


            //act

            var l = await hrm.ContentToCollectionAsync<Measurement>();

            // assert

            Assert.NotEmpty(l);
        }
    }
}