using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLauncher.Extensions;
using ConsoleLauncher.Models.Requests;
using ConsoleLauncher.Models.Responses;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace ConsoleLauncher.Services
{
    public class MessageService : IMessageService
    {
        private readonly IApiService _api;
        private readonly HttpClient _client;
        private readonly IUserService _userService;
        private readonly IClientLaunchService _launch;
        private readonly IHostApplicationLifetime _appLifetime;

        private bool _disposed;
        private string _lastIp;
        private DateTimeOffset _lastRegister;
        private readonly SemaphoreSlim _semaphore;
        private readonly HashSet<int> _consumed;

        public MessageService(IApiService api, IHttpClientFactory factory, IUserService userService, IClientLaunchService launch, 
            IHostApplicationLifetime appLifetime)
        {
            _semaphore = new SemaphoreSlim(1);
            _consumed = new HashSet<int>();
            _lastRegister = DateTimeOffset.MinValue;
            _api = api;
            _userService = userService;
            _client = factory.CreateClient("Default");
            _launch = launch;
            _appLifetime = appLifetime;
        }

        public async Task Poll(string tag)
        {
            try
            {
                if (!await _userService.HasSession())
                {
                    return;
                }
                await _semaphore.WaitAsync();
                await Register(tag);
                var messages = await _api.Get<IEnumerable<RemoteMessage>>("message/get?consumer=" + tag);
                foreach (var remoteMessage in messages)
                {
                    if (_consumed.Contains(remoteMessage.Id))
                    {
                        continue;
                    }
                    
                    await Consume(remoteMessage.Id);
                    
                    if (remoteMessage.Source == "bot_panel_user_request")
                    {
                        var request = JsonConvert.DeserializeObject<BotPanelUserRequest>(remoteMessage.Message);
                        if (request.Type == "start:client")
                        {
                            await _launch.Launch(request);
                        }

                        if (request.Type == "launcher:kill")
                        {
                            _appLifetime.StopApplication();
                        }
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task Dispose(string tag)
        {
            if (_disposed)
            {
                return;
            }
            await _api.Post<object>("botLauncher/unregister", new LauncherUnregisterRequest
            {
                Tag = tag
            });
            _client?.Dispose();
            _disposed = true;
        }

        private async Task Register(string tag)
        {
            if (_lastRegister.AddSeconds(30) > DateTimeOffset.Now)
            {
                return;
            }
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

        private async Task Consume(int id)
        {
            if (_consumed.Count > 100)
            {
                _consumed.Clear();
            }
            _consumed.Add(id);
            await _api.Post<object>("message/consume?message=" + id, new object());
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
    }
}