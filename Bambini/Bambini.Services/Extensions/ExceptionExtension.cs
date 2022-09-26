namespace Bambini.Services.Extension
{
    using Bambini.Services.Interfaces;
    using System;

    internal static class ExceptionExtension
    {
        internal static void SetCommand(this Exception exception, ICommand command)
        {
            var commandName = command.GetType().Name;
            if (commandName.ToLower().EndsWith("command"))
            {
                commandName = commandName[0..^"command".Length];
            }
            exception.Data[Constants.CommandExceptionKey] = commandName;
        }
    }
}
