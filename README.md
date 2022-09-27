# Bambini

### Bambini is voice assistant (windows only)

##### You can make you own commands that can be executed when you say "Hey Bambini {your command phrase}".
##### You can use your services just add them to the Dependency resolver and when a command needs the class to run it will pass it to the constructor.
##### To register your own command just inherit ICommand interface
###### If you have ideas make PR and let's have a discussion about it.

### Examples
#### Adding your custom service to the dependency resolver
```cs
using Bambini.Services;

var bambini = new BambiniMain();

bambini.DependencyResolver.Add<IMyCustomService, MyCustomService>();

bambini.Run();
```
#### Make sure to add the dependencies before calling ``` Run ``` method.
-----
#### Creating your own command
```cs
namespace Bambini.Commands
{
    using Bambini.Services.Interfaces;
    using Bambini.Services.WindowsHelpers;

    public class OpenGitHubCommand : ICommand
    {
        public string Phrase => "open github";
        private readonly IWindowsHelper windowsHelper;

        public OpenGitHubCommand(IWindowsHelper windowsHelper)
        {
            this.windowsHelper = windowsHelper;
        }

        public void Execute()
        {
            windowsHelper.ExecuteCommand(windowsHelper.DefaultBrowser, "https://github.com/Berat-Dzhevdetov");
        }
    }
}
```
#### Create a public class that inherits ```ICommand``` interface.
#### ```Phrase``` is used to recognize a command when the voice assistant is working. So in this case to execute the code simply just say "Hey bambini open github"
#### When the program recognizes the phrase it will call the ```Execute``` method which in this case opens the default system browser with link "https://github.com/Berat-Dzhevdetov".
-----
