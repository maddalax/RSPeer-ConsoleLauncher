using System.IO;
using System.Threading.Tasks;

namespace ConsoleLauncher.Services
{
    public interface IApiService
    {
        Task<T> Get<T>(string path);
        Task<Stream> GetStream(string path);
        Task<string> GetString(string path);
        Task<T> Post<T>(string path, object body);
    }
}