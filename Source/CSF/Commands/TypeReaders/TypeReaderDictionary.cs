using System;
using System.Collections.Generic;

namespace CSF
{
    /// <summary>
    ///     Represents a dictionary of type readers.
    /// </summary>
    public sealed class TypeReaderDictionary
    {
        private readonly Dictionary<Type, ITypeReader> _typeReaders;

        /// <summary>
        ///     Creates a new typereader dictionary with all default readers.
        /// </summary>
        public TypeReaderDictionary()
            : this(TypeReader.CreateDefaultReaders())
        {

        }

        /// <summary>
        ///     Creates a new <see cref="TypeReaderDictionary"/> with self-defined default readers.
        /// </summary>
        /// <param name="dictionary"></param>
        public TypeReaderDictionary(Dictionary<Type, ITypeReader> dictionary)
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
        ///     Includes an <see cref="ITypeReader"/> in the <see cref="TypeReaderDictionary"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns>The same instance for chaining calls.</returns>
        public TypeReaderDictionary Include<T>(TypeReader<T> reader)
            => Include(typeof(T), reader);

        /// <summary>
        ///     Includes an <see cref="ITypeReader"/> in the <see cref="TypeReaderDictionary"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="reader"></param>
        /// <returns>The same instance for chaining calls.</returns>
        public TypeReaderDictionary Include(Type type, ITypeReader reader)
        {
            _typeReaders.Add(type, reader);
            return this;
        }

        /// <summary>
        ///     Excludes an <see cref="ITypeReader"/> from the <see cref="TypeReaderDictionary"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> for which to remove the <see cref="ITypeReader"/>.</typeparam>
        /// <returns>The same instance for chaining calls.</returns>
        public TypeReaderDictionary Exclude<T>()
            => Exclude(typeof(T));

        /// <summary>
        ///     Excludes an <see cref="ITypeReader"/> from the <see cref="TypeReaderDictionary"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to remove the <see cref="ITypeReader"/>.</param>
        /// <returns>The same instance for chaining calls.</returns>
        public TypeReaderDictionary Exclude(Type type)
        {
            _typeReaders.Remove(type);
            return this;
        }

        /// <summary>
        ///     Tries to get a <see cref="ITypeReader"/> from the underlying dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns>True if success. False if not.</returns>
        public bool TryGetValue<T>(out ITypeReader reader)
            => TryGetValue(typeof(T), out reader);

        /// <summary>
        ///     Tries to get a <see cref="ITypeReader"/> from the underlying dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns>True if success. False if not.</returns>
        public bool TryGetValue(Type type, out ITypeReader reader)
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
        ///     Copies all keys in the current dictionary to another, overwriting existing keys.
        /// </summary>
        /// <param name="targetDictionary">The target dictionary to copy to.</param>
        public void CopyTo(TypeReaderDictionary targetDictionary)
        {
            foreach (var kvp in _typeReaders)
                targetDictionary[kvp.Key] = kvp.Value;
        }
    }
}
