using Xunit;
using XamarinBase;
using XamarinBase.Exstensions;
using System.Net.Http;
using Moq;
using Server.Models;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Http.Json;
using XamarinBase.Services;
using Microsoft.Net.Http.Client;
using System.Linq;

namespace XamarinTest
{
    public class UnitTest1
    {

        
        

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