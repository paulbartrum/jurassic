using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Jurassic.Library
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public partial class TypedArrayInstance : ObjectInstance
    {



        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new typed array instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        public TypedArrayInstance(ObjectInstance prototype)
            : base(prototype)
        {
        }

        /// <summary>
        /// Creates the TypedArray prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine, TypedArrayConstructor constructor)
        {
            var result = engine.Object.Construct();
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            result.FastSetProperties(properties);
            return result;
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the internal class name of the object.  Used by the default toString()
        /// implementation.
        /// </summary>
        protected override string InternalClassName
        {
            get { return ((TypedArrayConstructor)this["constructor"]).Name; }
        }



        //     OVERRIDES
        //_________________________________________________________________________________________

        ///// <summary>
        ///// Gets a descriptor for the property with the given array index.
        ///// </summary>
        ///// <param name="index"> The array index of the property. </param>
        ///// <returns> A property descriptor containing the property value and attributes. </returns>
        ///// <remarks> The prototype chain is not searched. </remarks>
        //public override PropertyDescriptor GetOwnPropertyDescriptor(uint index)
        //{
        //    if (index < this.value.Length)
        //    {
        //        var result = this.value[(int)index].ToString();
        //        return new PropertyDescriptor(result, PropertyAttributes.Enumerable);
        //    }

        //    // Delegate to the base class.
        //    return base.GetOwnPropertyDescriptor(index);
        //}

        ///// <summary>
        ///// Gets an enumerable list of every property name and value associated with this object.
        ///// </summary>
        //public override IEnumerable<PropertyNameAndValue> Properties
        //{
        //    get
        //    {
        //        // Enumerate array indices.
        //        for (int i = 0; i < this.value.Length; i++)
        //            yield return new PropertyNameAndValue(i.ToString(), this.value[i].ToString(), PropertyAttributes.Enumerable);

        //        // Delegate to the base implementation.
        //        foreach (var nameAndValue in base.Properties)
        //            yield return nameAndValue;
        //    }
        //}



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Fills all the elements of a typed array from a start index to an end index with a
        /// static value.
        /// </summary>
        /// <param name="value"> The value to fill the typed array with. </param>
        /// <param name="start"> Optional. Start index. Defaults to 0. </param>
        /// <param name="end"> Optional. End index (exclusive). Defaults to the length of the array. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "fill")]
        public static string Fill(object value, int start = 0, int end = -1)
        {
            throw new NotImplementedException();
        }

    }
}
