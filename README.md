# 🏗️ CSF.NET - Command Standardization Framework for .NET

CSF is an attribute based framework that makes creating and processing text based commands easy for any platform. It implements a modular, easy to implement pipeline for registering and executing commands, as well as a large number of extensions to make development on different platforms as easy as possible.

## 📍 Features

### 🗨️ Automated parameter parsing:

`ValueType` and nullable variant types are automatically parsed by the library and populate commands as below:

```cs
[Command("handle")]
public IResult Handle(int param1, DateTime param2)
{
    return Respond("{0}, {1}", param1, param2);
}
```
> This will automatically parse `int` by using the default `int.TryParse` implementation, and will do the same for the `DateTime`.

Outside of this, implementing and adding your own `TypeReader`'s is also supported to handle implementing commands with your own types. Nullability is automatically resolved by the library.

### 🧾 Parameterized remainder:

`RemainderAttribute` defines that all remaining parameters should be concatenated into a single 'remaining' string. This ensures that you don't need to add quotation marks for longer strings such as providing reasons, passwords or directories:

```cs
[Command("handle")]
public void Handle([Remainder] string path)
{
    if (!Path.Exists(path))
        Error("Failed to find specified path!");
}
```

### 🛑 Command preconditions.

Inheriting `PreconditionAttribute` creates a new precondition to add in the attribute range above commands. When a command is tried to be executed, it will walk through every added precondition and abort execution if any of them fail.

```cs
[RequireSystem(PlatformID.Unix)]
[Command("copy")]
public async Task Handle()
{
    
}
```

### 📖 Exposed command info & responsive errors:

CSF.NET will return results for building the command map and executing commands. If you want to run your commands asynchronously, you will have to handle the result process differently:

```cs
_csf.CommandExecuted += async (context, result) => 
{
    await Task.CompletedTask;
    if (result.IsSuccess)
        return;
    // handle failure.
}
```

### 🔗 Virtual base class to support freely overriding all results:

Every single method inside the `CommandFramework` is virtual and can be overwritten if desired to change results or rewrite certain steps inside the pipeline.

### 💡 Support for overriding context, module & framework:

`IContext` can be implemented in your own way, as it by itself serves as just a parsing tool. You can add a number of application-unique properties that are populated at creation. Because of how context's are created, it is easy to implement your own constructors and populate values from your own codebase.

### 💉 Dependency injection:

You can provide an `IServiceProvider` at execution to inject modules with dependencies. Modules are transient and will re-inject the expected services into the module every time a command is executed. Nullable services will not populate if the constructor parameter is declared as nullable.

## 🗺️ Roadmap

- [x] Add support for flags.
- [x] Add regex parameter support.
- [x] Add complex parameters.
- [x] Wrap around other potential frameworks.
- [x] Implement extensions for `IHost` and `WebHost`.

## 🤖 Samples

Samples are available to learn how to implement CSF in your own programs.

- [CSF.Samples.Console](https://github.com/Rozen4334/CSF.NET/tree/master/Samples/CSF.Samples.Console)
  - Shows how to implement CSF on a basic console application.
- [CSF.Samples.TShock4](https://github.com/Rozen4334/CSF.NET/tree/master/Samples/CSF.Samples4.TShock)
  - Shows how to implement CSF.NET.TShock into an OTAPI2 TShock plugin.
- [CSF.Samples.TShock5](https://github.com/Rozen4334/CSF.NET/tree/master/Samples/CSF.Samples5.TShock)
  - Shows how to implement CSF.NET.TShock into an OTAPI3 TShock plugin.

## 📰 Extensions

CSF introduces a number of extensions for external libraries.

- [CSF.Spectre](https://github.com/Rozen4334/CSF.NET/tree/master/Extensions/CSF.Spectre)
  - A package that wraps around [Spectre.Console](https://github.com/spectreconsole/spectre.console).
- [CSF.TShock](https://github.com/Rozen4334/CSF.NET/tree/master/Extensions/CSF.TShock)
  - A package that wraps around [TShock for Terraria](https://github.com/Pryaxis/TShock).

## ❓ Explaining the internals:

CSF.NET works by grabbing all available modules on the specified assembly, storing them into a command map, and running through the pipeline to enter and execute the target command method.

### 🏭 Build steps:

Building the command map is done in a number of steps to ensure the pipeline can run through it succesfully.

- [x] Find all types in the provided assembly and check if they inherit `ModuleBase<>`.
- [x] Find all methods signed with `CommandAttribute`, if any. 
- [x] Create a new `Module` from the found module, and create a new `Command` for each command within.
- [x] Populate the information with all found `TypeReader`s, `PreconditionAttribute`s, attributes and aliases.
- [x] Add all aliases for the command to the commandmap, sharing a reference to the same target command.

> The entire build process resides [here](https://github.com/Rozen4334/CSF.NET/blob/master/Source/CSF/CommandFramework.cs).

### 🔗 Pipeline steps:

The pipeline runs through several steps, in order, to make sure your command can safely pass through to its executing method.

- [x] Look up all available commands matching the provided name.
- [x] Find the best command match for the amount of provided parameters.
- [x] Check all `PreconditionAttribute`s to see if any fail.
- [x] Construct the module and inject found services from the provided `IServiceProvider`
- [x] Iterate through the parameters with provided `TypeReader`s and create a range of parsed parameters from the result.
- [x] Run the `BeforeExecuteAsync` method.
- [x] Run the command method.
- [x] Run the `AfterExecuteAsync` method.
- [x] Return a result to the caller.

> The entire pipeline process resides [here](https://github.com/Rozen4334/CSF.NET/blob/master/Source/CSF/CommandFramework.cs).
