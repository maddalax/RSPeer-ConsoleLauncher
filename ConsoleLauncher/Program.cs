using System;
using System.Threading.Tasks;
using ConsoleLauncher.Providers;
using ConsoleLauncher.Services;
using ConsoleLauncher.Shell;
using ConsoleLauncher.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ILogger = ConsoleLauncher.Shell.ILogger;

namespace ConsoleLauncher
{
    class Program
    {
        public const double Version = 1.00;
        private static bool _stopping;
        
        public static void Shutdown()
        {
            _stopping = true;
        }
            
        public static async Task Main(string[] args)
        {
            try
            {
                await CreateHostBuilder(args)
                    .RunConsoleAsync();
            }
            catch (Exception e)
            {
                if (_stopping && e is TaskCanceledException)
                {
                    return;
                }
                Console.WriteLine(e);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(w =>
                {
                    w.SetMinimumLevel(LogLevel.Error);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient();
                    services.AddSingleton<IAppArgProvider>(w => new AppArgProvider(args));
                    services.AddSingleton<Startup>();
                    services.AddSingleton<ILogger, ConsoleLogger>();
                    services.AddScoped<IFileService, FileService>();
                    services.AddScoped<IClientJarService, ClientJarService>();
                    services.AddScoped<IUserService, UserService>();
                    services.AddScoped<IAuthorizationService, AuthorizationService>();
                    services.AddScoped<IView, UserLoginView>();
                    services.AddScoped<IView, LaunchClientView>();
                    services.AddScoped<IClientLaunchService, ClientLaunchService>();
                    services.AddScoped<IMessageService, MessageService>();
                    services.AddScoped<IApiService, ApiService>();
                    services.AddHostedService<Startup>();
                });
    }
}