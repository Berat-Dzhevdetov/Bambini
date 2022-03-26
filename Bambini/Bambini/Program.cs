using System.Speech.Recognition;

using (
      SpeechRecognitionEngine recognizer =
        new SpeechRecognitionEngine(
          new System.Globalization.CultureInfo("en-US")))
{

    // Create a grammar for finding services in different cities.
    Choices services = new Choices(new string[] { "restaurants", "hotels", "gas stations" });
    Choices cities = new Choices(new string[] { "Seattle", "Boston", "Dallas" });

    GrammarBuilder findServices = new GrammarBuilder("Find");
    findServices.Append(services);
    findServices.Append("near");
    findServices.Append(cities);

    // Create a Grammar object from the GrammarBuilder and load it to the recognizer.
    Grammar servicesGrammar = new Grammar(findServices);
    recognizer.LoadGrammarAsync(servicesGrammar);

    // Add a handler for the speech recognized event.
    recognizer.SpeechRecognized +=
      new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);

    // Configure the input to the speech recognizer.
    recognizer.SetInputToDefaultAudioDevice();

    // Start asynchronous, continuous speech recognition.
    recognizer.RecognizeAsync(RecognizeMode.Multiple);

    // Keep the console window open.
    while (true)
    {
        Console.ReadLine();
    }
}

// Handle the SpeechRecognized event.
static void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
{
    Console.WriteLine("Recognized text: " + e.Result.Text);
}