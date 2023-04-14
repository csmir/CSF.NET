namespace CSF
{
    internal class BaseTypeReader<T> : TypeReader<T>
    {
        private delegate bool Tpd<TValue>(string str, out TValue value);

        private readonly static Lazy<IReadOnlyDictionary<Type, Delegate>> _container = new(ValueGenerator);

        public override object Read(IContext context, IParameterComponent parameter, IServiceProvider services, string value)
        {
            var parser = _container.Value[Type] as Tpd<T>;

            if (parser(value, out var result))
                return result;

            return Fail($"The provided value does not match the expected type. Expected {typeof(T).Name}, got {value}. At: '{parameter.Name}'");
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

                // guid
                [typeof(Guid)] = (Tpd<Guid>)Guid.TryParse
            };

            return callback;
        }
    }

    internal static class BaseTypeReader
    {
        public static TypeReader[] CreateBaseReaders()
        {
            var callback = new TypeReader[]
            {
                // char
                new BaseTypeReader<char>(),

                // bit / boolean
                new BaseTypeReader<bool>(),

                // 8 bit int
                new BaseTypeReader<byte>(),
                new BaseTypeReader<sbyte>(),

                // 16 bit int
                new BaseTypeReader<short>(),
                new BaseTypeReader<ushort>(),

                // 32 bit int
                new BaseTypeReader<int>(),
                new BaseTypeReader<uint>(),

                // 64 bit int
                new BaseTypeReader<long>(),
                new BaseTypeReader<ulong>(),

                // floating point int
                new BaseTypeReader<float>(),
                new BaseTypeReader<double>(),
                new BaseTypeReader<decimal>(),

                // time
                new BaseTypeReader<DateTime>(),
                new BaseTypeReader<DateTimeOffset>(),

                // guid
                new BaseTypeReader<Guid>()
            };

            return callback;
        }
    }
}
