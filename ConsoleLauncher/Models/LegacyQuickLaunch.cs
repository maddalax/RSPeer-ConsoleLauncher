namespace ConsoleLauncher.Models
{
    public class LegacyQuickLaunch
    {
        public string RsUsername { get; }
        public string RsPassword { get; }
        public string World { get; }
        public string ScriptName { get; }
        public bool IsRepoScript { get; }
        public string ScriptArgs { get; }
        public bool UseProxy { get; }
        public string ProxyPort { get; }
        public string ProxyIp { get; }
        public string ProxyUser { get; }
        public string ProxyPass { get; }
        public Config Config { get; }
        
        public Game Game { get; }

        public LegacyQuickLaunch(Client client)
        {
            RsUsername = client.RsUsername;
            RsPassword = client.RsPassword;
            World = client.World != "0" ? client.World : "-1";
            ScriptName = client.Script?.Name ?? client.ScriptName;
            IsRepoScript = client.Script?.IsRepoScript ?? client.IsRepoScript;
            ScriptArgs = client.Script?.ScriptArgs ?? client.ScriptArgs ?? string.Empty;
            UseProxy = client.Proxy != null ? client.Proxy?.Ip != null : client.ProxyIp != null;
            ProxyPort = client.Proxy?.Port ?? client.ProxyPort;
            ProxyIp = client.Proxy?.Ip ?? client.ProxyIp;
            ProxyUser = client.Proxy?.Username ?? client.ProxyUser;
            ProxyPass = client.Proxy?.Password ?? client.ProxyPass;
            Config = client.Config;
            Game = client.Game;
        }
    }
}