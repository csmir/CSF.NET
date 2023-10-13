using System.Runtime.CompilerServices;

namespace CSF
{
    /// <summary>
    ///     Represents a 
    /// </summary>
    public interface IResult
    {
        public TaskAwaiter GetAwaiter();

        /// <summary>
        ///     Checks if the command from which this <see cref="IResult"/> was formatted failed to execute or not.
        /// </summary>
        /// <returns><see langword="True"/> if the command failed to execute. <see langword="False"/> if it succeeded.</returns>
        public bool Failed(out FailedResult result);
    }
}
