using System.Reflection;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents the default layer of handling inbound command results.
    /// </summary>
    public interface IResultHandler
    {
        /// <summary>
        ///     Invoked when <see cref="CommandFramework.ExecuteCommandAsync{T}(T, System.IServiceProvider)"/> returns a result.
        /// </summary>
        /// <param name="context">The <see cref="IContext"/> used to invoke this command.</param>
        /// <param name="result">The result of this execution.</param>
        /// <param name="command">The found command. <see langword="null"/> if not found.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        Task OnResultAsync(IContext context, IResult result, CommandInfo command = null);

        /// <summary>
        ///     Invoked when <see cref="CommandFramework.ExecuteCommandAsync{T}(T, System.IServiceProvider)"/> returns a result.
        /// </summary>
        /// <param name="context">The <see cref="IContext"/> used to invoke this command.</param>
        /// <param name="result">The result of this execution.</param>
        /// <param name="command">The found command. <see langword="null"/> if not found.</param>
        void OnResult(IContext context, IResult result, CommandInfo command = null);
    }
}
