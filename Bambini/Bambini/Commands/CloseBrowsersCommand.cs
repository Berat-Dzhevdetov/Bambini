namespace Bambini.Commands
{
    using System.Diagnostics;
    using Bambini.Interfaces;

    public class CloseBrowsersCommand : ICommand
    {
        public string Phrase => "close browsers";
        private readonly WindowsHelper windowsHelper;

        public CloseBrowsersCommand(WindowsHelper windowsHelper)
        {
            this.windowsHelper = windowsHelper;
        }

        public void Execute()
        {
            var result = windowsHelper.DefaultBrowser.Substring(windowsHelper.DefaultBrowser.LastIndexOf('\\') + 1);
            if (result.EndsWith(".exe"))
            {
                result = result.Substring(0, result.IndexOf('.'));
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
