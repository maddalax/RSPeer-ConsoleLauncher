namespace ConsoleLauncher.Providers
{
    public class AppArgProvider : IAppArgProvider
    {
        public string[] Args { get; }
        public string Email { get; set; }
        public string Password { get; set; }
        
        public AppArgProvider(string[] args)
        {
            Args = args;
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (args.Length >= 2 && arg == "-email")
                {
                    Email = args[i + 1];
                }
                if (args.Length >= 2 && arg == "-password")
                {
                    Password = args[i + 1];
                }
            }
        }
    }
}