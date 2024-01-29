using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Core
{
    public interface IResultHandler : IDisposable
    {
        public Task HandleAsync(ICommandContext context, ICommandResult result, CancellationToken cancellationToken);
    }
}
