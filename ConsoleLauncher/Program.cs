using System;
using System.Threading.Tasks;
using ConsoleLauncher.Providers;
using ConsoleLauncher.Services;
using ConsoleLauncher.Shell;
using ConsoleLauncher.Views;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var program = new Program();
                program.Start(args).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task Start(string[] args)
        {
            var collection = new ServiceCollection();
            
            collection.AddHttpClient();
            collection.AddSingleton<IAppArgProvider>(w => new AppArgProvider(args));
            collection.AddSingleton<Startup>();
            collection.AddSingleton<ILogger, ConsoleLogger>();
            collection.AddScoped<IFileService, FileService>();
            collection.AddScoped<IClientJarService, ClientJarService>();
            collection.AddScoped<IUserService, UserService>();
            collection.AddScoped<IAuthorizationService, AuthorizationService>();
            collection.AddScoped<IView, UserLoginView>();
            collection.AddScoped<IView, DownloadClientView>();
            collection.AddScoped<IView, LaunchClientView>();
            
            var provider = collection.BuildServiceProvider();
            await provider.GetService<Startup>().Execute();
        }
    }
}