﻿using System;
using System.Diagnostics;
using System.Globalization;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents an instance of the Number object.
    /// </summary>
    /// <remarks>
    /// None of the methods of the Number prototype are generic; they should throw <c>TypeError</c>
    /// if the <c>this</c> value is not a Number object or a number primitive.
    /// </remarks>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Type = "{DebuggerDisplayType,nq}")]
    [DebuggerTypeProxy(typeof(ObjectInstanceDebugView))]
    public partial class NumberInstance : ObjectInstance
    {
        /// <summary>
        /// The primitive value.
        /// </summary>
        private double value;


        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Number instance and initializes it to the given value.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="value"> The value to initialize to. </param>
        public NumberInstance(ObjectInstance prototype, double value)
            : base(prototype)
        {
            this.value = value;
        }

        /// <summary>
        /// Creates the Number prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine, NumberConstructor constructor)
        {
            var result = engine.Object.Construct();
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            result.InitializeProperties(properties);
            return result;
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the primitive value of the number.
        /// </summary>
        public double Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayValue
        {
            get
            {
                return this.Value.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window when this object is part of array, map, etc.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayShortValue
        {
            get { return this.DebuggerDisplayValue; }
        }

        /// <summary>
        /// Gets type, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayType
        {
            get { return "Number"; }
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns a string representing a number represented in exponential notation.
        /// </summary>
        /// <param name="fractionDigits"> Number of digits after the decimal point. Must be in the
        /// range 0 – 20, inclusive.  Defaults to the number of digits necessary to specify the
        /// number. </param>
        /// <returns> A string representation of a number in exponential notation. The string
        /// contains one digit before the significand's decimal point, and may contain
        /// fractionDigits digits after it. </returns>
        [JSInternalFunction(Name = "toExponential")]
        public string ToExponential(object fractionDigits)
        {
            // If precision is undefined, the number of digits is dependant on the number.
            if (TypeUtilities.IsUndefined(fractionDigits))
                return NumberFormatter.ToString(this.value, 10, NumberFormatter.Style.Exponential, -1);

            // Convert the parameter to an integer.
            int fractionDigits2 = TypeConverter.ToInteger(fractionDigits);

            // Check the parameter is within range.
            if (fractionDigits2 < 0 || fractionDigits2 > 20)
                throw new JavaScriptException(ErrorType.RangeError, "toExponential() argument must be between 0 and 20.");

            // NumberFormatter does the hard work.
            return NumberFormatter.ToString(this.value, 10, NumberFormatter.Style.Exponential, fractionDigits2);
        }

        /// <summary>
        /// Returns a string representing a number in fixed-point notation.
        /// </summary>
        /// <param name="fractionDigits"> Number of digits after the decimal point. Must be in the
        /// range 0 – 20, inclusive. </param>
        /// <returns> A string representation of a number in fixed-point notation. The string
        /// contains one digit before the significand's decimal point, and must contain
        /// fractionDigits digits after it.
        /// If fractionDigits is not supplied or undefined, the toFixed method assumes the value
        /// is zero. </returns>
        [JSInternalFunction(Name = "toFixed")]
        public string ToFixed(int fractionDigits = 0)
        {
            // Check the parameter is within range.
            if (fractionDigits < 0 || fractionDigits > 20)
                throw new JavaScriptException(ErrorType.RangeError, "toFixed() argument must be between 0 and 20.");

            // NumberFormatter does the hard work.
            return NumberFormatter.ToString(this.value, 10, NumberFormatter.Style.Fixed, fractionDigits);
        }

        /// <summary>
        /// Returns a string containing a locale-dependant version of the number.
        /// </summary>
        /// <returns> A string containing a locale-dependant version of the number. </returns>
        [JSInternalFunction(Name = "toLocaleString")]
        public new string ToLocaleString()
        {
            // NumberFormatter does the hard work.
            return NumberFormatter.ToString(this.value, 10, CultureInfo.CurrentCulture.NumberFormat, NumberFormatter.Style.Regular);
        }

        /// <summary>
        /// Returns a string containing a number represented either in exponential or fixed-point
        /// notation with a specified number of digits.
        /// </summary>
        /// <param name="precision"> The number of significant digits. Must be in the range 1 – 21,
        /// inclusive. </param>
        /// <returns> A string containing a number represented either in exponential or fixed-point
        /// notation with a specified number of digits. </returns>
        /// <remarks>
        /// For numbers in exponential notation, precision - 1 digits are returned after the
        /// decimal point. For numbers in fixed notation, precision significant digits are
        /// returned.
        /// If precision is not supplied or is undefined, the toString method is called instead.
        /// </remarks>
        [JSInternalFunction(Name = "toPrecision")]
        public string ToPrecision(object precision)
        {
            // If precision is undefined, delegate to "toString()".
            if (TypeUtilities.IsUndefined(precision))
                return this.ToStringJS();

            // Convert the parameter to an integer.
            int precision2 = TypeConverter.ToInteger(precision);

            // Check the precision is in range.
            if (precision2 < 1 || precision2 > 21)
                throw new JavaScriptException(ErrorType.RangeError, "toPrecision() argument must be between 0 and 21.");

            // NumberFormatter does the hard work.
            return NumberFormatter.ToString(this.value, 10, NumberFormatter.Style.Precision, precision2);
        }

        /// <summary>
        /// Returns the textual representation of the number.
        /// </summary>
        /// <param name="radix"> Specifies a radix for converting numeric values to strings. </param>
        /// <returns> The textual representation of the number. </returns>
        [JSInternalFunction(Name = "toString")]
        public string ToStringJS(int radix = 10)
        {
            // Check the parameter is in range.
            if (radix < 2 || radix > 36)
                throw new JavaScriptException(ErrorType.RangeError, "The radix must be between 2 and 36, inclusive.");

            // NumberFormatter does the hard work.
            return NumberFormatter.ToString(this.value, radix, NumberFormatter.Style.Regular);
        }

        /// <summary>
        /// Returns the primitive value of the specified object.
        /// </summary>
        /// <returns> The primitive value of the specified object. </returns>
        [JSInternalFunction(Name = "valueOf")]
        public new double ValueOf()
        {
            return this.value;
        }
        
        /// <summary>
        /// Calculates the number of leading zero bits in the integer representation of this
        /// number.
        /// </summary>
        /// <returns> The number of leading zero bits in the integer representation of this number. </returns>
        [JSInternalFunction(Name = "clz")]
        public int Clz()
        {
            uint x = (uint)this.value;

            // Propagate leftmost 1-bit to the right 
            x = x | (x >> 1); 
            x = x | (x >> 2); 
            x = x | (x >> 4); 
            x = x | (x >> 8); 
            x = x | (x >> 16);

            return sizeof(int) * 8 - CountOneBits(x);
        }

        /// <summary>
        /// Counts the number of set bits in an integer.
        /// </summary>
        /// <param name="x"> The integer. </param>
        /// <returns> The number of set bits in the integer. </returns>
        private static int CountOneBits(uint x)
        {
            x -= ((x >> 1) & 0x55555555);
            x = (((x >> 2) & 0x33333333) + (x & 0x33333333));
            x = (((x >> 4) + x) & 0x0f0f0f0f);
            x += (x >> 8);
            x += (x >> 16);
            return (int)(x & 0x0000003f);
        }
    }
}
