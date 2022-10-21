namespace Bambini.Services
{
    using Bambini.Services.DependencyResolvers;
    using Bambini.Services.Extension;
    using Bambini.Services.Interfaces;
    using Bambini.Services.LoggerService;
    using Bambini.Services.SpeechServices;
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
        #endregion

        #region Properties
        public DependencyResolver DependencyResolver { get; }
        #endregion

        #region Constructors
        public BambiniMain()
        {
            FULL_PHRASE = $"{CALL_WORD.ToLower()} {APP_NAME.ToLower()}";
            commands = new List<ICommand>();
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
            // Create a SpeechRecognitionEngine object for the default recognizer for the machine's current culture.
            recognizer = new SpeechRecognitionEngine(CultureInfo.CurrentCulture);

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

            var commandsThatFailedToLoad = new List<string>();

            foreach (var command in commandTypes)
            {
                try
                {
                    ICommand commandToAdd;

                    if (command == null) continue;

                    var constructors = command.GetConstructors();

                    if (constructors.Length != 1)
                    {
                        throw new InvalidDataException($"Found more than one constructor in '{command.GetType().FullName}'. Not sure which one to take.");
                    }

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

                    if (commands.Any(x => x.Phrase.ToLower() == commandToAdd.Phrase.ToLower()))
                    {
                        throw new InvalidDataException($"Found two commands with same phrase: '{commandToAdd.Phrase}'");
                    }

                    commands.Add(commandToAdd);

                    Console.Clear();
                    Console.WriteLine($"Loaded {commands.Count}/{commandTypes.Length} command(s)");
                }
                catch (TargetInvocationException ex)
                {
                    Console.WriteLine(ex.InnerException.Message);
                    Log.Write(ex.InnerException);
                    Environment.Exit(1);
                }
                catch (Exception ex)
                {
                    commandsThatFailedToLoad.Add($"{command.Name} given message: {ex.Message}");
                    Log.Write(ex);
                }
                finally
                {
                    if (commandsThatFailedToLoad.Any())
                    {
                        Console.WriteLine($"Couldn't load:\n{string.Join("\n", commandsThatFailedToLoad)}");
                        Console.WriteLine("Check error logs for more informaiton");
                    }
                }
            }
            commandsThatFailedToLoad = null;
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

        private async void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine($"Recognized: {e.Result.Text}");
            if (e.Result.Text.StartsWith(FULL_PHRASE))
            {
                var token = e.Result.Text[(FULL_PHRASE.Length + 1)..];

                var command = commands.FirstOrDefault(x => x.Phrase == token);

                try
                {
                    var commandTask = Task.Factory.StartNew(command.Execute);

                    await commandTask;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{command} failed to run: {ex.Message}");
                    Console.WriteLine("Check the logs for more information");
                    ex.SetCommand(command);
                    Log.Write(ex);
                }
            }
        }

        private void LoadDependencies()
        {
            DependencyResolver.Add<IWindowsHelper, WindowsHelper>();
            DependencyResolver.Add<ISpeech, Speech>();
        }
        #endregion
    }
}
