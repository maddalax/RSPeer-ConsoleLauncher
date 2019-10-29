using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CliWrap;
using ConsoleLauncher.Models;
using ConsoleLauncher.Shell;

namespace ConsoleLauncher.Services
{
    public class ClientLaunchService : IClientLaunchService
    {
        private readonly IClientJarService _jarService;
        private readonly ILogger _logger;

        public ClientLaunchService(IClientJarService jarService, ILogger logger)
        {
            _jarService = jarService;
            _logger = logger;
        }

        public async Task Launch(Game game)
        {
            await _logger.Log("Attempting to launch client for " + game + ".");
            var path = await _jarService.GetLatestJarPath(game);
            var command = "java -jar " + path;
            Cli.Wrap("java")
                .SetArguments(new[] {"-jar", path})
                .SetStandardOutputCallback(l => _logger.Log(l))
                .SetStandardErrorCallback(l => _logger.Log($"Error: {l}"))
                .ExecuteAndForget();
            await _logger.Log($"Successfully executed {command}. Client should be starting shortly.");
            await _logger.Log($"Any errors will be printed to the console.");
            await Task.Delay(1000);
        }
    }
}