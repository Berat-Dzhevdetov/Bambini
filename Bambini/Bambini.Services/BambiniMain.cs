namespace Bambini.Services
{
    using Bambini.Services.DependencyResolvers;
    using Bambini.Services.Interfaces;
    using Bambini.Services.WindowsHelpers;
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Speech.Recognition;

    public class BambiniMain
    {
        #region Fields
        private const string CALL_WORD = "Hey";
        private const string APP_NAME = "Bambini";
        private readonly string FULL_PHRASE;
        private SpeechRecognitionEngine recognizer;
        private readonly List<ICommand> commands;
        private readonly DependencyResolver dependencyResolver;
        #endregion

        #region Properties
        public DependencyResolver DependencyResolver
        {
            get
            {
                return dependencyResolver;
            }
            init
            {
                dependencyResolver = value;
            }
        }
        #endregion

        #region Constructors
        public BambiniMain()
        {
            FULL_PHRASE = $"{CALL_WORD.ToLower()} {APP_NAME.ToLower()}";
            commands = new List<ICommand>();
            //windowsHelper = new WindowsHelper();
            DependencyResolver = new DependencyResolver();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Runs the program and start listening
        /// </summary>
        public void Run()
        {
            LoadDependencies();
            LoadCommands();
            LoadSpeechRecognition();

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
                    .GetEntryAssembly()
                    ?.GetTypes()
                    .Where(t => typeof(ICommand).IsAssignableFrom(t) && t.IsClass)
                    .ToArray();

            if (commandTypes == null) return;

            foreach (var command in commandTypes)
            {
                ICommand? commandToAdd;

                if (command == null) continue;

                var constructors = command.GetConstructors();

                if (constructors.Length != 1) throw new InvalidDataException($"Found more or less constructors in '{command.FullName}' command");

                var constructor = constructors.FirstOrDefault();

                var parameters = constructor.GetParameters();

                if (parameters.Length >= 1)
                {
                    var resolvedParameters = new List<object>();

                    foreach (var parameter in parameters)
                    {
                        var parameterType = parameter.ParameterType;

                        MethodInfo method = DependencyResolver.GetType().GetMethod(nameof(DependencyResolver.Get), BindingFlags.NonPublic | BindingFlags.Instance)
                                     .MakeGenericMethod(new Type[] { parameterType });

                        var resolvedParameter = method.Invoke(DependencyResolver, Array.Empty<object>());
                        resolvedParameters.Add(resolvedParameter);
                    }

                    commandToAdd = (ICommand)constructor.Invoke(resolvedParameters.ToArray());
                }
                else
                {
                    commandToAdd = (ICommand)Activator.CreateInstance(command);
                }

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
            DependencyResolver.Add<IWindowsHelper, WindowsHelper>();
        }
        #endregion
    }
}
