using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ConsoleLauncher.Extensions
{
    public class ProcessExtensions
    {
        public static void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                try
                {
                    // hack because of this: https://github.com/dotnet/corefx/issues/10361
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        url = url.Replace("&", "^&");
                        Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") {CreateNoWindow = true});
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        Process.Start("xdg-open", url);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        Process.Start("open", url);
                    }
                    else
                    {
                        Console.WriteLine("Failed to open: " + url);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to open url.");
                    Console.WriteLine(url);
                }
            }
        }
    }
}