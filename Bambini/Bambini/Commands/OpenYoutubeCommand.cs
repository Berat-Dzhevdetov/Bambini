namespace Bambini.Commands
{
    using Bambini.Services.Interfaces;
    using Bambini.Services.WindowsHelpers;
    using System.Diagnostics;

    public class OpenYoutubeCommand : ICommand
    {
        public string Phrase => "open youtube";
        private readonly IWindowsHelper windowsHelper;

        public OpenYoutubeCommand(IWindowsHelper windowsHelper)
        {
            this.windowsHelper = windowsHelper;
        }

        public void Execute()
        {
            Process.Start(windowsHelper.DefaultBrowser, "https://www.youtube.com/");
        }
    }
}
