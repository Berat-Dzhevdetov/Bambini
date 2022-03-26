namespace Bambini.Commands
{
    using System.Diagnostics;
    using Bambini.Interfaces;

    public class OpenGitHubCommand : CommandMain, ICommand
    {
        public string Phrase => "open github";
        public void Execute()
        {
            Process.Start(windowsHelper.DefaultBrowser, "https://www.youtube.com/");
        }
    }
}
