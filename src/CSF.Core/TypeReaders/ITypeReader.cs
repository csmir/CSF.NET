namespace CSF
{
    /// <summary>
    ///     Represents an interface used by <see cref="TypeReader{T}"/> without needing to internally provide the generic parameter.
    /// </summary>
    /// <remarks>
    ///     Do not use this interface to build type readers on. Use <see cref="TypeReader{T}"/> instead.
    /// </remarks>
    public interface ITypeReader
    {
        public Type Type { get; }

        public object Read(IContext context, IParameterComponent parameter, IServiceProvider services, string value);
    }
}
