using System.Threading.Tasks;

namespace ConsoleLauncher.Services
{
    public interface IMessageService
    {
        Task Poll(string tag);
        Task Dispose(string tag);
    }
}