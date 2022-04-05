namespace Bambini.Interfaces
{
    public interface ICommand
    {
        string Phrase { get; }
        void Execute();
    }
}
