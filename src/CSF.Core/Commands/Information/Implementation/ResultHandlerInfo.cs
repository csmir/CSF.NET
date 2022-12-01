using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CSF
{
    public sealed class ResultHandlerInfo : IComponent
    {
        /// <inheritdoc/>
        public string Name { get; }

        //// <inheritdoc/>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        ///     The type of this result handler.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///     The constructor used to create an instance of the result handler type.
        /// </summary>
        public ConstructorInfo Constructor { get; }

        internal ResultHandlerInfo(Type type)
        {
            Type = type;
            Name = type.Name;
            Constructor = new ConstructorInfo(type);
            Attributes = GetAttributes()
                .ToList();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ResultHandlerInfo Build(Type type)
        {
            if (type != typeof(IResultHandler))
                throw new InvalidOperationException($"Provided type is not a valid {nameof(IResultHandler)}.");

            return new ResultHandlerInfo(type);
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        public IResultHandler Construct(IServiceProvider provider)
        {
            var obj = Constructor.Construct(provider);

            if (obj is IResultHandler handler)
                return handler;
            else
                return null;
        }

        private IEnumerable<Attribute> GetAttributes()
        {
            foreach (var attr in Type.GetCustomAttributes(true))
                if (attr is Attribute attribute)
                    yield return attribute;
        }
    }
}
