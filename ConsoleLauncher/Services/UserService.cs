using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ConsoleLauncher.Models;
using ConsoleLauncher.Models.Requests;
using ConsoleLauncher.Models.Responses;

namespace ConsoleLauncher.Services
{
    public class UserService : IUserService
    {
        private readonly IAuthorizationService _authorization;
        private readonly HttpClient _client;

        public UserService(IAuthorizationService authorization, IHttpClientFactory factory)
        {
            _authorization = authorization;
            _client = factory.CreateClient("UserService");
        }

        public async Task<User> GetUser()
        {
            var session = await _authorization.GetSession();
            if (string.IsNullOrEmpty(session))
            {
                return null;
            }
            var message = new HttpRequestMessage(HttpMethod.Get, "https://services.rspeer.org/api/user/me");
            message.Headers.Add("Authorization", "Bearer " + session);
            var result = await _client.SendAsync(message);
            var content = await result.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> HasSession()
        {
            var session = await _authorization.GetSession();
            return !string.IsNullOrEmpty(session);
        }

        public async Task Login(string email, string password)
        {
            var serialized = JsonSerializer.Serialize(new UserLoginRequest {Email = email, Password = password});
            var response = await _client.PostAsync("https://services.rspeer.org/api/user/login", 
                new StringContent(serialized, Encoding.Default, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var token = JsonSerializer.Deserialize<UserLoginResponse>(content,
                    new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
                await _authorization.WriteSession(token.Token);
                return;
            }

            var error = JsonSerializer.Deserialize<ApiErrorResponse>(content, new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
            throw new Exception(error.Error ?? "Something went wrong.");
        }
    }
}