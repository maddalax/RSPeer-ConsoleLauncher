using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliWrap;
using ConsoleLauncher.Models;
using ConsoleLauncher.Models.Responses;
using ConsoleLauncher.Shell;
using Newtonsoft.Json;

namespace ConsoleLauncher.Services
{
    public class ClientLaunchService : IClientLaunchService
    {
        private readonly IClientJarService _jarService;
        private readonly ILogger _logger;

        public ClientLaunchService(IClientJarService jarService, ILogger logger)
        {
            _jarService = jarService;
            _logger = logger;
        }

        public async Task Launch(Game game, BotPanelUserRequest request, LegacyQuickLaunch quickLaunch)
        {
            await _logger.Log("Attempting to launch client for " + game + ".");
            var path = await _jarService.GetLatestJarPath(game);
            if (!await _jarService.HasLatestJar(game))
            {
                await _jarService.DownloadLatestJar(game);
            }
            var command = "java -jar " + path;
            var args = new List<string>();
            if (request.Game == Game.Rs3)
            {
                args.Add("-noverify");
            }
            request.JvmArgs = string.IsNullOrEmpty(request.JvmArgs)
                ? "-Xmx768m -Djava.net.preferIPv4Stack=true -Djava.net.preferIPv4Addresses=true -Xss2m"
                : request.JvmArgs;
            
            args.AddRange(request.JvmArgs.Split(" "));
            args.Add("-jar");
            args.Add(path);
            
            if (quickLaunch != null)
            {
                args.Add("-qs");
                args.Add(Convert.ToBase64String(Encoding.Default.GetBytes(JsonConvert.SerializeObject(quickLaunch))));
            }

            await _logger.Log("Launching with JVM arguments:");
            await _logger.Log(string.Join(' ', args));
            
            Cli.Wrap("java")
                .SetArguments(args.ToArray())
                .SetStandardOutputCallback(l => _logger.Log(l))
                .SetStandardErrorCallback(l => _logger.Log($"Error: {l}"))
                .ExecuteAndForget();
            await _logger.Log($"Successfully executed {command}. Client should be starting shortly.");
            await _logger.Log($"Any errors will be printed to the console.");
            await Task.Delay(1000);
        }

        public async Task Launch(BotPanelUserRequest request)
        {
            if (request.Qs != null)
            {
                await LaunchQuickLaunch(request);
            }
            else
            {
                for (var i = 0; i < request.Count; i++)
                {
                    await Launch(request.Game, request, request.Proxy != null ? new LegacyQuickLaunch(new Client
                    {
                        Proxy = request.Proxy
                    }) : null);
                    await Sleep(request);
                }
            }
        }

        private async Task LaunchQuickLaunch(BotPanelUserRequest request)
        {
            var launches = request.Qs.Clients.Select(w => new LegacyQuickLaunch(w));
            foreach (var qs in launches)
            {
                await Launch(qs.Game, request, qs);
                await Sleep(request);
            }
        }

        private async Task Sleep(BotPanelUserRequest request)
        {
            await Task.Delay(request.Sleep < 1000 ? request.Sleep * 1000 : request.Sleep);
        }
    }
}