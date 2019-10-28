using System;
using System.Text.Json;

namespace ConsoleLauncher.Models.Responses
{
    public class RemoteMessage
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Consumer { get; set; }
        public JsonElement Message { get; set; }
        public string Source { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}