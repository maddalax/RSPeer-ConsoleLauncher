using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLauncher.Models;
using ConsoleLauncher.Services;
using ConsoleLauncher.Shell;

namespace ConsoleLauncher.Views
{
    public class UserLoginView : IView
    {
        private readonly IUserService _service;
        private readonly ILogger _logger;

        public UserLoginView(IUserService service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<bool> Validate(CancellationToken token)
        {
            return !await _service.HasSession();
        }

        public async Task Execute(CancellationToken token)
        {
            Console.WriteLine("Enter your RSPeer email address:");
            var email = Console.ReadLine();
            Console.WriteLine("Enter your RSPeer password:");
            var password = GetPassword();
            Console.WriteLine();
            Console.WriteLine("Logging in... please wait.");
            await Task.Delay(500, token);
            try
            {
                await _service.Login(email, password);
                Console.WriteLine("Successfully logged in.");
            }
            catch (Exception e)
            {
                await _logger.Log(e.Message);
            }
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