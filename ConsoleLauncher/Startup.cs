using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;
using ConsoleLauncher.Models;
using ConsoleLauncher.Services;
using ConsoleLauncher.Shell;
using ConsoleLauncher.Views;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleLauncher
{
    public class Startup
    {
        private readonly IEnumerable<IView> _views;
        private readonly IServiceScopeFactory _factory;
        private readonly ILogger _logger;
        private readonly Guid _tag;

        public Startup(IEnumerable<IView> views, IServiceScopeFactory factory, ILogger logger)
        {
            _views = views;
            _factory = factory;
            _logger = logger;
            _tag = Guid.NewGuid();
        }

        public async Task Execute()
        {
            SetupShutdownHook();
            await StartMessageService();
            await ExecuteStartupViews();
            while (true)
            {
                await ExecuteViews(_views.Where(w => w.Type == ViewType.Regular));
                await Task.Delay(1000);
            }
        }

        private async Task StartMessageService()
        {
            await Task.Factory.StartNew(async () =>
            {
                using var scope = _factory.CreateScope();
                while (true)
                {
                    try
                    {
                        var service = scope.ServiceProvider.GetService<IMessageService>();
                        await service.Poll(_tag.ToString());
                        await Task.Delay(5000);
                    }
                    catch (Exception e)
                    {
                        await _logger.Log(string.Empty, e);
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        private void SetupShutdownHook()
        {
            AssemblyLoadContext.Default.Unloading += context =>
            {
                using var scope = _factory.CreateScope();
                var service = scope.ServiceProvider.GetService<IMessageService>();
                if (service is IAsyncDisposable disposable)
                {
                    disposable.DisposeAsync();
                }
            };
            
            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
            {
                using var scope = _factory.CreateScope();
                var service = scope.ServiceProvider.GetService<IMessageService>();
                if (service is IAsyncDisposable disposable)
                {
                    disposable.DisposeAsync();
                }
            };
        }

        private async Task ExecuteStartupViews()
        {
            await ExecuteViews(_views.Where(w => w.Type == ViewType.Startup).ToList());
        }

        private async Task ExecuteViews(IEnumerable<IView> views)
        {
            try
            {
                foreach (var view in views)
                {
                    if (!await view.Validate())
                    {
                        continue;
                    }
                    await view.Execute();
                    break;
                }
            }
            catch (Exception e)
            {
                await _logger.Log("", e);
            }
        }
    }
}
