using System.Threading.Tasks;
using ConsoleLauncher.Models;

namespace ConsoleLauncher.Services
{
    public interface IClientLaunchService
    {
        Task Launch(Game game);
    }
}