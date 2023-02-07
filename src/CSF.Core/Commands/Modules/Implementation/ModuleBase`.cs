namespace CSF
{
    /// <summary>
    ///     Represents a generic command base to implement commands with.
    /// </summary>
    /// <typeparam name="T">The <see cref="IContext"/> expected to use for this command.</typeparam>
    public abstract class ModuleBase<T> : ModuleBase
        where T : IContext
    {
        private T _context;

        /// <summary>
        ///     Gets the command's context.
        /// </summary>
        public new T Context
        {
            get
                => _context ??= (T)base.Context;
        }
    }
}
