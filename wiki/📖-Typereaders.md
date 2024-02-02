Typereaders read provided argument input in string format and try to convert them into the types as defined in the command signature.
Let's work on an example Typereader to learn how they work.

- [Creating your Typereader](#-creating-your-typereader)
- [Using your Typereader](#-using-your-typereader)

## 🏗️ Creating your Typereader

All Typereaders inherit `TypeReader<T>` or `TypeReader`. To start creating your Typereader, you have to inherit one of the two on a class.

> For the simplicity of this documentation, only the generic type is introduced here.

```cs
using CSF;

namespace XProject
{
    public class GuidTypeReader : TypeReader<Guid>
    {
        public override Result Evaluate(ICommandContext context, IParameterComponent parameter, IServiceProvider services, string value)
        {
        }
    }
}
```

With this class defined and the method that will operate the evaluation being implemented, we can now write our code which defines the succession and failure conditions. In case of success, we also need to pass in the parsed object that the typereader expects to see returned.

```cs
        public override Result Evaluate(ICommandContext context, IParameterComponent parameter, IServiceProvider services, string value)
        {
            if (Guid.TryParse(value, out Guid guid))
                return Success(guid);

            return Failure("Failed to parse the input string as a GUID.");
        }
```

That's it. With this done, we can look towards the application of our created Typereader.

## 📝 Using your Typereader

After you have written your Typereader, it is time to use it. Let's define a command that receives a `Guid` as one of its parameters.

```cs
[Command("guid")]
public void Guid(Guid guid)
{
    Respond("Here is your guid: " + guid.ToString());
}
```

This method will accept a Guid as parameter, and then responds with it. This can be handled any other way. Because of how CSF is written, if the Typereader is defined in the same assembly as registered commands, it will also be automatically registered.

> If your Typereader by special chance is not automatically registered, or you want to register X amount of the same type of typereader, it can be added manually through `IServiceCollection.WithCommandManager(x => x.TypeReaders = [])`