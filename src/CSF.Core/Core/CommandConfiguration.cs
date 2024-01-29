using CSF.TypeReaders;
using System.Reflection;

namespace CSF.Core
{
    public sealed class CommandConfiguration
    {
        public Assembly[] Assemblies { get; set; } = [ Assembly.GetEntryAssembly() ];

        public TypeReader[] TypeReaders { get; set; } = [];

        public TaskAwaitOptions ExecutionPattern { get; set; }
    }
}
