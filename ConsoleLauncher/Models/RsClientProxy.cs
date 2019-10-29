using System;

namespace ConsoleLauncher.Models
{
    public class RsClientProxy
    {
        public string ProxyId { get; set; }
        public DateTimeOffset Date { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}