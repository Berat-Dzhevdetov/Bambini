namespace Bambini.Commands
{
    using Bambini.Services.Interfaces;
    using Bambini.Services.SpeechServices;
    using System.Diagnostics;

    public class OpenLeagueOfLegends : ICommand
    {
        private readonly ISpeech speech;

        public OpenLeagueOfLegends(ISpeech speech)
        {
            this.speech = speech;
        }

        public string Phrase => "open league of legends";

        public void Execute()
        {
            var workers = Process.GetProcessesByName("LeagueClient");

            if(workers.Length > 0)
            {
                var repsondMessage = "Your League of Legends client is already opened";
                Console.WriteLine(repsondMessage);
                speech.Say(repsondMessage);
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
