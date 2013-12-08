using System;
using System.Collections;
using System.Collections.Generic;

namespace Jurassic
{
    /// <summary>
    /// Represents a basic copy of the .NET 4 HashSet class.
    /// </summary>
    /// <typeparam name="T"> The type of each element in the HashSet. </typeparam>
    public class HashSet<T> : ICollection<T>
    {
        private Dictionary<T, bool> _dictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class that is empty and uses
        /// the default equality comparer for the set type.
        /// </summary>
        public HashSet()
        {
            _dictionary = new Dictionary<T, bool>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class that is empty and uses
        /// the specified equality comparer for the set type.
        /// </summary>
        /// <param name="comparer"> The <see cref="IEqualityComparer{T}"/> implementation to use
        /// when comparing values in the set, or <c>null</c> to use the default
        /// <see cref="EqualityComparer{T}"/> implementation for the set type. </param>
        public HashSet(IEqualityComparer<T> comparer)
        {
            _dictionary = new Dictionary<T, bool>(comparer);
        }

        // Methods

        /// <summary>
        /// Adds the specified element to a set.
        /// </summary>
        /// <param name="item"> The element to add to the set. </param>
        public void Add(T item)
        {
            // We don't care for the value in dictionary, only the keys matter.
            _dictionary[item] = true;         
        }

        /// <summary>
        /// Removes all elements from a <see cref="HashSet{T}"/> object.
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
        }

        /// <summary>
        /// Determines whether a <see cref="HashSet{T}"/> object contains the specified element.
        /// </summary>
        /// <param name="item"> The element to locate in the <see cref="HashSet{T}"/> object. </param>
        /// <returns> <c>true</c> if the HashSet<T> object contains the specified element;
        /// otherwise, <c>false</c>. </returns>
        public bool Contains(T item)
        {
            return _dictionary.ContainsKey(item);
        }

        /// <summary>
        /// Copies the elements of a <see cref="HashSet{T}"/> object to an array, starting at the
        /// specified array index.
        /// </summary>
        /// <param name="array"> The one-dimensional array that is the destination of the elements
        /// copied from the <see cref="HashSet{T}"/> object. The array must have zero-based indexing. </param>
        /// <param name="arrayIndex"> The zero-based index in <paramref name="array"/> at which
        /// copying begins. </param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the specified element from a <see cref="HashSet{T}"/> object.
        /// </summary>
        /// <param name="item"> The element to remove. </param>
        /// <returns> <c>true</c> if the element is successfully found and removed; otherwise,
        /// <c>false</c>. This method returns <c>false</c> if item is not found in the
        /// <see cref="HashSet{T}"/> object. </returns>
        public bool Remove(T item)
        {
            return _dictionary.Remove(item);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a <see cref="HashSet{T}"/> object.
        /// </summary>
        /// <returns> A <see cref="HashSet{T}.Enumerator"/> object for the <see cref="HashSet{T}"/> object. </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _dictionary.Keys.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns> An IEnumerator object that can be used to iterate through the collection. </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.Keys.GetEnumerator();
        }

        // Properties

        /// <summary>
        /// Gets the number of elements that are contained in a set.
        /// </summary>
        public int Count
        {
            get { return _dictionary.Keys.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether a collection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
    }
}
