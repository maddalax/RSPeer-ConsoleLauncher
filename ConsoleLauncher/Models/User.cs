namespace ConsoleLauncher.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int Balance { get; set; }
        public string[] GroupNames { get; set; }
    }
}