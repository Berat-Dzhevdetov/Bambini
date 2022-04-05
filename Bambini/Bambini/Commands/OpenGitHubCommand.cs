namespace Bambini.Commands
{
    using System.Diagnostics;
    using Bambini.Interfaces;

    public class OpenGitHubCommand : ICommand
    {
        public string Phrase => "open github";
        private readonly WindowsHelper windowsHelper;

        public OpenGitHubCommand(WindowsHelper windowsHelper)
        {
            this.windowsHelper = windowsHelper;
        }

        public void Execute()
        {
            Process.Start(windowsHelper.DefaultBrowser, "https://github.com/Berat-Dzhevdetov");
        }
    }
}
