namespace ConsoleLauncher.Models
{
    public class LaunchOptions
    {
        public Game Game { get; set; }
        public string JvmArgs { get; set; }
        public int Count { get; set; }
        public int Sleep { get; set; }
        public QuickLaunch Qs { get; set; }
    }
}