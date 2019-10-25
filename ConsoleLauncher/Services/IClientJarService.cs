using System.Threading.Tasks;
using ConsoleLauncher.Models;

namespace ConsoleLauncher.Services
{
    public interface IClientJarService
    {
        Task<double> GetLatestVersion(Game game);
        Task<string> GetLatestJarPath(Game game);
        Task<bool> HasLatestJar(Game game);
        Task<double> GetVersionByHash(Game game, string hash);
        Task DownloadLatestJar(Game game);
        Task<bool> HasAccess(Game game);
    }
}