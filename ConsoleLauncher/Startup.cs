using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            while (true)
            {
                try
                {
                    foreach (var view in _views)
                    {
                        if (await view.Validate())
                        {
                            await view.Execute();
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    await _logger.Log("", e);
                }
                await Task.Delay(1000);
            }
        }
    }
}
