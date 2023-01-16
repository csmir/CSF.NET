using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSF
{
    public readonly struct ParseResult : IResult
    {
        /// <inheritdoc/>
        public bool IsSuccess { get; }

        /// <inheritdoc/>
        public string ErrorMessage { get; }

        /// <summary>
        ///     The unnamed output values of this command.
        /// </summary>
        internal IReadOnlyList<object> Arguments { get; }

        /// <summary>
        ///     The named output values of this command.
        /// </summary>
        internal IReadOnlyDictionary<string, object> NamedArguments { get; }

        /// <summary>
        ///     The prefix of this command.
        /// </summary>
        internal IPrefix Prefix { get; }

        /// <summary>
        ///     The name of the command.
        /// </summary>
        internal string Name { get; }

        /// <inheritdoc/>
        public Exception Exception { get; }

        private ParseResult(bool success, string name = null, IPrefix prefix = null, List<object> args = null, Dictionary<string, object> namedArgs = null, string msg = null, Exception exception = null)
        {
            IsSuccess = success;

            if (IsSuccess)
            {
                if (string.IsNullOrEmpty(name))
                    throw new MissingValueException("Found null or empty.", nameof(name));

                if (args is null)
                    throw new ArgumentNullException(nameof(args));

                namedArgs ??= new Dictionary<string, object>();
                prefix ??= EmptyPrefix.Create();
            }

            Prefix = prefix;
            Name = name;
            Arguments = args;
            NamedArguments = namedArgs;
            ErrorMessage = msg;
            Exception = exception;
        }

        public static implicit operator ValueTask<ParseResult>(ParseResult result)
            => result.AsValueTask();

        /// <summary>
        ///     Creates a failed args with provided parameters.
        /// </summary>
        /// <args name="errorMessage"></args>
        /// <args name="exception"></args>
        /// <returns></returns>
        public static ParseResult FromError(string errorMessage, Exception exception = null)
            => new ParseResult(false, null, null, null, null, errorMessage, exception);

        /// <summary>
        ///     Creates a succesful args with provided parameters.
        /// </summary>
        /// <returns></returns>
        public static ParseResult FromSuccess(string name, IPrefix prefix, List<object> args, Dictionary<string, object> namedArgs)
            => new ParseResult(true, name, prefix, args, namedArgs);
    }
}
