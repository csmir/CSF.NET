![csfbanner_lighttrans_outline](https://github.com/csmir/CSF.NET/assets/68127614/7255e535-41b6-431c-87f2-2d9aa18ef6f9)

# CSF.NET - Command Standardization Framework for .NET

CSF is an attribute based framework that makes creating and processing **text based commands** easy for any platform. It implements a modular, easy to implement pipeline for registering and executing commands, as well as a wide range of customization options to make development on different platforms as easy as possible.

- [Features](#features)
- [Additional Packages](#additional-packages)
- [Getting Started](#getting-started)

## Features

#### Type Conversion

For raw input, automated conversion to fit command signature is supported by `TypeConverter`'s. `ValueType`, `Enum` and nullable variant types are automatically parsed by the framework and populate commands as below:

```cs
...
[Command("test")]
public void Test(int param1, DateTime param2)
{
    Console.WriteLine("{0}, {1}", param1, param2);
}
...
```
- This will automatically parse `int` by using the default `int.TryParse` implementation, and will do the same for the `DateTime`.

Outside of this, implementing and adding your own `TypeConverter`'s is also supported to handle command signatures with normally unsupported types.

> See feature [documentation](https://github.com/csmir/CSF.NET/wiki/Type-Conversion) for more.

#### Preconditions

Implementing `PreconditionAttribute` creates a new evaluation to add in the set of attributes defined above command definitions. 
When a command is attempted to be executed, it will walk through every precondition present and abort execution if any of them fail.

```cs
...
[CustomPrecondition]
[Command("test")]
public async Task Test()
{
    
}
...
```

> See feature [documentation](https://github.com/csmir/CSF.NET/wiki/Preconditions) for more.

#### Dependency Injection

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

#### Informative Results

CSF.NET will return results for running commands through a `ResultResolver`. This resolver has a default implementation that can be configured through the `CommandConfiguration`

```cs
...
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

#### Customization

While already fully functional out of the box, the framework does not shy away from covering extensive applications with more specific needs, which in turn need more than the base features to function according to its developer's expectations. 

Types such as `CommandContext`, `ModuleBase`, `TypeConverter`, `PreconditionAttribute` and `Parser` can all be inherited and custom ones created for environmental specifics, custom type conversion and more.

#### Reflection

The framework saves cached command data in its own reflection types. 
These types, such as `CommandInfo`, `ArgumentInfo` and `ModuleInfo` store informative data about a command, its root module and any submembers.

The reflection data is accessible in various ways, most commonly in scope during type conversion & precondition evaluation.

## Additional Packages

CSF is not without its own dependencies, but it tries its best to keep all dependencies within a trusted atmosphere, using packages only when they outweigh self-written implementations. So far, it only depends on packages published by official channels of the .NET ecosystem.

#### Dependency Injection

Having grown into a vital part of building effective and modern applications, Dependency Injection (DI) is no less important to be carried along in the equally modern CSF. 
It integrates this feature deeply into its architecture and depends on it to function from the ground up. 

For applications to function with `CSF.Core`, it is necessary to install DI functionality through Microsoft's publicized package(s):

```xml
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="" />
```
> This and other required packages can also be installed through the Package Manager, .NET CLI or from within your IDE.

#### Hosting

Carrying out further support within the .NET ecosystem, CSF also introduces a hosting package for deploying apps with the .NET generic host. 

For applications to function with `CSF.Hosting`, it is necessary to install the hosting package that it also implements, also publicized by Microsoft itself:

```xml
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="" />
```

> The hosting extensions package publicized by Microsoft implements the packages necessary for the core component of CSF, and does not expect to have its dependencies implemented alongside it.

*For each of these packages, the minimum version is determined by CSF itself, usually being the latest or equal to the target framework upon which it was released. It is suggested to choose the latest version at time of installation.*

## Getting Started

There are various resources available in order to get started with CSF. Below, you can find samples and directions to the quick guide.

#### Quick Guide

You can find the quick guide [here](https://github.com/csmir/CSF.NET/wiki/Quick-Guide). 
This guide introduces you to the basics of defining modules, commands, and how to run them.

#### Samples

Samples are available to learn how to implement CSF in your own programs.

- [CSF.Samples.Console](https://github.com/csmir/CSF.NET/tree/master/examples/CSF.Samples.Console)
  - Shows how to implement CSF on a basic console application.
- [CSF.Samples.Hosting](https://github.com/csmir/CSF.NET/tree/master/examples/CSF.Samples.Console)
  - Shows how to implement CSF on a hosted console application.
