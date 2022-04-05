namespace Bambini
{
    using System;

    public class CommandMain
    {
        protected readonly WindowsHelper windowsHelper;
        public virtual string Phrase => throw new NotImplementedException();

        public CommandMain(WindowsHelper windowsHelper)
        {
            this.windowsHelper = windowsHelper;
        }

        public virtual void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
