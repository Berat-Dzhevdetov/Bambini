namespace Bambini.Commands
{
    using System.Diagnostics;
    using Bambini.Interfaces;

    public class OpenYoutubeCommand : CommandMain, ICommand
    {
        public string Phrase => "open youtube";

        public void Execute()
        {
            Process.Start(windowsHelper.DefaultBrowser, "https://www.youtube.com/");
        }
    }
}
