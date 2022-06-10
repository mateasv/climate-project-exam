using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace XamarinBase.Exstensions
{
    /// <summary>
    /// Extension class of the HttpResponseMessage class. Adds helper methods used for
    /// deserializing json to objects.
    /// </summary>
    public static class HttpResponseMessageExtension
    {
        /// <summary>
        /// Deserializes a an HttpResponseMessage's content to a collection of type T
        /// </summary>
        /// <typeparam name="T">type of object the collection holds</typeparam>
        /// <param name="hrm"></param>
        /// <returns>collection of type T</returns>
        public static async Task<ICollection<T>> ContentToCollectionAsync<T>(this HttpResponseMessage hrm)
        {
            return JsonConvert.DeserializeObject<ICollection<T>>(await hrm.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Deserializes a HttpResponseMessage's content to an object of type T
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="hrm"></param>
        /// <returns>object of type T</returns>
        public static async Task<T> ContentToObjectAsync<T>(this HttpResponseMessage hrm)
        {
            return JsonConvert.DeserializeObject<T>(await hrm.Content.ReadAsStringAsync());
        }
    }
}
