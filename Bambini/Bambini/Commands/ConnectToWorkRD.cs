namespace Bambini.Commands
{
    using Bambini.DTOs;
    using Bambini.Services.Interfaces;
    using Bambini.Services.JsonHelpers;
    using Bambini.Services.SpeechServices;
    using Bambini.Services.WindowsHelpers;
    using Newtonsoft.Json;
    using System;

    public class ConnectToWorkRD : ICommand
    {
        private readonly ISpeech speech;
        private readonly IWindowsHelper windowsHelper;

        public ConnectToWorkRD(ISpeech speech, IWindowsHelper windowsHelper)
        {
            this.speech = speech;
            this.windowsHelper = windowsHelper;
        }

        public string Phrase => "connect to work";

        public void Execute()
        {
            var filename = "Information.json";
            var message = string.Empty;
            if (!File.Exists(filename))
            {
                message = $"The given file doesn't exist '{filename}'";
                Console.WriteLine(message);
                speech.Say(message);
                return;
            }

            var fileValue = File.ReadAllText(filename);
            var fieldName = "WorkRemoteDesktop";
            var json = JsonHelper.GetField(fileValue, fieldName);

            if (json == null)
            {
                message = $"Couldn't get field '{fieldName}' in '{filename}'";
                Console.WriteLine(message);
                speech.Say(message);
                return;
            }

            var data = JsonConvert.DeserializeObject<ConnectoToVPNDTO>(json);

            // The following command is used to connect to given VPN
            var vpnArguments = $"/c rasdial \"{data.VPNName}\" \"{data.Username}\" \"{data.Password}\"";

            var connectToVpn = windowsHelper.ExecuteCommand("cmd.exe", vpnArguments);
            if (connectToVpn)
            {
                message = "Successfully connected to the VPN";
            }
            else
            {
                message = "Something went wrong when trying to connect to the VPN";
            }
            Console.WriteLine(message);
            speech.Say(message);

            if (!connectToVpn) return;

            // Open the Remote Desktop and connect
            var rdArguments = $"/c mstsc /v:{data.IP}";

            var openRd = windowsHelper.ExecuteCommand("cmd.exe", rdArguments);
            if (openRd)
            {
                message = "Successfully opened remote desktop";
            }
            else
            {
                message = "Something went wrong when trying to open the Remote Desktop";
            }

            Console.WriteLine(message);
            speech.Say(message);
        }
    }
}
