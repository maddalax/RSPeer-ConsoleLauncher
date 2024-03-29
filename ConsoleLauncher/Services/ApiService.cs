using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ConsoleLauncher.Models.Responses;
using Newtonsoft.Json;

namespace ConsoleLauncher.Services
{
    public class ApiService : IApiService
    {
        private readonly string _baseUrl = "https://services.rspeer.org/api/";
        private readonly HttpClient _client;
        private readonly IAuthorizationService _authorization;

        public ApiService(IAuthorizationService authorization, IHttpClientFactory factory)
        {
            _authorization = authorization;
            _client = factory.CreateClient("RsPeerApi");
        }

        public async Task<T> Get<T>(string path)
        {
            var content = await GetString(path);
            return string.IsNullOrEmpty(content)
                ? default : JsonConvert.DeserializeObject<T>(content);
        }

        public async Task<Stream> GetStream(string path)
        {
            var session = await _authorization.GetSession();
            var message = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}{path}");
            message.Headers.Add("Authorization", "Bearer " + session);
            var result = await _client.SendAsync(message);
            return await result.Content.ReadAsStreamAsync();
        }

        public async Task<string> GetString(string path)
        {
            var session = await _authorization.GetSession();
            var message = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}{path}");
            message.Headers.Add("Authorization", "Bearer " + session);
            var result = await _client.SendAsync(message);
            var content = await result.Content.ReadAsStringAsync();
            AssertError(result, content);
            return content;
        }

        public async Task<T> Post<T>(string path, object body)
        {
            var session = await _authorization.GetSession();
            var message = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}{path}");
            var serialized = JsonConvert.SerializeObject(body);
            message.Content = new StringContent(serialized, Encoding.Default, "application/json");
            message.Headers.Add("Authorization", "Bearer " + session);
            var result = await _client.SendAsync(message);
            var content = await result.Content.ReadAsStringAsync();
            AssertError(result, content);
            return string.IsNullOrEmpty(content) ? default : JsonConvert.DeserializeObject<T>(content);
        }

        private void AssertError(HttpResponseMessage message, string content)
        {
            if (message.IsSuccessStatusCode)
            {
                return;
            }
            var error = JsonConvert.DeserializeObject<ApiErrorResponse>(content);
            throw new Exception(error.Error ?? "Something went wrong.");
        }
    }
}