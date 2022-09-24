namespace Bambini.Services
{
    using System;
    using System.Speech.Recognition;
    using System.Globalization;
    using System.Reflection;
    using Bambini.Services.Interfaces;

    public class BambiniMain
    {
        #region Fields
        private const string CALL_WORD = "Hey";
        private const string APP_NAME = "Bambini";
        private string FULL_PHRASE;
        private SpeechRecognitionEngine recognizer;
        private List<ICommand> commands;
        private readonly DependencyResolver dependencyResolver;
        #endregion

        #region Constructors
        public BambiniMain()
        {
            FULL_PHRASE = $"{CALL_WORD.ToLower()} {APP_NAME.ToLower()}";
            commands = new List<ICommand>();
            //windowsHelper = new WindowsHelper();
            dependencyResolver = new DependencyResolver();
        }
        #endregion

        #region Public methods
        public void Run()
        {
            LoadCommands();
            LoadSpeechRecognition();
            LoadDependencies();

            var a = dependencyResolver.Get<WindowsHelper>();

            Console.WriteLine("Listening!");

            // Keep the console window open.
            while (true)
            {
                Console.ReadLine();
            }
        }
        #endregion

        #region Private methods
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

        private void LoadCommands()
        {
            var commandTypes = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => typeof(ICommand).IsAssignableFrom(t) && t.IsClass)
                    .ToArray();

            foreach (var command in commandTypes)
            {
                ICommand? commandToAdd;

                if (command == null) continue;

                //commandToAdd = Activator.CreateInstance(command, new WindowsHelper("asd")) as ICommand;

                if (commandToAdd == null) continue;

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
                var token = e.Result.Text[(FULL_PHRASE.Length + 1)..];

                var command = commands.FirstOrDefault(x => x.Phrase == token);

                command.Execute();
            }
        }

        private void LoadDependencies()
        {
            dependencyResolver.Add<WindowsHelper>();
        }
        #endregion
    }
}
