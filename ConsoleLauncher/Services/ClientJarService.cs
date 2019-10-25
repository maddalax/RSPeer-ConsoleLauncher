using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ConsoleLauncher.Extensions;
using ConsoleLauncher.Models;
using ConsoleLauncher.Shell;

namespace ConsoleLauncher.Services
{
    public class ClientJarService : IClientJarService
    {
        private HttpClient _client;
        private readonly IAuthorizationService _authorization;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        private readonly ILogger _logger;

        public ClientJarService(IHttpClientFactory factory, IUserService userService, IFileService fileService, ILogger logger, IAuthorizationService authorization)
        {
            _client = factory.CreateClient("ClientJarDownloader");
            _userService = userService;
            _fileService = fileService;
            _logger = logger;
            _authorization = authorization;
        }

        public async Task<double> GetLatestVersion(Game game)
        {
            var result = await _client.GetStringAsync("https://services.rspeer.org/api/bot/currentVersionRaw?game=" + game);
            return double.Parse(result);
        }

        public async Task<string> GetLatestJarPath(Game game)
        {
            var version = await GetLatestVersion(game);
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

            if (!await HasAccess(game))
            {
                return;
            }
            
            var session = await _authorization.GetSession();

            if (string.IsNullOrEmpty(session))
            {
                await _logger.Log("Attempted to downloaded latest client version but session was null.");
                return;
            }
            
            await _logger.Log("Downloading the latest jar file for " + game);
            var path = await GetLatestJarPath(game);
            var url = "https://services.rspeer.org/api/bot/currentJar?game=" + game;
            await _logger.Log("Saving to path " + path);
            await _logger.Log("Sending request to " + url);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", "Bearer " + session);
            var response = await _client.SendAsync(request);
            await _logger.Log("Creating file and copying downloaded jar.");
            await using var file = File.Create(path);
            using var stream = response.Content;
            await stream.CopyToAsync(file);
            await _logger.Log("Successfully downloaded jar.");
        }

        public async Task<bool> HasAccess(Game game)
        {
            if (game == Game.Osrs)
            {
                return true;
            }

            var user = await _userService.GetUser();
            return user.GroupNames.FirstOrDefault(w => w == "Inuvation" || w == "Inuvation Maintainers") != null;
        }
    }
}