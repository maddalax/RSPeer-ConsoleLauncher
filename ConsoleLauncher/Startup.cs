using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLauncher.Models;
using ConsoleLauncher.Services;
using ConsoleLauncher.Shell;
using ConsoleLauncher.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsoleLauncher
{
    public class Startup : IHostedService
    {
        private readonly IEnumerable<IView> _views;
        private readonly IServiceScopeFactory _factory;
        private readonly ILogger _logger;
        private readonly Guid _tag;
        private readonly IHostApplicationLifetime _appLifetime;

        public Startup(IEnumerable<IView> views, IServiceScopeFactory factory, ILogger logger, IHostApplicationLifetime appLifetime)
        {
            _views = views;
            _factory = factory;
            _logger = logger;
            _appLifetime = appLifetime;
            _tag = Guid.NewGuid();
        }
        
        private async Task StartMessageService(CancellationToken token)
        {
            await Task.Factory.StartNew(async () =>
            {
                using var scope = _factory.CreateScope();
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var service = scope.ServiceProvider.GetService<IMessageService>();
                        await service.Poll(_tag.ToString());
                        await Task.Delay(5000, token);
                    }
                    catch (Exception e)
                    {
                        if (token.IsCancellationRequested && e is TaskCanceledException)
                        {
                            return;
                        }
                        await _logger.Log(string.Empty, e);
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }
        
        private async Task ExecuteStartupViews(CancellationToken token)
        {
            await ExecuteViews(_views.Where(w => w.Type == ViewType.Startup).ToList(), token);
        }

        private async Task ExecuteViews(IEnumerable<IView> views, CancellationToken token)
        {
            try
            {
                foreach (var view in views)
                {
                    if (!await view.Validate(token))
                    {
                        continue;
                    }
                    await view.Execute(token);
                    break;
                }
            }
            catch (Exception e)
            {
                if (token.IsCancellationRequested && e is TaskCanceledException)
                {
                    return;
                }
                await _logger.Log("", e);
            }
        }
        
        public async Task StartAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            await _logger.Log($"Starting RSPeer Console Launcher V{Program.Version}.");
            _appLifetime.ApplicationStopped.Register(Shutdown);
            _appLifetime.ApplicationStopping.Register(Shutdown);
            await StartMessageService(cancellationToken);
            await ExecuteStartupViews(cancellationToken);
            while (!cancellationToken.IsCancellationRequested)
            {
                await ExecuteViews(_views.Where(w => w.Type == ViewType.Regular), cancellationToken);
                await Task.Delay(1000, cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            Shutdown();
            return Task.CompletedTask;
        }

        private void Shutdown()
        {
            Console.WriteLine("Shutting down launcher.");
            Program.Shutdown();
            using var scope = _factory.CreateScope();
            scope.ServiceProvider.GetService<IMessageService>().Dispose(_tag.ToString()).Wait();
        }
    }
}
