using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CSF
{
    public interface IModuleBase
    {
        /// <summary>
        ///     Gets the command's context.
        /// </summary>
        public IContext Context { get; }

        /// <summary>
        ///     Displays all information about the command thats currently in scope.
        /// </summary>
        public Command Command { get; }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public ValueTask BeforeExecuteAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public ValueTask AfterExecuteAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public IResult Respond(string message);
    }
}
