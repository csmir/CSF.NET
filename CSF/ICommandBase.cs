using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     
    /// </summary>
    public interface ICommandBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task ExecuteAsync();
    }
}
