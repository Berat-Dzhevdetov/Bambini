namespace Bambini.Interfaces
{
    public interface ICommand
    {
        public string Phrase { get; }
        void Execute();
    }
}
