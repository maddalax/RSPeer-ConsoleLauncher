namespace ConsoleLauncher.Models.Responses
{
    public class BotPanelUserRequest
    {
        public string Type { get; set; }
        public string Session { get; set; }
        public QuickLaunch Qs { get; set; }
        public string JvmArgs { get; set; }
        public int Sleep { get; set; }
        public Proxy Proxy { get; set; }
        public Game Game { get; set; }
        public int Count { get; set; } = 1;
    }
}