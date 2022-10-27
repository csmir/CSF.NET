namespace CSF
{
    /// <summary>
    ///     The source of an executed command.
    /// </summary>
    public interface ISource
    {
        /// <summary>
        ///     The name of the source.
        /// </summary>
        string Name { get; }
    }
}
