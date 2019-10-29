using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLauncher.Extensions;
using ConsoleLauncher.Models;
using ConsoleLauncher.Services;
using ConsoleLauncher.Shell;
using Microsoft.Extensions.Hosting;

namespace ConsoleLauncher.Views
{
    public class LaunchClientView : IView
    {
        private readonly IClientLaunchService _launchService;
        private readonly DownloadClientView _downloadClientView;
        private readonly IAuthorizationService _authorization;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly ILogger _logger;

        public LaunchClientView(IClientLaunchService launchService, DownloadClientView downloadClientView, IAuthorizationService authorization, IHostApplicationLifetime appLifetime, ILogger logger)
        {
            _launchService = launchService;
            _downloadClientView = downloadClientView;
            _authorization = authorization;
            _appLifetime = appLifetime;
            _logger = logger;
        }

        public Task<bool> Validate(CancellationToken token)
        {
            return Task.FromResult(true);
        }

        public async Task Execute(CancellationToken token)
        {
           Console.WriteLine();
           Console.WriteLine("What would you like to do?");
           Console.WriteLine("(1) Launch Runescape 2007 Client.");
           Console.WriteLine("(2) Launch Runescape RS3 Client.");
           Console.WriteLine("(3) Check for Client Updates.");
           Console.WriteLine("(4) Troubleshoot Client Issues. Choose this option if your client is not starting or an error has occured.");
           Console.WriteLine("(5) Open Bot Management Panel to manage quick launch configurations, running clients, and proxies.");
           Console.WriteLine("(6) Shut Down Launcher Safely.");
           Console.WriteLine("Type an option, such as '1'.");
           Console.WriteLine();
           var option = await ConsoleExtensions.WaitForKey(token);
           
           if (!option.HasValue || !int.TryParse(option.Value.KeyChar.ToString(), out var number))
           {
               Console.WriteLine("Invalid choice, option must be a number.");
               return;
           }

           switch (number)
           {
               case 1:
                   await LaunchClient(Game.Osrs);
                   break;
               case 2:
                   await LaunchClient(Game.Rs3);
                   break;
               case 3:
                   await CheckUpdate(token);
                   break;
               case 4:
                   await OpenTroubleGuide();
                   break;
               case 5:
                   await OpenBotManagement();
                   break;
               case 6:
                   Close();
                   break;
               default:
                   Console.WriteLine("Invalid option.");
                   break;
           }
        }

        public ViewType Type { get; } = ViewType.Regular;

        private async Task LaunchClient(Game game)
        {
           await _launchService.Launch(game);
        }

        private async Task CheckUpdate(CancellationToken token)
        {
            if (!await _downloadClientView.Validate(token))
            {
                Console.WriteLine("No update available. You are on the latest client version.");
                return;
            }

            await _downloadClientView.Execute(token);
        }

        private async Task OpenTroubleGuide()
        {
            await _logger.Log("Opening: https://docs.rspeer.org/docs/client-wont-start");
            ProcessExtensions.OpenUrl("https://docs.rspeer.org/docs/client-wont-start");
        }

        private async Task OpenBotManagement()
        {
            await _logger.Log("Opening: https://app.rspeer.org/#/bot/management");
            var session = await _authorization.GetSession();
            var menu = "menu=bot_panel";
            var qs = session != null ? $"?idToken={session}&{menu}" : $"?{menu}";
            ProcessExtensions.OpenUrl("https://app.rspeer.org/#/bot/management" + qs);
        }
        
        private void Close()
        {
            _appLifetime.StopApplication();
        }
    }
}