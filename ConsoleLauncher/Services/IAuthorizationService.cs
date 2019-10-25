using System.Threading.Tasks;

namespace ConsoleLauncher.Services
{
    public interface IAuthorizationService
    {
        Task WriteSession(string session);
        Task<string> GetSession();
    }
}