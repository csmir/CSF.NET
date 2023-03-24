using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CSF
{
    /// <summary>
    ///     Represents a dictionary of type readers.
    /// </summary>
    public sealed class TypeReaderProvider
    {
        private readonly Dictionary<Type, ITypeReader> _typeReaders;

        /// <summary>
        ///     Creates a new typereader dictionary with all default readers.
        /// </summary>
        public TypeReaderProvider()
            : this(TypeReader.CreateDefaultReaders())
        {

        }

        /// <summary>
        ///     Creates a new <see cref="TypeReaderProvider"/> with self-defined default readers.
        /// </summary>
        /// <param name="dictionary"></param>
        public TypeReaderProvider(Dictionary<Type, ITypeReader> dictionary)
        {
            _typeReaders = dictionary;
        }

        /// <summary>
        ///     Gets or sets a typereader for the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ITypeReader this[Type key]
        {
            get
                => _typeReaders[key];
            set
                => _typeReaders[key] = value;
        }

        /// <summary>
        ///     Includes an <see cref="ITypeReader"/> in the <see cref="TypeReaderProvider"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="reader"></param>
        /// <returns>The same instance for chaining calls.</returns>
        public TypeReaderProvider Include(ITypeReader reader)
        {
            if (_typeReaders.ContainsKey(reader.Type))
                _typeReaders[reader.Type] = reader;
            else
                _typeReaders.Add(reader.Type, reader);
            return this;
        }

        /// <summary>
        ///     Excludes an <see cref="ITypeReader"/> from the <see cref="TypeReaderProvider"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> for which to remove the <see cref="ITypeReader"/>.</typeparam>
        /// <returns>The same instance for chaining calls.</returns>
        public TypeReaderProvider Exclude<T>()
            => Exclude(typeof(T));

        /// <summary>
        ///     Excludes an <see cref="ITypeReader"/> from the <see cref="TypeReaderProvider"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to remove the <see cref="ITypeReader"/>.</param>
        /// <returns>The same instance for chaining calls.</returns>
        public TypeReaderProvider Exclude(Type type)
        {
            _typeReaders.Remove(type);
            return this;
        }

        /// <summary>
        ///     Checks if the underlying dictionary contains the provided type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>True if success. False if not.</returns>
        public bool ContainsType(Type type)
            => _typeReaders.ContainsKey(type);

        /// <summary>
        ///     Tries to get a <see cref="ITypeReader"/> from the underlying dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns>True if success. False if not.</returns>
        public bool TryGetReader<T>([NotNullWhen(true)] out ITypeReader reader)
            => TryGetReader(typeof(T), out reader);

        /// <summary>
        ///     Tries to get a <see cref="ITypeReader"/> from the underlying dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns>True if success. False if not.</returns>
        public bool TryGetReader(Type type, [NotNullWhen(true)] out ITypeReader reader)
        {
            reader = null;

            if (type == typeof(string))
                return true;

            if (type == typeof(object))
                return true;

            if (_typeReaders.ContainsKey(type))
            {
                reader = _typeReaders[type];
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Gets if the <see cref="TypeReaderProvider"/> has any <see cref="ITypeReader"/>'s defined.
        /// </summary>
        /// <returns><see langword="true"/> if there are any <see cref="ITypeReader"/>'s inside the current <see cref="TypeReaderProvider"/>. <see langword="false"/> if not.</returns>
        public bool HasValues()
            => _typeReaders.Any();
    }
}
