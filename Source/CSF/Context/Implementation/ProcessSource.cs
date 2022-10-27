using System;
using System.Diagnostics;

namespace CSF
{
    /// <summary>
    ///     Represents a source from the process. The <see cref="Name"/> be the name of the program.
    /// </summary>
    public class ProcessSource : ISource
    {
        private readonly static Lazy<string> _name = new Lazy<string>(() => Process.GetCurrentProcess().ProcessName);

        /// <inheritdoc/>
        public string Name
        {
            get
                => _name.Value;
        }

        /// <summary>
        ///     Creates a new process source that grabs the current process name as its name.
        /// </summary>
        public ProcessSource()
        {

        }
    }
}
