namespace Bambini
{
    using System;
    using System.Speech.Recognition;
    using System.Globalization;
    using System.Reflection;
    using Bambini.Interfaces;

    public class BambiniMain
    {
        private const string CALL_WORD = "Hey";
        private const string APP_NAME = "Bambini";
        private string FULL_PHRASE;
        private List<ICommand> commands;

        public BambiniMain()
        {
            FULL_PHRASE = $"{CALL_WORD.ToLower()} {APP_NAME.ToLower()}";
            commands = new List<ICommand>();
        }

        private SpeechRecognitionEngine recognizer;

        public void Run()
        {
            LoadCommands();
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

            Console.WriteLine("Listening!");
            Console.WriteLine("Type \"help\" for see options");
        }

        private void LoadCommands()
        {
            var commandTypes = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => typeof(ICommand).IsAssignableFrom(t) && t.IsClass)
                    .ToArray();

            foreach (var command in commandTypes)
            {
                ICommand commandToAdd = (ICommand)Activator.CreateInstance(command);
                commands.Add(commandToAdd);
            }
        }

        private Choices GetChoices()
        {
            Choices myChoices = new();

            foreach (var command in commands)
            {
                myChoices.Add($"{FULL_PHRASE} {command.Phrase}");
            }

            return myChoices;
        }

        private void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine($"Recognized: {e.Result.Text}");
            if (e.Result.Text.StartsWith(FULL_PHRASE))
            {
                var token = e.Result.Text.Substring(FULL_PHRASE.Length + 1);

                var command = commands.FirstOrDefault(x => x.Phrase == token);

                command.Execute();
            }
        }
    }
}
