using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace XamarinBase.Exstensions
{
    public static class HttpResponseMessageExtension
    {
        public static async Task<ICollection<T>> ContentToCollectionAsync<T>(this HttpResponseMessage hrm)
        {
            return JsonConvert.DeserializeObject<ICollection<T>>(await hrm.Content.ReadAsStringAsync());
        }

        public static async Task<T> ContentToObjectAsync<T>(this HttpResponseMessage hrm)
        {
            return JsonConvert.DeserializeObject<T>(await hrm.Content.ReadAsStringAsync());
        }
    }
}
