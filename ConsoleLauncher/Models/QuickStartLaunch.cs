namespace ConsoleLauncher.Models
{
    public class QuickLaunch
    {
        public Client[] Clients { get; set; }
    }

    public class Client
    {
        public Game Game { get; set; }
        public string RsUsername { get; set; }
        public string RsPassword { get; set; }
        public long? World { get; set; }
        public string ProxyIp { get; set; }
        public int? ProxyPort { get; set; }
        public string ProxyUser { get; set; }
        public string ProxyPass { get; set; }
        public Proxy Proxy { get; set; }
        public bool IsRepoScript { get; set; }
        public string ScriptArgs { get; set; }
        public string ScriptName { get; set; }
        public Script Script { get; set; }
        public Config Config { get; set; }
    }

    public class Config
    {
        public bool LowCpuMode { get; set; }
        public bool SuperLowCpuMode { get; set; }
        public long? EngineTickDelay { get; set; }
        public bool DisableModelRendering { get; set; }
        public bool DisableSceneRendering { get; set; }
    }

    public class Proxy
    {
        public string Ip { get; set; }
        public int? Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Script
    {
        public string ScriptArgs { get; set; }
        public string Name { get; set; }
        public bool IsRepoScript { get; set; }
    }
}