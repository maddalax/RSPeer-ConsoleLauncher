using System.Threading.Tasks;
using ConsoleLauncher.Models;

namespace ConsoleLauncher.Views
{
    public interface IView
    {
        Task<bool> Validate();
        Task Execute();
        ViewType Type { get; }
    }
}