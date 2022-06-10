using Android.Net;
using Javax.Net.Ssl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Android.Net;
using Xamarin.Forms;
using XamarinBase.Exstensions;

namespace XamarinBase.Services
{

    /// <summary>
    /// Database service class responsible for basic crud operations and setup of the
    /// HttpClient
    /// </summary>
    public class DatabaseService : IDatabaseService
    {
        // http client handler used, if we are running on an android phone with Xamarin.forms
        private IHTTPClientHandlerCreationService _httpHandlerCreationService;

        private string _APIUrl;
        public string APIUrl
        {
            get { return _APIUrl; }
            set { _APIUrl = value; }
        }

        public HttpClient HttpClient { get; set; }

        public DatabaseService(IHTTPClientHandlerCreationService clienthandler = null)
        {
            _httpHandlerCreationService = clienthandler;

            Build();
        }
        public void Build()
        {
            /* To enable timeout, link: 
             * https://tousu.in/qa/?qa=550162/
             * https://stackoverflow.com/questions/28629989/ignore-ssl-certificate-errors-in-xamarin-forms-pcl/54318410#54318410
             */

            // if the program is not using Xamarin.forms
            if (_httpHandlerCreationService == null)
            {
                // setup the httpclient handler with ssl bypass for testing the program
                var httpClientHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                };

                HttpClient = new HttpClient(httpClientHandler);

                APIUrl = "https://localhost:7189/api";
            }
            else
            {
                // setup the httpclient with an Android specific implementation of the httpclient handler
                HttpClient = new HttpClient(_httpHandlerCreationService.GetInsecureHandler());

                APIUrl = "https://10.0.2.2:7189/api";
            }

            // set timeout to 15 seconds
            HttpClient.Timeout = TimeSpan.FromSeconds(15);
        }

        /// <summary>
        /// Sends a http get request that fetches all records
        /// </summary>
        /// <typeparam name="T">type of object to get from the server</typeparam>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync<T>()
        {
            var endPointUrl = $"{APIUrl}/{typeof(T).Name}s";

            var response = await GetAsync(endPointUrl);

            return response;
        }

        /// <summary>
        /// Http get request for a single object
        /// </summary>
        /// <typeparam name="T">type of object to get from the server</typeparam>
        /// <param name="id">id of the object</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync<T>(int id)
        {
            var endPointUrl = $"{APIUrl}/{typeof(T).Name}s/{id}";

            var response = await GetAsync(endPointUrl);

            return response;
        }

        /// <summary>
        /// Http request for a single object, but with a different endpoint
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="endpoint">endpoint</param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync<T>(string endpoint, int id)
        {
            var endPointUrl = $"{APIUrl}/{typeof(T).Name}s/{endpoint}/{id}";

            var response = await GetAsync(endPointUrl);

            return response;
        }

        /// <summary>
        /// Http request with a specified url 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            var response = await HttpClient.GetAsync(url);

            return response;
        }

        /// <summary>
        /// Post request
        /// </summary>
        /// <typeparam name="T">object type to be posted</typeparam>
        /// <param name="obj">object to post</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync<T>(T obj)
        {
            var endPointUrl = $"{APIUrl}/{typeof(T).Name}s";

            var response = await PostAsync(endPointUrl, obj);

            return response;
        }


        /// <summary>
        /// Post request with userdefined url
        /// </summary>
        /// <param name="url">Url for the endpoint</param>
        /// <param name="obj">object to post</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync(string url, object obj)
        {
            var httpContent = CreateHttpContent(obj);

            var response = await HttpClient.PostAsync(url, httpContent);

            return response;
        }

        /// <summary>
        /// Put request to modify an object
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="id">id of object</param>
        /// <param name="obj">the updated object</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutAsync<T>(int id, T obj)
        {
            var endPointUrl = $"{APIUrl}/{typeof(T).Name}s/{id}";

            var response = await PutAsync(endPointUrl, obj);

            return response;
        }

        /// <summary>
        /// Put request, update an object but different endpoint
        /// </summary>
        /// <param name="endpoint">url path</param>
        /// <param name="id">object id</param>
        /// <param name="obj">updated object</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutAsync(string endpoint, int id, object obj)
        {
            var endPointUrl = $"{APIUrl}/{endpoint}/{id}";

            var response = await PutAsync(endPointUrl, obj);

            return response;
        }

        /// <summary>
        /// Put request, update an object
        /// </summary>
        /// <param name="url">path for the endpoint</param>
        /// <param name="obj">updated object</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutAsync(string url, object obj)
        {
            var httpContent = CreateHttpContent(obj);

            var response = await HttpClient.PutAsync(url, httpContent);

            return response;
        }


        /// <summary>
        /// Delete request
        /// </summary>
        /// <typeparam name="T">object type to delete</typeparam>
        /// <param name="id">id of the object to delete</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteAsync<T>(int id)
        {
            var endPointUrl = $"{APIUrl}/{typeof(T).Name}s/{id}";

            var response = await DeleteAsync(endPointUrl, id);

            return response;
        }

        /// <summary>
        /// Delete request
        /// </summary>
        /// <param name="url">path for the endpoint</param>
        /// <param name="id">id of the object</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteAsync(string url, int id)
        {
            var response = await HttpClient.DeleteAsync(url);

            return response;
        }








        private HttpContent CreateHttpContent(object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
        }
    }
}
