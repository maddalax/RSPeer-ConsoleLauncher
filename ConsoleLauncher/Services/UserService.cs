using System.Threading.Tasks;
using ConsoleLauncher.Models;
using ConsoleLauncher.Models.Requests;
using ConsoleLauncher.Models.Responses;

namespace ConsoleLauncher.Services
{
    public class UserService : IUserService
    {
        private readonly IAuthorizationService _authorization;
        private readonly IApiService _api;

        public UserService(IAuthorizationService authorization, IApiService api)
        {
            _authorization = authorization;
            _api = api;
        }
        
        public async Task<User> GetUser()
        {
            var session = await _authorization.GetSession();
            if (string.IsNullOrEmpty(session))
            {
                return null;
            }
            return await _api.Get<User>("user/me");
        }

        public async Task<bool> HasSession()
        {
            var session = await _authorization.GetSession();
            return !string.IsNullOrEmpty(session);
        }

        public async Task Login(string email, string password)
        {
            var response = await _api.Post<UserLoginResponse>("user/login", new UserLoginRequest {Email = email, Password = password});
            await _authorization.WriteSession(response.Token);
        }
    }
}