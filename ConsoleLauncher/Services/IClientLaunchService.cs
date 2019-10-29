using System.Threading.Tasks;
using ConsoleLauncher.Models;
using ConsoleLauncher.Models.Responses;

namespace ConsoleLauncher.Services
{
    public interface IClientLaunchService
    {
        Task Launch(Game game, BotPanelUserRequest request, LegacyQuickLaunch quickLaunch);
        Task Launch(BotPanelUserRequest request);
    }
}