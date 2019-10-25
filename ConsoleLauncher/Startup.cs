using System.Threading.Tasks;
using ConsoleLauncher.Models;
using ConsoleLauncher.Services;

namespace ConsoleLauncher
{
    public class Startup
    {
        private readonly IClientJarService _service;

        public Startup(IClientJarService service)
        {
            _service = service;
        }

        public async Task Execute()
        {
            await _service.DownloadLatestJar(Game.Osrs);
        }
    }
}
