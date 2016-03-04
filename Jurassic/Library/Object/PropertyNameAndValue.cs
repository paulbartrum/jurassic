namespace Jurassic.Library
{
    /// <summary>
    /// Represents a property name and value.
    /// </summary>
    public sealed class PropertyNameAndValue
    {
        private object key;
        private PropertyDescriptor descriptor;

        /// <summary>
        /// Initializes a property with any descriptor.
        /// </summary>
        /// <param name="key"> The property key (either a string or a Symbol). </param>
        /// <param name="descriptor"> A descriptor describing the property. </param>
        public PropertyNameAndValue(object key, PropertyDescriptor descriptor)
        {
            this.key = key;
            this.descriptor = descriptor;
        }

        /// <summary>
        /// Initializes a simple property.
        /// </summary>
        /// <param name="key"> The property key (either a string or a Symbol). </param>
        /// <param name="value"> The property value. </param>
        /// <param name="attributes"> Indicates whether the property is readable, writable and/or enumerable. </param>
        public PropertyNameAndValue(object key, object value, PropertyAttributes attributes)
        {
            this.key = key;
            this.descriptor = new PropertyDescriptor(value, attributes);
        }

        /// <summary>
        /// Initializes a getter/setter property.
        /// </summary>
        /// <param name="key"> The property key (either a string or a Symbol). </param>
        /// <param name="getter"> The function to call to retrieve the property value. </param>
        /// <param name="setter"> The function to call to set the property value. </param>
        /// <param name="attributes"> Indicates whether the property is readable, writable and/or enumerable. </param>
        public PropertyNameAndValue(object key, FunctionInstance getter, FunctionInstance setter, PropertyAttributes attributes)
        {
            this.key = key;
            this.descriptor = new PropertyDescriptor(getter, setter, attributes);
        }

        /// <summary>
        /// Gets the property key.
        /// </summary>
        public object Key
        {
            get { return this.key; }
        }

        /// <summary>
        /// Gets the value of the property.
        /// </summary>
        public object Value
        {
            get { return this.descriptor.Value; }
        }

        /// <summary>
        /// Gets the property attributes.  These attributes describe how the property can
        /// be modified.
        /// </summary>
        public PropertyAttributes Attributes
        {
            get { return this.descriptor.Attributes; }
        }

        /// <summary>
        /// Gets a boolean value indicating whether the property value can be set.
        /// </summary>
        public bool IsWritable
        {
            get { return this.descriptor.IsWritable; }
        }

        /// <summary>
        /// Gets a boolean value indicating whether the property value will be included during an
        /// enumeration.
        /// </summary>
        public bool IsEnumerable
        {
            get { return this.descriptor.IsEnumerable; }
        }

        /// <summary>
        /// Gets a boolean value indicating whether the property can be deleted.
        /// </summary>
        public bool IsConfigurable
        {
            get { return this.descriptor.IsConfigurable; }
        }
    }
}
