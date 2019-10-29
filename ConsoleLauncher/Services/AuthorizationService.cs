using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ConsoleLauncher.Extensions;
using ConsoleLauncher.Models;

namespace ConsoleLauncher.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IFileService _file;

        public AuthorizationService(IFileService file)
        {
            _file = file;
        }

        public async Task WriteSession(string session)
        {
            var cache = _file.GetCacheFolder(Game.Osrs);
            var cacheInu = _file.GetCacheFolder(Game.Rs3);
            var file = Path.Join(cache, "misc_new");
            var fileInu = Path.Join(cacheInu, "misc_new");
            var xor = HashExtensions.Xor(session, GetKey());
            await File.WriteAllBytesAsync(file, xor);
            await File.WriteAllBytesAsync(fileInu, xor);
        }

        public async Task<string> GetSession()
        {
            var cache = _file.GetCacheFolder(Game.Osrs);
            var file = Path.Join(cache, "misc_new");
            var text = File.Exists(file) ? await File.ReadAllTextAsync(file) : null;
            return string.IsNullOrEmpty(text) ? null : Encoding.Default.GetString(HashExtensions.Xor(text, GetKey()));
        }

        private string GetKey()
        {
            var username = Environment.UserName;
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return $"{username}{home}";
        }
    }
}