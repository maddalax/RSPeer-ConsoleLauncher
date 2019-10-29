using System;
using System.IO;
using System.Runtime.InteropServices;
using ConsoleLauncher.Models;

namespace ConsoleLauncher.Services
{
    public class FileService : IFileService
    {
        public string GetJarFolder(Game game)
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var path = Path.Join(home, game == Game.Osrs ? ".rspeer" : ".rspeer_inuvation");
            Directory.CreateDirectory(path);
            return path;
        }

        public string GetCacheFolder(Game game)
        {
            var folder = game == Game.Osrs ? "RSPeer/cache" : "RSPeer Inuvation/cache";
            string path;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                path = Path.Join(documents, folder);
                Directory.CreateDirectory(path);
                return path;
            }
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            path = Path.Join(home, folder);
            Directory.CreateDirectory(path);
            return path;
        }
    }
}