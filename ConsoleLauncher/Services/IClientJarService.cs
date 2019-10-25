using System.Threading.Tasks;
using ConsoleLauncher.Models;

namespace ConsoleLauncher.Services
{
    public interface IClientJarService
    {
        Task<double> GetLatestVersion(Game game);
        Task<string> GetLatestJarPath(Game game);
        Task DownloadLatestJar(Game game);
    }
}