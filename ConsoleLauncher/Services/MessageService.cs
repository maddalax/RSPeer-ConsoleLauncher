using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ConsoleLauncher.Extensions;
using ConsoleLauncher.Models.Requests;
using ConsoleLauncher.Models.Responses;

namespace ConsoleLauncher.Services
{
    public class MessageService : IMessageService, IAsyncDisposable
    {
        private readonly IApiService _api;
        private readonly HttpClient _client;
        private readonly IUserService _userService;

        private string _tag;
        private string _lastIp;
        private DateTimeOffset _lastRegister;

        public MessageService(IApiService api, IHttpClientFactory factory, IUserService userService)
        {
            _lastRegister = DateTimeOffset.MinValue;
            _api = api;
            _userService = userService;
            _client = factory.CreateClient("Default");
        }

        public async Task Poll(string tag)
        {
            _tag = tag;
           await Register(tag);
           var messages = await _api.Get<IEnumerable<RemoteMessage>>("message/get?consumer=" + tag);
           foreach (var remoteMessage in messages)
           {
               //Console.WriteLine(JsonSerializer.Serialize(remoteMessage));
           }
        }

        private async Task Register(string tag)
        {
            if (_lastRegister.AddSeconds(30) > DateTimeOffset.Now)
            {
                return;
            }
            Console.WriteLine("Registering: " + tag);
            _lastRegister = DateTimeOffset.Now;
            _lastIp = await TryGetIp();
            var user = await _userService.GetUser();
            await _api.Post<object>("botLauncher/register", new LauncherRegisterRequest
            {
                Tag = tag,
                Ip = _lastIp,
                Host = Dns.GetHostName(),
                MachineUsername = Environment.MachineName,
                Platform = RuntimeExtensions.RuntimeToString(),
                UserId = user.Id
            });
        }

        private async Task<string> TryGetIp()
        {
            try
            {
                 var ip = await _client.GetStringAsync("https://checkip.amazonaws.com");
                 return ip?.Trim() ?? _lastIp;
            }
            catch (Exception)
            {
                return _lastIp;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _api.Post<object>("botLauncher/unregister", new LauncherUnregisterRequest
            {
                Tag = _tag
            });
        }
    }
}