using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents a binding that holds a reference to a scope.
    /// </summary>
    public interface IBinding
    {
        /// <summary>
        ///     The value that is used to bind.
        /// </summary>
        public object Binding { get; }

        /// <summary>
        ///     Compares the binding to the input value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetBinding<T>(T value)
        {
            if (Binding is T t)
            {
                if (t.Equals(value))
                    return true;
            }
            return false;
        }
    }
}
