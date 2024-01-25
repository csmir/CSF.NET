using CSF.TypeReaders;
using System.Reflection;

namespace CSF
{
    public sealed class CommandConfiguration
    {
        public Assembly[] Assemblies { get; set; } = [ Assembly.GetEntryAssembly() ];

        public TypeReader[] TypeReaders { get; set; } = [];
    }
}
