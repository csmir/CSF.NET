![csfbanner_lighttrans_outline](https://github.com/csmir/CSF.NET/assets/68127614/7255e535-41b6-431c-87f2-2d9aa18ef6f9)

# 🏗️ CSF.NET - Command Standardization Framework for .NET

CSF is an attribute based framework that makes creating and processing **text based commands** easy for any platform. It implements a modular, easy to implement pipeline for registering and executing commands, as well as a wide range of customization options to make development on different platforms as easy as possible.

## 📍 Features

### 🗨️ Type Conversion:

`ValueType`, `Enum` and nullable variant types are automatically parsed by the library and populate commands as below:

```cs
...
[Command("test")]
public void Test(int param1, DateTime param2)
{
    Console.WriteLine("{0}, {1}", param1, param2);
}
...
```
> This will automatically parse `int` by using the default `int.TryParse` implementation, and will do the same for the `DateTime`.

Outside of this, implementing and adding your own `TypeConverter`'s is also supported to handle commands with your own type signature. Nullability is automatically resolved by the library.

> See feature [documentation](https://github.com/csmir/CSF.NET/wiki/Type-Conversion) for more.

### 🛑 Preconditions:

Implementing `PreconditionAttribute` creates a new evaluation to add in the set of attributes defined above command definitions. 
When a command is attempted to be executed, it will walk through every precondition present and abort execution if any of them fail.

```cs
...
[MyPrecondition(PlatformID.Unix)]
[Command("copy")]
public async Task Handle()
{
    
}
...
```

> See feature [documentation](https://github.com/csmir/CSF.NET/wiki/Preconditions) for more.

### 💡 Customization:

`CommandContext` and `ModuleBase` can each be implemented in your own ways, them serving as extensible carriers for command-level data. You can add a number of application-unique properties that are populated at creation.

```cs
...
class MyContext(User user) : CommandContext
{
    public User CommandUser { get; } = user;
}
...
```

> See feature [documentation](https://github.com/csmir/CSF.NET/wiki/Modules-And-Contexts) for more.

### 💉 Dependency Injection:

You can provide an `IServiceProvider` at execution to inject modules with dependencies, in accordance to the conventions `Microsoft.Extensions.DependencyInjection` follows. The `IServiceProvider` has a number of extensions that are suggested to be used when writing your codebase with CSF. These extensions serve you and the program, reducing boilerplate in the application setup.

```cs
...
var services = new ServiceCollection()
    .ConfigureCommands(configuration =>
    {
        configuration.WithAssemblies(Assembly.GetEntryAssembly());
    });
...
```

> See feature [documentation](https://github.com/csmir/CSF.NET/wiki/Dependency-Injection) for more.

### 📖 Informative Results:

CSF.NET will return results for running commands through a `ResultResolver`. This resolver has a default implementation that can be configured through the `CommandConfiguration`

```cs
...
var configuration = new CommandConfiguration();

configuration.ConfigureResultAction(async (context, result, services) =>
{
    if (result.Success)
    {
        await Task.CompletedTask;
    }
    else
    {
        Console.WriteLine(result.Exception);
    }
});
...
```


> See feature [documentation](https://github.com/csmir/CSF.NET/wiki/Handling-Results) for more.

### 📄 Reflection:

The framework saves cached command data in its own reflection types. 
These types, such as `CommandInfo` `ArgumentInfo` and `ModuleInfo` store informative data about a command, its root module and any submembers.

The reflection data is accessible in various ways, most commonly in scope during type conversion & precondition evaluation.

## 🤖 Samples

Samples are available to learn how to implement CSF in your own programs.

- [CSF.Samples.Console](https://github.com/Rozen4334/CSF.NET/tree/master/examples/CSF.Samples.Console)
  - Shows how to implement CSF on a basic console application.
- [CSF.Samples.Hosting](https://github.com/Rozen4334/CSF.NET/tree/master/examples/CSF.Samples.Console)
  - Shows how to implement CSF on a hosted console application.

## 📰 Extensions

CSF introduces a number of extensions for external libraries.

- [CSF.Hosting](https://github.com/Rozen4334/CSF.NET/tree/master/src/CSF.Hosting)
  - A package that wraps around [Microsoft.Extensions.Hosting](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host?tabs=appbuilder).
