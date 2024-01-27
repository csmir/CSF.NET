using CSF.Reflection;

namespace CSF
{
    public abstract class ModuleBase<T> : ModuleBase
        where T : ICommandContext
    {
        private T _context;

        public new T Context
        {
            get
                => _context ??= (T)base.Context;
        }
    }

    public abstract class ModuleBase
    {
        public ICommandContext Context { get; internal set; }

        public IServiceProvider Services { get; internal set; }

        public CommandInfo Command { get; internal set; }

        internal virtual RunResult ReturnTypeResolve(object value)
        {
            switch (value)
            {
                case Task task:
                    return new(Command, task);
                case null:
                    return new(Command, returnValue: null);
                default:
                    {
                        var result = HandleUnknownReturnType(value);

                        if (!result.Success)
                        {
                            Context.LogWarning("{} returned unknown type. Consider overriding {} to resolve this message.", Command, nameof(HandleUnknownReturnType));
                            return new(Command, returnValue: value);
                        }

                        return result;
                    }
            }
        }

        public virtual RunResult HandleUnknownReturnType(object value)
        {
            return new RunResult(Command, exception: null);
        }

        public virtual ValueTask BeforeExecuteAsync()
        {
            return ValueTask.CompletedTask;
        }

        public virtual ValueTask AfterExecuteAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
