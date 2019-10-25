using System.Threading.Tasks;

namespace ConsoleLauncher.Views
{
    public interface IView
    {
        Task<bool> Validate();
        Task Execute();
    }
}