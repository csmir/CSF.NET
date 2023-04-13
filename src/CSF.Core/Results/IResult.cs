using System.Diagnostics.CodeAnalysis;

namespace CSF
{
    /// <summary>
    ///     The interface implemented by all result types.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        ///     The result's execution code.
        /// </summary>
        public FailureCode Code { get; }

        /// <summary>
        ///     The result's message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        ///     The result's inner exception. <c>null</c> if not applicable.
        /// </summary>
        public Exception Exception { get; }

        public bool TryGetValue<TValue>([NotNullWhen(true)] out TValue value);
    }
}
