using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Hosting
{
    public interface IContextFactory : IDisposable
    {
        public ICommandContext CreateContext();
    }

    public interface IContextFactory<T> : IContextFactory
        where T : ICommandContext
    {
        public new T CreateContext();

        ICommandContext IContextFactory.CreateContext()
        {
            return CreateContext();
        }
    }
}
