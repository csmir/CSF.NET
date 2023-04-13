using System.Diagnostics.CodeAnalysis;

namespace CSF
{
    public readonly struct Result : IResult
    {
        /// <inheritdoc/>
        public FailureCode Code { get; } = FailureCode.None;

        /// <inheritdoc/>
        public string Message { get; } = null;

        /// <inheritdoc/>
        public Exception Exception { get; } = null;

        public Result(FailureCode code, string message = null, Exception exception = null)
        {
            Code = code;
            Message = message;
            Exception = exception;
        }

        /// <inheritdoc/>
        public bool TryGetValue<TValue>([NotNullWhen(true)] out TValue value)
        {
            value = default;
            return false;
        }

        /// <summary>
        ///     Formats a readable output from the result.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"Code: {Code}, Message: {Message}, Exception: {Exception}";
    }
}
