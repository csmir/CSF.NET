using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Defines the default <see cref="TypeReader{T}"/> for enums.
    /// </summary>
    /// <remarks>
    ///     To implement this typereader, you must first define it with the associated enum in <see cref="CommandStandardizationFramework.RegisterTypeReader{T}(TypeReader{T}, bool)"/>.
    /// </remarks>
    /// <typeparam name="T">The enum this parser belongs to.</typeparam>
    public class EnumTypeReader<T> : TypeReader<T>
        where T : struct, Enum
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, ParameterInfo info, string value, IServiceProvider provider)
        {
            if (Enum.TryParse<T>(value, out var result))
                return Task.FromResult(TypeReaderResult.FromSuccess(result));

            return Task.FromResult(TypeReaderResult.FromError(
                errorMessage: $"The provided value is not a part the enum specified. Expected: '{typeof(T).Name}', got: '{value}'. At: '{info.Name}'"));
        }
    }
}
