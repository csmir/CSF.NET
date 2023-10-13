using System.Runtime.CompilerServices;

namespace CSF
{
    public readonly struct SuccessResult : IResult
    {
        public Task CommandTask { get; }

        public SuccessResult(Task t)
        {
            CommandTask = t;
        }

        public SuccessResult()
        {
            CommandTask = Task.CompletedTask;
        }

        /// <inheritdoc />
        public bool Failed()
            => false;

        /// <inheritdoc />
        public TaskAwaiter GetAwaiter()
            => CommandTask.GetAwaiter();

        public static implicit operator Task(SuccessResult result)
        {
            return result.CommandTask;
        }
    }
}
