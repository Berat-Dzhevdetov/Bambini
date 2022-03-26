namespace Bambini.Commands
{
    using System.Diagnostics;
    using Bambini.Interfaces;

    public class CloseBrowsersCommand : CommandMain, ICommand
    {
        public CloseBrowsersCommand()
        {

        }

        public string Phrase => "close browsers";

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
