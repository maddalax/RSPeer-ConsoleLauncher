using System;
using System.Threading.Tasks;

namespace ConsoleLauncher.Shell
{
    public class ConsoleLogger : ILogger
    {
        public Task Log(string message)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }

        public Task Log(string message, Exception exception)
        {
            Console.WriteLine(exception);
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
    }
}