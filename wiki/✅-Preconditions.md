Preconditions are checks that ensure that the command in scope is allowed to be executed. 
Let's work on an example precondition to understand how they work. 

- [Creating your Precondition](#-creating-your-precondition)
- [Using your Precondition](#-using-your-precondition)

## 🏗️ Creating your Precondition

All preconditions inherit `PreconditionAttribute`, which in turn inherits `Attribute`. To start creating your own precondition, you have to inherit `PreconditionAttribute` on a class:

```cs
using CSF;

namespace XProject
{
    public class RequireOperatingSystemAttribute : PreconditionAttribute
    {
        public override Result Evaluate(ICommandContext context, Command command, IServiceProvider provider)
        {
        }
    }
}
```

With this class defined, and the method that will operate the evaluation being implemented, we can now write our code which defines the succession and failure conditions.

First of all, the restriction must be compared against. To define this, we will implement a constructor parameter, automatically resolved as an attribute parameter by the IDE or code editor:

```cs
using CSF;

namespace XProject
{
    public class RequireOperatingSystemAttribute : PreconditionAttribute
    {

        public PlatformID Platform { get; }

        public RequireOperatingSystemAttribute(PlatformID platform)
        {
            Platform = platform;
        }

        public override Result Evaluate(ICommandContext context, Command command, IServiceProvider provider)
        {
        }
    }
}
```

After this has been defined, the `Platform` property can be used to evaluate the current operating system against. Our focus goes to the `Evaluate` method, which will run this check.

```cs
        public override Result Evaluate(ICommandContext context, Command command, IServiceProvider provider)
        {
            if (Environment.OSVersion.Platform == Platform)
                return Success();

            return Failure("The platform this command was executed does not support this operation.");
        }
```

That's it. With this done, we can look towards the application of our created precondition.

## 📝 Using your Precondition

After you have written your precondition, it is time to use it. Let's define a command that depends on the operating system to run.

```cs
[Command("copy")]
public void Copy([Remainder] string toCopy)
{
    Process clipboardExecutable = new()
    {
        StartInfo = new ProcessStartInfo
        {
            RedirectStandardInput = true,
            FileName = "clip",
        }
    };
    clipboardExecutable.Start();

    clipboardExecutable.StandardInput.Write(toCopy);
    clipboardExecutable.StandardInput.Close();

    Respond("Succesfully copied the content to your clipboard.");
}
```

This method will use the windows clip executable to copy a string onto your clipboard. 
This method does not work on macOS or Linux, so we have to make sure the command is executed on windows.

To do this, all we have to do is add `[RequireOperatingSystem(PlatformID.Win32NT)]` above the method, like so:

```cs
[Command("copy")]
[RequireOperatingSystem(PlatformID.Win32NT)]
public void Copy([Remainder] string toCopy)
```

The precondition is now defined on this command, and will be called when this command is triggered.