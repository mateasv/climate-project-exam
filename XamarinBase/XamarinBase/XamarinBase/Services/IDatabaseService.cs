using System.Net.Http;
using System.Threading.Tasks;

namespace XamarinBase.Services
{
    public interface IDatabaseService
    {
        string APIUrl { get; set; }
        HttpClient HttpClient { get; set; }

        void Build();
        Task<HttpResponseMessage> DeleteAsync(string url, int id);
        Task<HttpResponseMessage> DeleteAsync<T>(int id);
        Task<HttpResponseMessage> GetAsync(string url);
        Task<HttpResponseMessage> GetAsync<T>();
        Task<HttpResponseMessage> GetAsync<T>(int id);
        Task<HttpResponseMessage> GetAsync<T>(string endpoint, int id);
        Task<HttpResponseMessage> PostAsync(string url, object obj);
        Task<HttpResponseMessage> PostAsync<T>(T obj);
        Task<HttpResponseMessage> PutAsync(string url, object obj);
        Task<HttpResponseMessage> PutAsync<T>(int id, T obj);
    }
}