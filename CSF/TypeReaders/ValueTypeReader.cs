using CSF.Commands;
using CSF.Info;
using CSF.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSF.TypeReaders
{
    internal class ValueTypeReader<T> : TypeReader<T>
    {
        private delegate bool Tpd<TValue>(string str, out TValue value);

        private static Lazy<IReadOnlyDictionary<Type, Delegate>> _container = new Lazy<IReadOnlyDictionary<Type, Delegate>>(ValueGenerator);

        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, ParameterInfo info, string value, IServiceProvider provider)
        {
            if (TryGetParser(out var parser))
            {
                if (parser(value, out var result))
                    return Task.FromResult(TypeReaderResult.FromSuccess(result));
            }
            return Task.FromResult(TypeReaderResult.FromError($"Input invalid! Expected {nameof(T)}, got {value}."));
        }

        private static bool TryGetParser(out Tpd<T> parser)
        {
            parser = null;
            if (_container.Value.TryGetValue(typeof(T), out var result))
            {
                parser = (Tpd<T>)result;
                return true;
            }
            return false;
        }

        private static IReadOnlyDictionary<Type, Delegate> ValueGenerator()
        {
            var callback = new Dictionary<Type, Delegate>
            {
                // char
                [typeof(char)] = (Tpd<char>)char.TryParse,

                // bit / boolean
                [typeof(bool)] = (Tpd<bool>)bool.TryParse,

                // 8 bit int
                [typeof(byte)] = (Tpd<byte>)byte.TryParse,
                [typeof(sbyte)] = (Tpd<sbyte>)sbyte.TryParse,

                // 16 bit int
                [typeof(short)] = (Tpd<short>)short.TryParse,
                [typeof(ushort)] = (Tpd<ushort>)ushort.TryParse,

                // 32 bit int
                [typeof(int)] = (Tpd<int>)int.TryParse,
                [typeof(uint)] = (Tpd<uint>)uint.TryParse,

                // 64 bit int
                [typeof(long)] = (Tpd<long>)long.TryParse,
                [typeof(ulong)] = (Tpd<ulong>)ulong.TryParse,

                // floating point int
                [typeof(float)] = (Tpd<float>)float.TryParse,
                [typeof(double)] = (Tpd<double>)double.TryParse,
                [typeof(decimal)] = (Tpd<decimal>)decimal.TryParse,

                // time
                [typeof(DateTime)] = (Tpd<DateTime>)DateTime.TryParse,
                [typeof(DateTimeOffset)] = (Tpd<DateTimeOffset>)DateTimeOffset.TryParse,
            };

            return callback;
        }
    }
}
