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
            var folder = game == Game.Osrs ? "RSPeer" : "RSPeer Inuvation";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                return Path.Join(documents, folder);
            }
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Join(home, folder);
        }
    }
}