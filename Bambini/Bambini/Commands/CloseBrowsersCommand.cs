namespace Bambini.Commands
{
    using System.Diagnostics;
    using Bambini.Services.Interfaces;
    using Bambini.Services.WindowsHelpers;

    public class CloseBrowsersCommand : ICommand
    {
        public string Phrase => "close browsers";
        private readonly IWindowsHelper windowsHelper;

        public CloseBrowsersCommand(IWindowsHelper windowsHelper)
        {
            this.windowsHelper = windowsHelper;
        }

        public void Execute()
        {
            var result = windowsHelper.DefaultBrowser[(windowsHelper.DefaultBrowser.LastIndexOf('\\') + 1)..];
            if (result.EndsWith(".exe"))
            {
                result = result[..result.IndexOf('.')];
            }
            Process[] workers = Process.GetProcessesByName(result);
            foreach (Process worker in workers)
            {
                worker.Kill();
                worker.WaitForExit();
                worker.Dispose();
            }
        }
    }
}
