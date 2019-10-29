using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLauncher.Models;
using ConsoleLauncher.Providers;
using ConsoleLauncher.Services;
using ConsoleLauncher.Shell;

namespace ConsoleLauncher.Views
{
    public class UserLoginView : IView
    {
        private readonly IUserService _service;
        private readonly ILogger _logger;
        private readonly IAppArgProvider _argProvider;
        private int _tries;

        public UserLoginView(IUserService service, ILogger logger, IAppArgProvider argProvider)
        {
            _service = service;
            _logger = logger;
            _argProvider = argProvider;
        }

        public async Task<bool> Validate(CancellationToken token)
        {
            return !await _service.HasSession();
        }

        public async Task Execute(CancellationToken token)
        {
            var credentials = GetCredentials();
            Console.WriteLine();
            Console.WriteLine("Logging in... please wait.");
            await Task.Delay(500, token);
            try
            {
                await _service.Login(credentials.email, credentials.password);
                Console.WriteLine("Successfully logged in.");
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message);
                _tries++;
            }

            // Failed to login with args 5 times, clear the args.
            if (_tries > 5 && _argProvider.Email != null)
            {
                await _logger.Log("Failed to login over 5 times, clearing arguments. Please login manually.");
                _argProvider.Email = null;
                _argProvider.Password = null;
            }
        }

        private (string email, string password) GetCredentials()
        {
            if (_argProvider.Email != null && _argProvider.Password != null)
            {
                return (_argProvider.Email, _argProvider.Password);
            }
            Console.WriteLine("Enter your RSPeer email address:");
            var email = Console.ReadLine();
            Console.WriteLine("Enter your RSPeer password:");
            var password = GetPassword();
            return (email, password);
        }

        public static string GetPassword()
        {
            var pass = string.Empty;
            do
            {
                var key = Console.ReadKey(true);
                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Substring(0, (pass.Length - 1));
                        Console.Write("\b \b");
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }
            } while (true);

            return pass;
        }

        public ViewType Type { get; } = ViewType.Regular;
    }
}