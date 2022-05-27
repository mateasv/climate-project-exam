using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XamarinBase.Exstensions;

namespace XamarinBase.Services
{
    public class DatabaseService
    {
        private string _APIUrl;

        public string APIUrl
        {
            get { return _APIUrl; }
            set { _APIUrl = value; }
        }


        public HttpClient HttpClient { get; set; }

        public DatabaseService()
        {
            HttpClient = new HttpClient();
            APIUrl = "https://localhost:7189/api";
        }

        public async Task<HttpResponseMessage> GetAsync<T>()
        {
            var endPointUrl = $"{APIUrl}/{typeof(T).Name}s";

            var response = await GetAsync(endPointUrl);

            return response;
        }

        public async Task<HttpResponseMessage> GetAsync<T>(int id)
        {
            var endPointUrl = $"{APIUrl}/{typeof(T).Name}s/{id}";

            var response = await GetAsync(endPointUrl);

            return response;
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            var response = await HttpClient.GetAsync(url);

            return response;
        }

        public async Task<HttpResponseMessage> PostAsync<T>(T obj)
        {
            var endPointUrl = $"{APIUrl}/{typeof(T).Name}s";
            
            var response = await PostAsync(endPointUrl, obj);

            return response;
        }

        public async Task<HttpResponseMessage> PostAsync(string url, object obj)
        {
            var httpContent = CreateHttpContent(obj);

            var response = await HttpClient.PostAsync(url, httpContent);

            return response;
        }

        public async Task<HttpResponseMessage> PutAsync<T>(int id, T obj)
        {
            var endPointUrl = $"{APIUrl}/{typeof(T).Name}s/{id}";

            var response = await PutAsync(endPointUrl,obj);

            return response;
        }

        public async Task<HttpResponseMessage> PutAsync(string url, object obj)
        {
            var httpContent = CreateHttpContent(obj);

            var response = await HttpClient.PutAsync(url, httpContent);

            return response;
        }

        public async Task<HttpResponseMessage> DeleteAsync<T>(int id)
        {
            var endPointUrl = $"{APIUrl}/{typeof(T).Name}s/{id}";

            var response = await DeleteAsync(endPointUrl,id);

            return response;
        }

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
