using System.Threading.Tasks;

namespace ConsoleLauncher.Services
{
    public interface IMessageService
    {
        Task Register(string tag);
        Task Unregister(string tag);
        Task Consume(int message);
        Task GetMessages(string tag);
    }
}