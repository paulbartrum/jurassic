using System;

namespace Jurassic.Library
{

    /// <summary>
    /// Represents a property name. Used to speed up access to object properties and global variables.
    /// </summary>
    public sealed class PropertyReference
    {
        private string propertyName;

        /// <summary>
        /// Creates a new PropertyName instance.
        /// </summary>
        /// <param name="propertyName"> The name of the property to be accessed. </param>
        public PropertyReference(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));
            this.propertyName = propertyName;
        }

        /// <summary>
        /// The property name.
        /// </summary>
        public string Name
        {
            get { return propertyName; }
        }

        /// <summary>
        /// A reference to a schema that defines how properties are stored.
        /// </summary>
        internal HiddenClassSchema CachedSchema { get; private set; }

        /// <summary>
        /// The index into the property array.
        /// </summary>
        internal int CachedIndex { get; private set; }

        /// <summary>
        /// Caches property details.
        /// </summary>
        /// <param name="schema"> A reference to a schema that defines how properties are stored. </param>
        /// <param name="index"> The index into the property array. </param>
        internal void CachePropertyDetails(HiddenClassSchema schema, int index)
        {
            this.CachedSchema = schema;
            this.CachedIndex = index;
        }
        
        /// <summary>
        /// Clears the cached property details.
        /// </summary>
        internal void ClearCache()
        {
            this.CachedSchema = null;
        }

        /// <summary>
        /// Returns a textual representation of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }

}
