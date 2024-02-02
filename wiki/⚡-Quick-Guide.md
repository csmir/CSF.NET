This guide introduces you to CSF, covering the basics for developers ranging from beginner to veteran.

- [Before Coding](#-setting-up-the-basics)
- [Defining Commands](#-defining-commands)
- [Running your Commands](#-running-your-commands)
- [Putting it Together](#-summary)

## 📩 Before Coding

Before we can start writing code, there are a few things to set up. This includes getting a fresh project to start, and installing CSF.

### Creating a New Project

Before being able to write code, we need to give ourselves a place to do it. 
With your favourite code editor, we will create a new empty project. 
This project can be anything, but in our case, a **Console Application** will be the focus. 
If you are using `dotnet-cli`, you can create such a project by executing `dotnet new console`. 
Keep in mind that the project has to target `.net6` or higher.

> In a visual editor such as Rider or Visual studio, you can create it by simply opening the application, navigating to 'Create New Project' and selecting the Console Application template.

### Installing the Package

To be able to reference CSF, we will need to head over to your package manager to install the package. 
This can be in the PM console, in your `.csproj` file or through the code editor.
We can grab the latest version from [NuGet](https://www.nuget.org/packages/CSF.NET). 
If you prefer to use the visual tools, simply look up `csf.net` and install the first package available.

## 📝 Defining Commands

### Creating a Module

Let's create a new file in your favourite code editor. 
The ideal way to do this is by calling it `XModule.cs` where `X` is its name or category.

After creating the file, we should see the following:

```cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XProject
{
    internal class XModule
    {
    }
}
```

Next up, we will want to make the class public, and turn it into a module. 
To do that, we need to include CSF into the usings, and change the signature of the class to inherit `ModuleBase`. 
With both of those steps out of the way, it should now look like this:

```cs
using CSF;

namespace XProject
{
    public class XModule : ModuleBase
    {
    }
}
```

> Starting in .NET6, [implicit usings](https://learn.microsoft.com/en-gb/dotnet/core/project-sdk/overview#implicit-using-directives) are part of the core features. 
> Because we will assume that this is a clean, newly created project on .NET 6 or higher, we can remove the usings without an effect on the functionality. 
> If you do see errors from removing the extra usings, please ignore this advice.

### Creating Commands

With your basic module complete, we are now ready to start writing commands. 
To begin, we will write 2 commands:

#### Command 1: Hello World

Within the class declaration of the file, we will write a new method. 
The method can return `Task` or `void`, but for the sake of simplicity, we will use `void`.

```cs
using CSF;

namespace XProject
{
    public class XModule : ModuleBase
    {
        public void HelloWorld()
        {
        }
    }
}
```

Next up, we will want to decorate the method with an attribute. The `[Command]` attribute accepts a name, and registers the command by it, so we can use that name to run it from our input source. We will call the command `helloworld`. Let's add the attribute and see how that looks:

```cs
using CSF;

namespace XProject
{
    public class XModule : ModuleBase
    {
        [Command("helloworld")]
        public void HelloWorld()
        {
        }
    }
}
```

Now the `HelloWorld` method will run when we run the `helloworld` command, but there is still one thing missing. The method is empty! You can add anything here, but in our case, a simple reply will do:

```cs
using CSF;

namespace XProject
{
    public class XModule : ModuleBase
    {
        [Command("helloworld")]
        public void HelloWorld()
        {
            Respond("Hello world!");
        }
    }
}
```

> To actually start testing and running the command, skip to [#

#### Command 2: Reply

Our next command will be quite simple as well. Just like before, we can create a new method, mark it with an attribute and give it a name. Except this time, it will accept input, and respond with it. Let's create this functionality:

```cs
using CSF;

namespace XProject
{
    public class XModule : ModuleBase
    {
        [Command("helloworld")]
        public void HelloWorld()
        {
            Respond("Hello world!");
        }

        [Command("reply")]
        public void Reply([Remainder] string message)
        {
            Respond(message);
        }
    }
}
```

As you can see here, the reply method accepts a parameter called `message`, and replies to the command with its value. Normally, this would automatically accept a string parameter, usually a single word: `word` or multiple words connected by quotations: `"a few words"`.

In our case however, the command is marked with `[Remainder]`. This attribute has a special property, it ignores all parameters provided after it. Instead, it connects all of them together and makes it one long string (or object). This way you don't need to add quotation marks, making the command more intuitive to use.

## 🏗️ Running your Commands

CSF is quite simple in that it does a very good job in doing things *for* you. Because of this, setting up your commands is not actually directly visible. But, there is one very important step, and for it, we need to look at a more advanced concept in programming. Dependency Injection!

### Setting up Dependency Injection

You don't actually need to understand how dependency injection works to enjoy its benefits. 
For newcomers, it is worth it to do some research, but this guide does not depend on it. 
CSF does however, expect a component to be installed to work with it in the background. For installing it, please refer to [here](https://github.com/csmir/CSF.NET/wiki#-additional-packages).

> Dependency Injection is well documented by Microsoft and the .NET foundation [here](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection).

To set up the framework with it, we will go back to the `Program.cs` file. 
It may look different for everyone, but for simplicity sake, we will only focus on [top-level statements](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/top-level-statements).

Let's start with the basics, and define a new `ServiceCollection`. 
This collection works like a list, and things can be added to it. 
CSF already has the needed methods to configure everything at once, so all we have to do is call them:

```cs
using CSF;
using Microsoft.Extensions.DependencyInjection;

var collection = new ServiceCollection();

collection.WithCommandManager();

var services = collection.BuildServiceProvider();
```

The `AddCommandManager` method configures everything we need, and leaves very little for us to do. 
With that out of the way, we can build the collection into a provider. 
To do this, we call the `BuildServiceProvider` method.

Now that everything in the background has been built, the command manager has also been made accessible to us. 
While staying in `Program.cs`, we can grab the manager from the new `services` variable by calling `GetRequiredService`.

```cs
var framework = services.GetRequiredService<CommandManager>();
```

Hovering over the `CommandManager` declaration, you can read some things about it. It is the root type, or class, that makes CSF work as a whole. 
You can greatly customize it and make a lot of changes to your personal taste. 
Though, we only need very little from it. 

### Listening to Input

The most important part of running a command is defining *what* command you want to run. 
In our case, we want to test the `helloworld` command, so we can define that as our input.
You can replace the way to get your input with anything you like, ranging from `Console.ReadLine` to listening to a discord message.

```cs
var input = "helloworld";

var context = new CommandContext(input);
```

With this input, we can create the most important component to running a command: The context. 
Execution accepts `ICommandContext` and will use that to parse and run the command. 
For regular string commands, `CommandContext` is predefined by CSF for you to use.

### Running the Command

With the context defined, it is time to run the command. 
By calling `ExecuteAsync`, CSF will read your command input and try to find the most appropriate command to run. 
In our case, it is an exact match to our `helloworld` command, so it will succeed the search and run it.

```cs
var result = await framework.ExecuteAsync(context);

if (result.Failed())
    throw result.Exception;
```

The final step is to handle the result of the command. 
If the command failed in any way, the `CommandResult` returned by the execution will cover where it went wrong and contain the exception that occurred. 
Here, it is simplest to see if it failed, and then throw the exception if it did.

## 📑 Summary

Let's review what we achieved in this guide. 
First of all, we created an empty project and installed the package.
After that, we defined our module that implements 2 commands.
One accepting a value, and one being a simple trigger.

To run the command, we moved back to the `Program.cs` file and implemented the code to set up the commands. 
Finally, we set up the requirements to run it.

Let's take a look at the code we wrote to summarize our work.

### Module and Commands

```cs
#XModule.cs
using CSF;

namespace XProject
{
    public class XModule : ModuleBase
    {
        [Command("helloworld")]
        public void HelloWorld()
        {
            Respond("Hello world!");
        }

        [Command("reply")]
        public void Reply([Remainder] string message)
        {
            Respond(message);
        }
    }
}
```

### Setup and Execution

```cs
#Program.cs
using CSF;
using Microsoft.Extensions.DependencyInjection;

var collection = new ServiceCollection()
    .AddCommandManager();

var services = collection.BuildServiceProvider();

var framework = services.GetRequiredService<CommandManager>();

var input = "helloworld";

var context = new CommandContext(input);

var result = await framework.ExecuteAsync(context);

if (result.Failed())
    throw result.Exception;
```

If these files match yours, you're good to go. 
To run your command, simply run the application and see the result. 