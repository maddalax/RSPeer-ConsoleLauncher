using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ConsoleLauncher.Extensions;
using ConsoleLauncher.Models;
using ConsoleLauncher.Providers;
using ConsoleLauncher.Shell;

namespace ConsoleLauncher.Services
{
    public class ClientJarService : IClientJarService
    {
        private HttpClient _client;
        private IAppArgProvider _provider;
        private readonly IFileService _fileService;
        private readonly ILogger _logger;

        public ClientJarService(IHttpClientFactory factory, IAppArgProvider provider, IFileService fileService, ILogger logger)
        {
            _client = factory.CreateClient("ClientJarDownloader");
            _provider = provider;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<double> GetLatestVersion(Game game)
        {
            var result = await _client.GetStringAsync("https://services.rspeer.org/api/bot/currentVersionRaw?game=" + game);
            return double.Parse(result);
        }

        public async Task<string> GetLatestJarPath(Game game)
        {
            var version = await GetLatestVersion(game);
            await _logger.Log($"Latest client version for {game.ToString()} is {version}");
            var folder = _fileService.GetJarFolder(game);
            var jar = Path.Join(folder, $"{version}.jar");
            
            if (!File.Exists(jar))
            {
                return jar;
            }

            await using var stream = File.OpenRead(jar);
            
            var hash = stream.CalculateHash();
            var versionByHash = await GetVersionByHash(game, hash);
            
            // Check if the hash of the jar file exists what is on live. If it does not, something must have failed when downloading.
            if (versionByHash < version)
            {
                await _logger.Log("Current jar is outdated or corrupt, deleting file jar file.");
                File.Delete(jar);
            }
            
            return jar;
        }

        public async Task<bool> HasLatestJar(Game game)
        {
            var path = await GetLatestJarPath(game);
            return File.Exists(path);
        }

        public async Task<double> GetVersionByHash(Game game, string hash)
        {
            var result = await _client.GetStringAsync($"https://services.rspeer.org/api/bot/getVersionByHash?game={game}&hash={hash}");
            return double.TryParse(result, out var version) ? version : 0.00;
        }

        public async Task DownloadLatestJar(Game game)
        {
            if (await HasLatestJar(game))
            {
                await _logger.Log("Attempted to download latest jar file for " + game +
                                  " but we already have the latest. Skipping.");
                return;
            }
            
            await _logger.Log("Downloading the latest jar file for " + game.ToString());
            var path = await GetLatestJarPath(game);
            var url = "https://services.rspeer.org/api/bot/currentJar?game=" + game;
            await _logger.Log("Saving to path " + path);
            await _logger.Log("Sending request to " + url);
            await using var stream = await _client.GetStreamAsync(url);
            await _logger.Log("Creating file and copying downloaded jar.");
            await using var file = File.Create(path);
            await stream.CopyToAsync(file);
            await _logger.Log("Successfully downloaded jar.");
        }
    }
}