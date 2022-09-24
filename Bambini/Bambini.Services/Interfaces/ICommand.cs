namespace Bambini.Services.Interfaces
{
    public interface ICommand
    {
        string Phrase { get; }
        void Execute();
    }
}
