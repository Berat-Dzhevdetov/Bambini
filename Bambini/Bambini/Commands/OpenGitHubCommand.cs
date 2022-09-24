namespace Bambini.Commands
{
    using System.Diagnostics;
    using Bambini.Services.Interfaces;
    using Bambini.Services.WindowsHelpers;

    public class OpenGitHubCommand : ICommand
    {
        public string Phrase => "open github";
        private readonly IWindowsHelper windowsHelper;

        public OpenGitHubCommand(IWindowsHelper windowsHelper)
        {
            this.windowsHelper = windowsHelper;
        }

        public void Execute()
        {
            Process.Start(windowsHelper.DefaultBrowser, "https://github.com/Berat-Dzhevdetov");
        }
    }
}
