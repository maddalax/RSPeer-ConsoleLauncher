using System;
using System.Threading.Tasks;
using ConsoleLauncher.Extensions;
using ConsoleLauncher.Models;
using ConsoleLauncher.Services;
using ConsoleLauncher.Shell;

namespace ConsoleLauncher.Views
{
    public class LaunchClientView : IView
    {
        private readonly IClientLaunchService _launchService;
        private readonly DownloadClientView _downloadClientView;
        private readonly IAuthorizationService _authorization;
        private readonly ILogger _logger;

        public LaunchClientView(IClientLaunchService launchService, DownloadClientView downloadClientView, IAuthorizationService authorization, ILogger logger)
        {
            _launchService = launchService;
            _downloadClientView = downloadClientView;
            _authorization = authorization;
            _logger = logger;
        }

        public async Task<bool> Validate()
        {
            return true;
        }

        public async Task Execute()
        {
           Console.WriteLine("What would you like to do?");
           Console.WriteLine("(1) Launch Runescape 2007 Client.");
           Console.WriteLine("(2) Launch Runescape RS3 Client.");
           Console.WriteLine("(3) Check for Client Updates");
           Console.WriteLine("(4) Troubleshoot Client Issues. Choose this option if your client is not starting or an error has occured.");
           Console.WriteLine("(5) Open Bot Management Panel to manage quick launch configurations, running clients, and proxies.");
           Console.WriteLine("Type an option, such as '1'.");
           var option = Console.ReadLine();
           
           if (!int.TryParse(option, out var number))
           {
               Console.WriteLine("Invalid choice, option must be a number.");
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
                   await CheckUpdate();
                   break;
               case 4:
                   await OpenTroubleGuide();
                   break;
               case 5:
                   await OpenBotManagement();
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

        private async Task CheckUpdate()
        {
            if (!await _downloadClientView.Validate())
            {
                Console.WriteLine("No update available. You are on the latest client version.");
                return;
            }

            await _downloadClientView.Execute();
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
    }
}