using System;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents the built-in javascript Number object.
    /// </summary>
    [Serializable]
    public partial class NumberConstructor : ClrStubFunction
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Number object.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal NumberConstructor(ObjectInstance prototype)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            // Initialize the constructor properties.
            var properties = GetDeclarativeProperties();
            InitializeConstructorProperties(properties, "Number", 1, new NumberInstance(this));
            FastSetProperties(properties);
        }



        //     JAVASCRIPT INTERNAL METHODS
        //_________________________________________________________________________________________

        /// <summary>
        /// Called when the Number object is invoked like a function, e.g. var x = Number("5").
        /// Returns zero.
        /// </summary>
        [JSCallFunction]
        public double Call()
        {
            return 0;
        }

        /// <summary>
        /// Called when the Number object is invoked like a function, e.g. var x = Number("5").
        /// Converts the given argument into a number value (not a Number object).
        /// </summary>
        [JSCallFunction]
        public double Call(double value)
        {
            return value;
        }

        /// <summary>
        /// Creates a new Number instance and initializes it to zero.
        /// </summary>
        [JSConstructorFunction]
        public NumberInstance Construct()
        {
            return new NumberInstance(this.InstancePrototype, 0);
        }

        /// <summary>
        /// Creates a new Number instance and initializes it to the given value.
        /// </summary>
        /// <param name="value"> The value to initialize to. </param>
        [JSConstructorFunction]
        public NumberInstance Construct(double value)
        {
            return new NumberInstance(this.InstancePrototype, value);
        }



        //     JAVASCRIPT FIELDS
        //_________________________________________________________________________________________

        /// <summary>
        /// The largest representable number, approximately 1.8e+308.
        /// </summary>
        [JSField]
        public const double MAX_VALUE = double.MaxValue;

        /// <summary>
        /// The smallest positive representable number, approximately 5e-324.
        /// </summary>
        [JSField]
        public const double MIN_VALUE = double.Epsilon;

        /// <summary>
        /// Special "not a number" value.
        /// </summary>
        [JSField]
        public const double NaN = double.NaN;

        /// <summary>
        /// Special value representing negative infinity.
        /// </summary>
        [JSField]
        public const double NEGATIVE_INFINITY = double.NegativeInfinity;

        /// <summary>
        /// Special value representing positive infinity.
        /// </summary>
        [JSField]
        public const double POSITIVE_INFINITY = double.PositiveInfinity;

        /// <summary>
        /// The difference between 1 and the smallest value greater than 1 that is representable as
        /// a numeric value.
        /// </summary>
        [JSField]
        public const double EPSILON = 2.2204460492503130808472633361816e-16;

        /// <summary>
        /// The maximum integer within the range of integers that can be represented exactly.
        /// Outside the safe range multiple integers are mapped to a single value.
        /// </summary>
        [JSField]
        public const double MAX_SAFE_INTEGER = 9007199254740991;

        /// <summary>
        /// The minimum integer within the range of integers that can be represented exactly.
        /// Outside the safe range multiple integers are mapped to a single value.
        /// </summary>
        [JSField]
        public const double MIN_SAFE_INTEGER = -9007199254740991;
        


        /// <summary>
        /// Determines whether the given number is finite.
        /// </summary>
        /// <param name="value"> The number to test. </param>
        /// <returns> <c>false</c> if the number is NaN or positive or negative infinity,
        /// <c>true</c> otherwise.  <c>false</c> if the value is not a number. </returns>
        [JSInternalFunction(Name = "isFinite")]
        public static bool IsFinite(object value)
        {
            if (value is int || value is uint)
                return true;
            if (value is double)
                return double.IsNaN((double)value) == false && double.IsInfinity((double)value) == false;
            return false;
        }

        /// <summary>
        /// Determines whether the given number is NaN.
        /// </summary>
        /// <param name="value"> The number to test. </param>
        /// <returns> <c>true</c> if the number is NaN, <c>false</c> otherwise. </returns>
        [JSInternalFunction(Name = "isNaN")]
        public static bool IsNaN(object value)
        {
            if (value is double)
                return double.IsNaN((double)value);
            return false;
        }

        /// <summary>
        /// Determines whether the given number is an integer.
        /// </summary>
        /// <param name="value"> The number to test. </param>
        /// <returns> <c>true</c> if the number is an integer, <c>false</c> otherwise. </returns>
        [JSInternalFunction(Name = "isInteger")]
        public static bool IsInteger(object value)
        {
            if (value is int || value is uint)
                return true;
            if (value is double)
            {
                double number = (double)value;
                if (double.IsInfinity(number))
                    return false;
                return Math.Floor(number) == number;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the given number is within the "safe" integer range.
        /// </summary>
        /// <param name="value"> The number to test. </param>
        /// <returns> <c>true</c> if the number is a safe integer, <c>false</c> otherwise. </returns>
        [JSInternalFunction(Name = "isSafeInteger")]
        public static bool IsSafeInteger(object value)
        {
            if (value is int || value is uint)
                return true;
            if (value is double)
            {
                double number = (double)value;
                if (double.IsInfinity(number))
                    return false;
                return (Math.Floor(number) == number) && number >= MIN_SAFE_INTEGER && number <= MAX_SAFE_INTEGER;
            }
            return false;
        }

        /// <summary>
        /// Parses the given string and returns the equivalent integer value. 
        /// </summary>
        /// <param name="engine"> The associated script engine. </param>
        /// <param name="input"> The string to parse. </param>
        /// <param name="radix"> The numeric base to use for parsing.  Pass zero to use base 10
        /// except when the input string starts with '0' in which case base 16 or base 8 are used
        /// instead (base 8 is only supported in compatibility mode). </param>
        /// <returns> The equivalent integer value of the given string. </returns>
        /// <remarks> Leading whitespace is ignored.  Parsing continues until the first invalid
        /// character, at which point parsing stops.  No error is returned in this case. </remarks>
        [JSInternalFunction(Name = "parseInt", Flags = JSFunctionFlags.HasEngineParameter)]
        public static double ParseInt(ScriptEngine engine, string input, [DefaultParameterValue(0.0)] double radix = 0)
        {
            return GlobalObject.ParseInt(engine, input, radix);
        }

        /// <summary>
        /// Parses the given string and returns the equivalent numeric value. 
        /// </summary>
        /// <param name="input"> The string to parse. </param>
        /// <returns> The equivalent numeric value of the given string. </returns>
        /// <remarks> Leading whitespace is ignored.  Parsing continues until the first invalid
        /// character, at which point parsing stops.  No error is returned in this case. </remarks>
        [JSInternalFunction(Name = "parseFloat")]
        public static double ParseFloat(string input)
        {
            return GlobalObject.ParseFloat(input);
        }
    }
}
