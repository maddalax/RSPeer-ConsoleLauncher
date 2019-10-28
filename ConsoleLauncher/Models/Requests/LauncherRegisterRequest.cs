namespace ConsoleLauncher.Models.Requests
{
    public class LauncherRegisterRequest
    {
        public int UserId { get; set; }
        public string Tag { get; set; }
        public string Ip { get; set; }
        public string MachineUsername { get; set; }
        public string Platform { get; set; }
        public string Host { get; set; }
    }
}