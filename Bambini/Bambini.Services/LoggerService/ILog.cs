namespace Bambini.Services.LoggerService
{
    using System;

    internal interface ILog
    {
        void Write(Exception exception);
    }
}
