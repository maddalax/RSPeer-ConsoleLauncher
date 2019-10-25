using System;
using System.Threading.Tasks;

namespace ConsoleLauncher.Shell
{
    public interface ILogger
    {
        Task Log(string message);
        Task Log(string message, Exception exception);
    }
}