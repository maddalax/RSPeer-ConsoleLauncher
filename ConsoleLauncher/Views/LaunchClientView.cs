using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLauncher.Extensions;
using ConsoleLauncher.Models;
using ConsoleLauncher.Models.Responses;
using ConsoleLauncher.Services;
using ConsoleLauncher.Shell;
using Microsoft.Extensions.Hosting;

namespace ConsoleLauncher.Views
{
    public class LaunchClientView : IView
    {
        private readonly IClientLaunchService _launchService;
        private readonly IClientJarService _jarService;
        private readonly IAuthorizationService _authorization;
        private readonly IUserService _userService;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly ILogger _logger;
        private User _user;

        public LaunchClientView(IClientLaunchService launchService, IAuthorizationService authorization, IUserService userService, IHostApplicationLifetime appLifetime, ILogger logger, IClientJarService jarService)
        {
            _launchService = launchService;
            _authorization = authorization;
            _userService = userService;
            _appLifetime = appLifetime;
            _jarService = jarService;
            _logger = logger;
        }

        public Task<bool> Validate(CancellationToken token)
        {
            return _userService.HasSession();
        }

        public async Task Execute(CancellationToken token)
        {
            if (_user == null)
            {
                _user = await _userService.GetUser();
                Console.WriteLine("Welcome " + _user.Username + ".");
                Console.WriteLine("This launcher must stay open to use with the Bot Management Panel.");
            }
            Console.WriteLine();
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("(1) Launch Runescape 2007 Client.");
            Console.WriteLine("(2) Launch Runescape RS3 Client.");
            Console.WriteLine("(3) Troubleshoot Client Issues. Choose this option if your client is not starting or an error has occured.");
            Console.WriteLine("(4) Open Bot Management Panel to manage quick launch configurations, running clients, and proxies.");
            Console.WriteLine("(5) Shut Down Launcher Safely.");
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
                    await OpenTroubleGuide();
                    break;
                case 4:
                    await OpenBotManagement();
                    break;
                case 5:
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
            if (game == Game.Rs3 && !await _jarService.HasAccess(Game.Rs3))
            {
               await _logger.Log("You do not have access to RSPeer 3 Inuvation.");
               await _logger.Log("Visit https://rspeer.org/runescape-3-rs3-bot/ for more information.");
               ProcessExtensions.OpenUrl("https://rspeer.org/runescape-3-rs3-bot/");
               return;
            }
            await _launchService.Launch(new BotPanelUserRequest
           {
               Game = game
           });
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