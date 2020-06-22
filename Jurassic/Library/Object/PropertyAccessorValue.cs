﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jurassic.Library
{

    /// <summary>
    /// Represents a the value of an accessor property.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Type = "{DebuggerDisplayType,nq}")]
    internal sealed class PropertyAccessorValue : IDebuggerDisplay
    {
        private FunctionInstance getter;
        private FunctionInstance setter;

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
        /// <param name="argumentValues"></param>
        /// <returns> The property value returned by the getter. </returns>
        public object GetValue(ObjectInstance thisObject, params object[] argumentValues)
        {
            if (this.getter == null)
                return Undefined.Value;
            return this.getter.CallLateBound(thisObject, argumentValues);
        }

        /// <summary>
        /// Sets the property value by calling the setter, if one is present.
        /// </summary>
        /// <param name="thisObject"> The value of the "this" keyword inside the setter. </param>
        /// <param name="argumentValues"> The values as arguments list. </param>
        public void SetValue(ObjectInstance thisObject, params object[] argumentValues)
        {
            if (this.setter == null)
                return;
            this.setter.CallLateBound(thisObject, argumentValues);
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerDisplayValue
        {
            get
            {
                string result = string.Empty;
                if (this.getter != null && this.setter != null)
                {
                    result = "getter;setter";
                }
                else if (this.getter != null)
                {
                    result = "getter";
                }
                else if (this.setter != null)
                {
                    result = "getter";
                }
                return result;
            }
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window when this object is part of array, map, etc.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerDisplayShortValue
        {
            get { return this.DebuggerDisplayValue; }
        }

        /// <summary>
        /// Gets type, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerDisplayType
        {
            get { return "Property Accessor"; }
        }
    }

}
