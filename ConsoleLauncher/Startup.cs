using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsoleLauncher.Models;
using ConsoleLauncher.Shell;
using ConsoleLauncher.Views;

namespace ConsoleLauncher
{
    public class Startup
    {
        private readonly IEnumerable<IView> _views;
        private readonly ILogger _logger;

        public Startup(IEnumerable<IView> views, ILogger logger)
        {
            _views = views;
            _logger = logger;
        }

        public async Task Execute()
        {
            await ExecuteStartupViews();
            while (true)
            {
                await ExecuteViews(_views.Where(w => w.Type == ViewType.Regular));
                await Task.Delay(1000);
            }
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
