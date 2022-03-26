namespace Bambini
{
    using System;
    using System.Speech.Recognition;
    using System.Globalization;
    using System.Diagnostics;

    public class BambiniMain
    {
        private const string CALL_WORD = "Hey";
        private const string APP_NAME = "Bambini";
        private string FULL_PHRASE;
        private readonly WindowsHelper windowsHelper;

        public BambiniMain()
        {
            windowsHelper = new WindowsHelper();
            FULL_PHRASE = $"{CALL_WORD.ToLower()} {APP_NAME.ToLower()}";
        }

        private SpeechRecognitionEngine recognizer;

        public void Run()
        {
            LoadSpeechRecognition();

            // Keep the console window open.
            while (true)
            {
                Console.ReadLine();

            }
        }

        private void LoadSpeechRecognition()
        {
            // Create a SpeechRecognitionEngine object for the default recognizer in the en-US locale.
            recognizer = new SpeechRecognitionEngine(new CultureInfo("en-US"));

            var choices = GetChoices();
            var grammerBuilder = new GrammarBuilder(choices);
            var grammar = new Grammar(grammerBuilder);
            recognizer.LoadGrammar(grammar);

            recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);

            // Configure the input to the speech recognizer.
            recognizer.SetInputToDefaultAudioDevice();

            // Start asynchronous, continuous speech recognition.
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        private Choices GetChoices()
        {
            Choices myChoices = new();

            //load from Json
            myChoices.Add($"{FULL_PHRASE} open youtube");
            myChoices.Add($"{FULL_PHRASE} close browser");
           
            return myChoices;
        }

        private void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            switch (e.Result.Text)
            {
                case $"hey bambini open youtube":
                    Process.Start(windowsHelper.DefaultBrowser, "https://www.youtube.com/");
                    break;
                //case $"{FULL_PHRASE} close browser":

                //    break;
                default:
                    break;
            }

            Console.WriteLine($"Recognized: {e.Result.Text}");
        }
    }
}
