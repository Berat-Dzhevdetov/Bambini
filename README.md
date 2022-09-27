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
Make sure to add the dependencies before calling ``` .Run() ``` method.
-----
