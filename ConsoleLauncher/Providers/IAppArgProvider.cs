namespace ConsoleLauncher.Providers
{
    public interface IAppArgProvider
    {
        string[] Args { get; }
        string Email { get; set; }
        string Password { get; set; }
    }
}