using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleLauncher.Extensions
{
    public static class ConsoleExtensions
    {
        public static async Task<ConsoleKeyInfo?> WaitForKey(CancellationToken token)
        {
            ConsoleKeyInfo? key = null;
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                

                if (Console.KeyAvailable)
                { 
                    var temp = Console.ReadKey();
                    if (temp.Key == ConsoleKey.Enter)
                    {
                        return key;
                    }
                    key = temp;
                }

                await Task.Delay(50, token);
            }

            return new ConsoleKeyInfo((char) 0, 0, false, false, false);
        }
    }
}