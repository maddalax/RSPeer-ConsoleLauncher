namespace ConsoleLauncher.Providers
{
    public class AppArgProvider : IAppArgProvider
    {
        public string[] Args { get; }
        
        public AppArgProvider(string[] args)
        {
            Args = args;
        }
    }
}