namespace Bambini.Commands
{
    using Bambini.Services.Interfaces;
    using Bambini.Services.WindowsHelper;
    using System.Diagnostics;

    public class OpenLeagueOfLegends : ICommand
    {
        public string Phrase => "open league of legends";

        public void Execute()
        {
            var workers = Process.GetProcessesByName("LeagueClient");

            if(workers.Length >= 0)
            {
                // tell the user that league is already open
                return;
            }
            foreach (Process worker in workers)
            {
                worker.Kill();
                worker.WaitForExit();
                worker.Dispose();
            }
            Process.Start("D:\\League Of Legends\\Riot Games\\Riot Client\\RiotClientServices.exe", "--launch-product=league_of_legends --launch-patchline=live");
        }
    }
}
