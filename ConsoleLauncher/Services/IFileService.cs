using ConsoleLauncher.Models;

namespace ConsoleLauncher.Services
{
    public interface IFileService
    {
        string GetJarFolder(Game game);
        string GetCacheFolder(Game game);
    }
}