namespace Bambini.Commands
{
    using Bambini.Interfaces;
    using System.Diagnostics;

    public class OpenYoutubeCommand : ICommand
    {
        public string Phrase => "open youtube";
        private readonly WindowsHelper windowsHelper;

        public OpenYoutubeCommand(WindowsHelper windowsHelper)
        {
            this.windowsHelper = windowsHelper;
        }

        public void Execute()
        {
            Process.Start(windowsHelper.DefaultBrowser, "https://www.youtube.com/");
        }
    }
}
