namespace Bambini.Services.WindowsHelpers
{

    public interface IWindowsHelper
    {
        public string DefaultBrowser { get; }
        bool ExecuteCommand(string filename, string arguments);
        void EnsureFile(string path);
    }
}
