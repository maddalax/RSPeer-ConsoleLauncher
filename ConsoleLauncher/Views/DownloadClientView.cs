using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLauncher.Models;
using ConsoleLauncher.Services;

namespace ConsoleLauncher.Views
{
    public class DownloadClientView : IView
    {
        private readonly IClientJarService _service;
        public bool SkipUpdate { get; private set; }
        
        public DownloadClientView(IClientJarService service)
        {
            _service = service;
        }

        public async Task<bool> Validate(CancellationToken token)
        {
            if (!await _service.HasLatestJar(Game.Osrs))
            {
                return true;
            }

            return await _service.HasAccess(Game.Rs3) && !await _service.HasLatestJar(Game.Rs3);
        }

        public async Task Execute(CancellationToken token)
        {
            Console.WriteLine("Your client is outdated with the latest version. RSPeer may have been updated or you do not have the client downloaded.");
            Console.WriteLine("Would you like to download the latest client?");
            Console.WriteLine("If you choose No, you won't be asked again until you restart this application.");
            Console.WriteLine("Y/N?");
            
            var response = Console.ReadLine()?.ToLower().Trim();
            if (response == "n" || response == "no")
            {
                SkipUpdate = true;
                return;
            }

            if (response != "y" && response != "yes")
            {
                Console.WriteLine("Invalid response, must be 'Y' or 'N'");
                return;
            }

            await Task.Delay(1000, token);
            await _service.DownloadLatestJar(Game.Osrs);
            await Task.Delay(2000, token);
            await _service.DownloadLatestJar(Game.Rs3);
        }

        public ViewType Type { get; } = ViewType.Startup;
    }
}