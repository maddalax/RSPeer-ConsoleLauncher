using System;
using System.Threading.Tasks;
using ConsoleLauncher.Models;
using ConsoleLauncher.Services;

namespace ConsoleLauncher.Views
{
    public class UserLoginView : IView
    {
        private readonly IUserService _service;

        public UserLoginView(IUserService service)
        {
            _service = service;
        }

        public async Task<bool> Validate()
        {
            return !await _service.HasSession();
        }

        public async Task Execute()
        {
            Console.WriteLine("Enter your RSPeer email address:");
            var email = Console.ReadLine();
            Console.WriteLine("Enter your RSPeer password:");
            var password = Console.ReadLine();
            try
            {
                await _service.Login(email, password);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public ViewType Type { get; } = ViewType.Startup;
    }
}