namespace Bambini.Services.LoggerService
{
    using System;
    using System.Globalization;
    using System.Text;

    public static class Log
    {
        #region Fields
        private static readonly string logFileName = "Errors.txt";
        private static readonly int defaultCountRepeating = 40;
        #endregion

        #region Public methods
        public static void Write(Exception exception)
        {
            var sb = new StringBuilder();
            var currentCulture = CultureInfo.CurrentCulture;

            sb.AppendLine(new string('-', defaultCountRepeating));

            sb.AppendLine("");
            sb.AppendLine(DateTime.UtcNow.ToString(currentCulture));
            sb.AppendLine("");
            sb.AppendLine(new string('-', defaultCountRepeating));
            sb.AppendLine($"Timestamp: {DateTime.Now.ToString(currentCulture)}");
            sb.AppendLine($"Type: {exception.GetType().Name}");
            sb.AppendLine($"Message: {exception.Message}");
            sb.AppendLine($"Stack Trace: {exception.StackTrace?.Trim() ?? "The stack trace is unavailable"}");
            sb.AppendLine($"Command: {exception.Data[Constants.CommandExceptionKey] ?? "Unknown"}");
            sb.AppendLine(new string('-', defaultCountRepeating));

            try
            {
                File.AppendAllText(logFileName, sb.ToString());
            }
            catch (IOException)
            {
                Console.WriteLine("Couldn't write to the file because it is open from another process");
                Console.WriteLine(sb.ToString());
            }
        }
        #endregion

        #region Private methods
        #endregion
    }
}