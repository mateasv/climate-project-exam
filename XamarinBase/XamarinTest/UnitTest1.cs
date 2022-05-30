using Xunit;
using XamarinBase.Exstensions;
using System.Net.Http;
using Server.Models;
using System.Threading.Tasks;
using XamarinBase.Services;
using System.IO;
using System;
using System.Text;
using Xamarin.Forms;

namespace XamarinTest
{
    public class UnitTest1
    {
        [Fact]
        public async Task Dbtest_GetPlant()
        {
            // arrange
            var db = new DatabaseService();

            // act

            var res = await db.GetAsync<Plant>(2);
            var t = await res.ContentToObjectAsync<Plant>();

            // assert

            Assert.NotNull(t);
        }

        [Fact]
        public async Task DbTest()
        {
            // arrange
            var db = new DatabaseService();
            db.APIUrl = "https://localhost:7189/api";
            db.Build();

            byte[] img;

            using (FileStream fs = File.Open(@"C:\Users\Lenovo\Desktop\xp.jpg", FileMode.Open))
            {
               
                var ms = new MemoryStream();
                fs.CopyTo(ms);
                img = ms.ToArray();
            }

            var tree = new Plant()
            {
                PlantTypeId = 1,
                Price = 22.22F,
                WarrantyStartDate = null,
                Image = img
            };

            // act

            var res = await db.PostAsync<Plant>(tree);
            var t = await res.ContentToObjectAsync<Plant>();

            // assert

            Assert.NotNull(t);
        }



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