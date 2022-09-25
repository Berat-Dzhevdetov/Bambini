namespace Bambini.Services.SpeechServices
{
    using System.Speech.Synthesis;

    internal class Speech : ISpeech
    {
        #region Private fields
        private readonly SpeechSynthesizer speechSynthesizer;
        #endregion

        #region Constructors
        public Speech()
        {
            speechSynthesizer = new();
            speechSynthesizer.SetOutputToDefaultAudioDevice();
            speechSynthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Teen);
        }
        #endregion

        #region Public methods
        public void Say(string value)
        {
            speechSynthesizer.Speak(value);
        }
        #endregion
    }
}
