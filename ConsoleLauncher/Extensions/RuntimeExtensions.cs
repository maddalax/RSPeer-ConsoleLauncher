using System.Runtime.InteropServices;

namespace ConsoleLauncher.Extensions
{
    public class RuntimeExtensions
    {
        public static string RuntimeToString()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "Linux";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "Windows";
            }
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "Mac OS X" : "Unknown Runtime";
        }
    }
}