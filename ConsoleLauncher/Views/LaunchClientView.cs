using System;
using System.Threading.Tasks;

namespace ConsoleLauncher.Views
{
    public class LaunchClientView : IView
    {
        public async Task<bool> Validate()
        {
            return true;
        }

        public Task Execute()
        {
           Console.WriteLine("What would you like to do?");
           Console.WriteLine("(1) Launch Runescape 2007 Client.");
           Console.WriteLine("(2) Launch Runescape RS3 Client.");
           Console.WriteLine("Type an option, such as '1'.");
           var option = Console.ReadLine();
           Console.WriteLine(option);
           return Task.CompletedTask;
        }
    }
}