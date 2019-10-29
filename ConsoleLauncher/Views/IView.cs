using System.Threading;
using System.Threading.Tasks;
using ConsoleLauncher.Models;

namespace ConsoleLauncher.Views
{
    public interface IView
    {
        Task<bool> Validate(CancellationToken token);
        Task Execute(CancellationToken token);
        ViewType Type { get; }
    }
}