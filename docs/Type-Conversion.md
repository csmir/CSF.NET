A `TypeConverter` reads provided argument input in string format and try to convert them into the types as defined in the command signature.
Let's work on an example to learn how they work.

- [Creating your TypeConverter](#creating-your-typeconverter)
- [Using your TypeConverter](#using-your-typeconverter)

## Creating your TypeConverter

All TypeConverter inherit `TypeConverter<T>` or `TypeConverter`. To start creating your TypeConverter, you have to inherit one of the two on a class.

> For the simplicity of this documentation, only the generic type is introduced here.

```cs
using CSF.Core;
using CSF.Reflection;
using CSF.TypeConverters;

namespace CSF.Samples
{
    public class ReflectionTypeConverter : TypeConverter<Type>
    {
        public override async ValueTask<ConvertResult> EvaluateAsync(ICommandContext context, IServiceProvider services, IArgument argument, string raw, CancellationToken cancellationToken)
        {
        }
    }
}
```

With this class defined and the method that will operate the evaluation being implemented, we can now write our code which defines the succession and failure conditions. In case of success, we also need to pass in the parsed object that the typereader expects to see returned.

```cs
    ...
        public override async ValueTask<ConvertResult> EvaluateAsync(ICommandContext context, IServiceProvider services, IArgument argument, string raw, CancellationToken cancellationToken)
        {
            try
            {
                var typeSrc = Type.GetType(
                    typeName: raw, 
                    throwOnError: true, 
                    ignoreCase: true);

                return ValueTask.FromResult(Success(typeSrc));
            }
            catch (Exception ex)
            {
                return ValueTask.FromResult(Error(ex));
            }
        }
    ...
```

With the logic defined, we can also add options in the converter, for example by customizing how `ignoreCase` is configured in the `Type` search:

```cs
...
    public class ReflectionTypeConverter(bool caseIgnore) : TypeConverter<Type>
    {
        private readonly bool _caseIgnore = caseIgnore;
        
        ...
    }
...
```

```cs

        ...
            var typeSrc = Type.GetType(
                typeName: raw, 
                throwOnError: true, 
                ignoreCase: _caseIgnore);
        ...
```

## Using your TypeConverter

After you have written your TypeConverter, it is time to use it. Let's define a command that receives a `Type` as one of its parameters.

```cs
    ...
        [Command("type-info", "typeinfo", "type")]
        public void TypeInfo(Type type)
        {
            Console.WriteLine($"Information about: {type.Name}");

            Console.WriteLine($"Fullname: {type.FullName}");
            Console.WriteLine($"Assembly: {type.Assembly.FullName}");
        }
    ...
```

If you start the program now, it will throw an error on startup. `System.Type` has no known `TypeConverter`. In order to resolve this error, we must return to the `Program.cs` file of your application, and define the `TypeConverter` through configuration:

```cs
    ...
        configuration.AddTypeReader(new ReflectionTypeConverter(caseIgnore: true));
    ...
```

Restarting the program now, you can try out your new command and see the results for yourself.