namespace Jurassic.Library
{

    /// <summary>
    /// Represents a the value of an accessor property.
    /// </summary>
    internal sealed class PropertyAccessorValue
    {
        private readonly FunctionInstance getter;
        private readonly FunctionInstance setter;

        /// <summary>
        /// Creates a new PropertyAccessorValue instance.
        /// </summary>
        /// <param name="getter"> The getter function, or <c>null</c> if no getter was provided. </param>
        /// <param name="setter"> The setter function, or <c>null</c> if no setter was provided. </param>
        public PropertyAccessorValue(FunctionInstance getter, FunctionInstance setter)
        {
            this.getter = getter;
            this.setter = setter;
        }

        /// <summary>
        /// Gets the function that is called when the property value is retrieved.
        /// </summary>
        public FunctionInstance Getter
        {
            get { return this.getter; }
        }

        /// <summary>
        /// Gets the function that is called when the property value is modified.
        /// </summary>
        public FunctionInstance Setter
        {
            get { return this.setter; }
        }

        /// <summary>
        /// Gets the property value by calling the getter, if one is present.
        /// </summary>
        /// <param name="thisObject"> The value of the "this" keyword inside the getter. </param>
        /// <returns> The property value returned by the getter. </returns>
        public object GetValue(object thisObject)
        {
            if (this.getter == null)
                return Undefined.Value;
            return this.getter.CallLateBound(thisObject);
        }

        /// <summary>
        /// Sets the property value by calling the setter, if one is present.
        /// </summary>
        /// <param name="thisObject"> The value of the "this" keyword inside the setter. </param>
        /// <param name="value"> The desired value. </param>
        public void SetValue(object thisObject, object value)
        {
            if (this.setter == null)
                return;
            this.setter.CallLateBound(thisObject, value);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            if (getter == null)
                return "setter";
            else if (setter == null)
                return "getter";
            return "getter, setter";
        }
    }

}
