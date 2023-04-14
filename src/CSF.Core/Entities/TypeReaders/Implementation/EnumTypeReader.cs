using Microsoft.Extensions.DependencyInjection;

namespace CSF
{
    /// <summary>
    ///     Defines the default <see cref="TypeReader{T}"/> for enums.
    /// </summary>
    /// <remarks>
    ///     To implement this typereader, you must first define it with the associated enum and add it to the <see cref="IServiceCollection"/>.
    /// </remarks>
    /// <typeparam name="T">The enum this parser belongs to.</typeparam>
    public class EnumTypeReader<T> : TypeReader<T>
        where T : struct, Enum
    {
        public override object Read(ICommandContext context, IParameterComponent parameter, IServiceProvider services, string value)
        {
            if (Enum.TryParse<T>(value, true, out var result))
                return result;

            return Fail($"The provided value is not a part the enum specified. Expected: '{typeof(T).Name}', got: '{value}'. At: '{parameter.Name}'");
        }
    }
}
