using System;
using System.Collections.Generic;
using System.Text;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents an instance of the JavaScript string object.
    /// </summary>
    public partial class StringInstance : ObjectInstance
    {
        private readonly string value;



        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new string instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="value"> The value to initialize the instance. </param>
        public StringInstance(ObjectInstance prototype, string value)
            : base(prototype)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            this.value = value;
            this.FastSetProperty("length", value.Length);
        }

        /// <summary>
        /// Creates the string prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine, StringConstructor constructor)
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
        /// Gets the primitive value of this object.
        /// </summary>
        public string Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the number of characters in the string.
        /// </summary>
        public int Length
        {
            get { return this.value.Length; }
        }



        //     OVERRIDES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets a descriptor for the property with the given array index.
        /// </summary>
        /// <param name="index"> The array index of the property. </param>
        /// <returns> A property descriptor containing the property value and attributes. </returns>
        /// <remarks> The prototype chain is not searched. </remarks>
        public override PropertyDescriptor GetOwnPropertyDescriptor(uint index)
        {
            if (index < this.value.Length)
            {
                var result = this.value[(int)index].ToString();
                return new PropertyDescriptor(result, PropertyAttributes.Enumerable);
            }

            // Delegate to the base class.
            return base.GetOwnPropertyDescriptor(index);
        }

        /// <summary>
        /// Defines or redefines the value and attributes of a property.  The prototype chain is
        /// not searched so if the property exists but only in the prototype chain a new property
        /// will be created.
        /// </summary>
        /// <param name="key"> The property key of the property to modify. </param>
        /// <param name="descriptor"> The property value and attributes. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set.  This can happen if the property is not configurable or the object is sealed. </param>
        /// <returns> <c>true</c> if the property was successfully modified; <c>false</c> otherwise. </returns>
        public override bool DefineProperty(object key, PropertyDescriptor descriptor, bool throwOnError)
        {
            // Check if the property is an indexed property.
            uint arrayIndex = ArrayInstance.ParseArrayIndex(key);
            if (arrayIndex < this.Length)
                return IsCompatiblePropertyDescriptor(IsExtensible, descriptor, GetOwnPropertyDescriptor(arrayIndex));

            // Delegate to the base class.
            return base.DefineProperty(key, descriptor, throwOnError);
        }

        /// <summary>
        /// Gets an enumerable list of every property name associated with this object.
        /// Does not include properties in the prototype chain.
        /// </summary>
        public override IEnumerable<object> OwnKeys
        {
            get
            {
                // Enumerate array indices.
                for (int i = 0; i < this.value.Length; i++)
                    yield return i.ToString();

                // Delegate to the base implementation.
                foreach (var key in base.OwnKeys)
                    yield return key;
            }
        }


        
        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns the character at the specified index.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="index"> The character position (starts at 0). </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "charAt", Flags = JSFunctionFlags.HasThisObject)]
        public static string CharAt(string thisObject, int index)
        {
            if (index < 0 || index >= thisObject.Length)
                return string.Empty;
            return thisObject[index].ToString();
        }

        /// <summary>
        /// Returns a number indicating the 16-bit UTF-16 character code at the given index.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="index"> The character position (starts at 0). </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "charCodeAt", Flags = JSFunctionFlags.HasThisObject)]
        public static double CharCodeAt(string thisObject, int index)
        {
            if (index < 0 || index >= thisObject.Length)
                return double.NaN;
            return (double)(int)thisObject[index];
        }

        /// <summary>
        /// Returns a number indicating the Unicode code point of the character at the given index.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="index"> The character position (starts at 0). </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "codePointAt", Flags = JSFunctionFlags.HasThisObject)]
        public static double CodePointAt(string thisObject, int index)
        {
            if (index < 0 || index >= thisObject.Length)
                return double.NaN;
            int firstCodePoint = (int) thisObject[index];
            if (firstCodePoint < 0xD800 || firstCodePoint > 0xDBFF || index + 1 == thisObject.Length)
                return firstCodePoint;
            int secondCodePoint = (int) thisObject[index + 1];
            if (secondCodePoint < 0xDC00 || secondCodePoint > 0xDFFF)
                return firstCodePoint;
            return (double)((firstCodePoint - 0xD800) * 1024 + (secondCodePoint - 0xDC00) + 0x10000);
        }

        /// <summary>
        /// Combines the text of two or more strings and returns a new string.
        /// </summary>
        /// <param name="engine"> The current script environment. </param>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="strings"> The strings to concatenate with this string. </param>
        /// <returns> The result of combining this string with the given strings. </returns>
        [JSInternalFunction(Name = "concat", Flags = JSFunctionFlags.HasEngineParameter | JSFunctionFlags.HasThisObject)]
        public static ConcatenatedString Concat(ScriptEngine engine, object thisObject, params object[] strings)
        {
            if (thisObject is ConcatenatedString)
            {
                // Append the strings together.
                ConcatenatedString result = (ConcatenatedString)thisObject;
                if (strings.Length == 0)
                    return result;
                result = result.Concatenate(strings[0]);
                for (int i = 1; i < strings.Length; i ++)
                    result.Append(strings[i]);
                return result;
            }
            else
            {
                // Convert "this" to a string.
                TypeUtilities.VerifyThisObject(engine, thisObject, "concat");
                var thisObject2 = TypeConverter.ToString(thisObject);

                // Append the strings together.
                var result = new ConcatenatedString(thisObject2);
                foreach (object str in strings)
                    result.Append(str);
                return result;
            }
        }

        
        /// <summary>
        /// Returns <c>true</c> if the calling String object contains the given string.
        /// </summary>
        /// <param name="engine"> The script engine. </param>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="substring"> The substring to search for. </param>
        /// <param name="startIndex"> The character position within the string to start searching. </param>
        /// <returns> <c>true</c> if the substring was found; <c>false</c> otherwise. </returns>
        [JSInternalFunction(Name = "includes", Flags = JSFunctionFlags.HasEngineParameter | JSFunctionFlags.HasThisObject, Length = 1)]
        public static bool Includes(ScriptEngine engine, string thisObject, object substring, int startIndex = 0)
        {
            if (IsRegExp(substring))
                throw new JavaScriptException(ErrorType.TypeError, "Substring argument must not be a regular expression.");
            return IndexOf(thisObject, TypeConverter.ToString(substring), startIndex) >= 0;
        }

        
        /// <summary>
        /// Returns the index within the calling String object of the first occurrence of the specified value, or -1 if not found.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="substring"> The substring to search for. </param>
        /// <param name="startIndex"> The character position to start searching from.  Defaults to 0. </param>
        /// <returns> The character position of the start of the substring, if it was found, or -1 if it wasn't. </returns>
        [JSInternalFunction(Name = "indexOf", Flags = JSFunctionFlags.HasThisObject, Length = 1)]
        public static int IndexOf(string thisObject, string substring, int startIndex = 0)
        {
            startIndex = Math.Min(Math.Max(startIndex, 0), thisObject.Length);
            return thisObject.IndexOf(substring, startIndex, StringComparison.Ordinal);
        }

        

        /// <summary>
        /// Returns the index within the calling String object of the specified value, searching
        /// backwards from the end of the string.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="substring"> The substring to search for. </param>
        /// <param name="startIndex"> The index of the character to start searching. </param>
        /// <returns> The index of the substring, or <c>-1</c> if not found. </returns>
        [JSInternalFunction(Name = "lastIndexOf", Flags = JSFunctionFlags.HasThisObject, Length = 1)]
        public static int LastIndexOf(string thisObject, string substring, double startIndex = double.NaN)
        {
            // Limit startIndex to the length of the string.  This must be done first otherwise
            // when startIndex = MaxValue it wraps around to negative.
            int startIndex2 = double.IsNaN(startIndex) ? int.MaxValue : TypeConverter.ToInteger(startIndex);
            startIndex2 = Math.Min(startIndex2, thisObject.Length - 1);
            startIndex2 = Math.Min(startIndex2 + substring.Length - 1, thisObject.Length - 1);
            if (startIndex2 < 0)
            {
                if (thisObject == string.Empty && substring == string.Empty)
                    return 0;
                return -1;
            }
            return thisObject.LastIndexOf(substring, startIndex2, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns a number indicating whether a reference string comes before or after or is the
        /// same as the given string in sort order.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="str"> The string to compare with. </param>
        /// <returns> -1, 0 or 1 depending on whether the given string comes before or after or is
        /// the same as the given string in sort order. </returns>
        [JSInternalFunction(Name = "localeCompare", Flags = JSFunctionFlags.HasThisObject)]
        public static int LocaleCompare(string thisObject, string str)
        {
            return string.Compare(thisObject, str);
        }

        /// <summary>
        /// Finds the first match of the given substring within this string.
        /// </summary>
        /// <param name="engine"> The current script environment. </param>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="substrOrRegExp"> The substring or regular expression to search for. </param>
        /// <returns> An array containing the matched strings. </returns>
        [JSInternalFunction(Name = "match", Flags = JSFunctionFlags.HasEngineParameter | JSFunctionFlags.HasThisObject)]
        public static object Match(ScriptEngine engine, object thisObject, object substrOrRegExp)
        {
            if (thisObject == Null.Value || thisObject == Undefined.Value)
                throw new JavaScriptException(ErrorType.TypeError, "String.prototype.match called on null or undefined.");

            if (substrOrRegExp != Null.Value && substrOrRegExp != Undefined.Value)
            {
                // Get the [Symbol.match] property value.
                var matchFunctionObj = TypeConverter.ToObject(engine, substrOrRegExp)[Symbol.Match];
                if (matchFunctionObj != Undefined.Value)
                {
                    // If it's a function, call it and return the result.
                    if (matchFunctionObj is FunctionInstance matchFunction)
                        return matchFunction.CallLateBound(substrOrRegExp, thisObject);
                    else
                        throw new JavaScriptException(ErrorType.TypeError, "Symbol.match value is not a function.");
                }
            }

            // Convert the argument to a regex.
            var regex = new RegExpInstance(engine.RegExp.InstancePrototype, TypeConverter.ToString(substrOrRegExp, string.Empty));

            // Call the [Symbol.match] function.
            return regex.Match(TypeConverter.ToString(thisObject));
        }

        /// <summary>
        /// Returns a new string whose binary representation is in a particular Unicode normalization form.
        /// </summary>
        /// <param name="engine"> The current ScriptEngine instance. </param>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="form"> A Unicode normalization form. </param>
        /// <returns> A new string whose binary representation is in a particular Unicode normalization form. </returns>
        [JSInternalFunction(Name = "normalize", Flags = JSFunctionFlags.HasEngineParameter | JSFunctionFlags.HasThisObject)]
        public static string Normalize(ScriptEngine engine, string thisObject, string form = "NFC")
        {
            switch (form)
            {
                case "NFC":
                    return thisObject.Normalize(NormalizationForm.FormC);
                case "NFD":
                    return thisObject.Normalize(NormalizationForm.FormD);
                case "NFKC":
                    return thisObject.Normalize(NormalizationForm.FormKC);
                case "NFKD":
                    return thisObject.Normalize(NormalizationForm.FormKD);
            }
            throw new JavaScriptException(ErrorType.RangeError, "The normalization form should be one of NFC, NFD, NFKC, NFKD.");
        }

        /// <summary>
        /// Wraps the string in double quotes (").  Any existing double quotes in the string are
        /// escaped using the backslash character.
        /// </summary>
        /// <param name="thisObject"> The string to wrap. </param>
        /// <returns> The input string wrapped with double quotes and with existing double quotes
        /// escaped. </returns>
        [JSInternalFunction(Name = "quote", Flags = JSFunctionFlags.HasThisObject, NonStandard = true)]
        public static string Quote(string thisObject)
        {
            var result = new StringBuilder(thisObject.Length + 2);
            result.Append('"');
            for (int i = 0; i < thisObject.Length; i++)
            {
                char c = thisObject[i];
                if (c == '"')
                    result.Append('\\');
                result.Append(c);
            }
            result.Append('"');
            return result.ToString();
        }

        /// <summary>
        /// Substitutes the given string or regular expression with the given text or the result
        /// of a replacement function.
        /// </summary>
        /// <param name="engine"> The current script environment. </param>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="substrOrRegExp"> The substring to replace -or- a regular expression that
        /// matches the text to replace. </param>
        /// <param name="replaceTextOrFunction"> The text to substitute -or- a function that
        /// returns the text to substitute. </param>
        /// <returns> A copy of this string with text replaced. </returns>
        [JSInternalFunction(Name = "replace", Flags = JSFunctionFlags.HasEngineParameter | JSFunctionFlags.HasThisObject)]
        public static object Replace(ScriptEngine engine, object thisObject, object substrOrRegExp, object replaceTextOrFunction)
        {
            if (thisObject == Null.Value || thisObject == Undefined.Value)
                throw new JavaScriptException(ErrorType.TypeError, "String.prototype.replace called on null or undefined.");

            if (substrOrRegExp != Null.Value && substrOrRegExp != Undefined.Value)
            {
                // Get the [Symbol.replace] property value.
                var matchFunctionObj = TypeConverter.ToObject(engine, substrOrRegExp)[Symbol.Replace];
                if (matchFunctionObj != Undefined.Value)
                {
                    // If it's a function, call it and return the result.
                    if (matchFunctionObj is FunctionInstance matchFunction)
                        return matchFunction.CallLateBound(substrOrRegExp, thisObject, replaceTextOrFunction);
                    else
                        throw new JavaScriptException(ErrorType.TypeError, "Symbol.replace value is not a function.");
                }
            }

            if (replaceTextOrFunction is FunctionInstance)
                return Replace(TypeConverter.ToString(thisObject), TypeConverter.ToString(substrOrRegExp), (FunctionInstance)replaceTextOrFunction);
            else
                return Replace(TypeConverter.ToString(thisObject), TypeConverter.ToString(substrOrRegExp), TypeConverter.ToString(replaceTextOrFunction));
        }

        /// <summary>
        /// Returns a copy of this string with text replaced.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="substr"> The text to search for. </param>
        /// <param name="replaceText"> A string containing the text to replace for every successful
        /// match. </param>
        /// <returns> A copy of this string with text replaced. </returns>
        public static string Replace(string thisObject, string substr, string replaceText)
        {
            // Find the first occurrance of substr.
            int start = thisObject.IndexOf(substr, StringComparison.Ordinal);
            if (start == -1)
                return thisObject;
            int end = start + substr.Length;

            // Replace only the first match.
            var result = new StringBuilder(thisObject.Length + (replaceText.Length - substr.Length));
            result.Append(thisObject, 0, start);
            result.Append(replaceText);
            result.Append(thisObject, end, thisObject.Length - end);
            return result.ToString();
        }

        /// <summary>
        /// Returns a copy of this string with text replaced using a replacement function.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="substr"> The text to search for. </param>
        /// <param name="replaceFunction"> A function that is called to produce the text to replace
        /// for every successful match. </param>
        /// <returns> A copy of this string with text replaced. </returns>
        public static string Replace(string thisObject, string substr, FunctionInstance replaceFunction)
        {
            // Find the first occurrance of substr.
            int start = thisObject.IndexOf(substr, StringComparison.Ordinal);
            if (start == -1)
                return thisObject;
            int end = start + substr.Length;

            // Get the replacement text from the provided function.
            var replaceText = TypeConverter.ToString(replaceFunction.CallFromNative("replace", null, substr, start, thisObject));

            // Replace only the first match.
            var result = new StringBuilder(thisObject.Length + (replaceText.Length - substr.Length));
            result.Append(thisObject, 0, start);
            result.Append(replaceText);
            result.Append(thisObject, end, thisObject.Length - end);
            return result.ToString();
        }

        /// <summary>
        /// Returns the position of the first substring match.
        /// </summary>
        /// <param name="engine"> The current script environment. </param>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="substrOrRegExp"> The string or regular expression to search for. </param>
        /// <returns> The character position of the first match, or -1 if no match was found. </returns>
        [JSInternalFunction(Name = "search", Flags = JSFunctionFlags.HasEngineParameter | JSFunctionFlags.HasThisObject)]
        public static object Search(ScriptEngine engine, object thisObject, object substrOrRegExp)
        {
            if (thisObject == Null.Value || thisObject == Undefined.Value)
                throw new JavaScriptException(ErrorType.TypeError, "String.prototype.search called on null or undefined.");

            if (substrOrRegExp != Null.Value && substrOrRegExp != Undefined.Value)
            {
                // Get the [Symbol.search] property value.
                var matchFunctionObj = TypeConverter.ToObject(engine, substrOrRegExp)[Symbol.Search];
                if (matchFunctionObj != Undefined.Value)
                {
                    // If it's a function, call it and return the result.
                    if (matchFunctionObj is FunctionInstance matchFunction)
                        return matchFunction.CallLateBound(substrOrRegExp, thisObject);
                    else
                        throw new JavaScriptException(ErrorType.TypeError, "Symbol.search value is not a function.");
                }
            }

            // Convert the argument to a regex.
            var regex = new RegExpInstance(engine.RegExp.InstancePrototype, TypeConverter.ToString(substrOrRegExp, string.Empty));

            // Call the [Symbol.search] function.
            return regex.Search(TypeConverter.ToString(thisObject));
        }

        /// <summary>
        /// Extracts a section of the string and returns a new string.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="start"> The character position to start extracting. </param>
        /// <param name="end"> The character position to stop extacting. </param>
        /// <returns> A section of the string. </returns>
        [JSInternalFunction(Name = "slice", Flags = JSFunctionFlags.HasThisObject)]
        public static string Slice(string thisObject, int start, int end = int.MaxValue)
        {
            // Negative offsets are measured from the end of the string.
            if (start < 0)
                start += thisObject.Length;
            if (end < 0)
                end += thisObject.Length;

            // Constrain the parameters to within the limits of the string.
            start = Math.Min(Math.Max(start, 0), thisObject.Length);
            end = Math.Min(Math.Max(end, 0), thisObject.Length);
            if (end <= start)
                return string.Empty;

            return thisObject.Substring(start, end - start);
        }

        /// <summary>
        /// Splits this string into an array of strings by separating the string into substrings.
        /// </summary>
        /// <param name="engine"> The current script environment. </param>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="separator"> A string or regular expression that indicates where to split the string. </param>
        /// <param name="limit"> The maximum number of array items to return.  Defaults to unlimited. </param>
        /// <returns> An array containing the split strings. </returns>
        [JSInternalFunction(Name = "split", Flags = JSFunctionFlags.HasEngineParameter | JSFunctionFlags.HasThisObject)]
        public static object Split(ScriptEngine engine, object thisObject, object separator, object limit)
        {
            if (thisObject == Null.Value || thisObject == Undefined.Value)
                throw new JavaScriptException(ErrorType.TypeError, "String.prototype.split called on null or undefined.");

            if (separator != Null.Value && separator != Undefined.Value)
            {
                // Get the [Symbol.split] property value.
                var matchFunctionObj = TypeConverter.ToObject(engine, separator)[Symbol.Split];
                if (matchFunctionObj != Undefined.Value)
                {
                    // If it's a function, call it and return the result.
                    if (matchFunctionObj is FunctionInstance matchFunction)
                        return matchFunction.CallLateBound(separator, thisObject, limit);
                    else
                        throw new JavaScriptException(ErrorType.TypeError, "Symbol.split value is not a function.");
                }
            }

            // If separator is undefined, then don't match anything.
            if (separator == Undefined.Value)
                return engine.Array.New(new object[] { TypeConverter.ToString(thisObject) });

            // Call the strongly-typed string split method.
            var limitUint = limit == Undefined.Value ? uint.MaxValue : TypeConverter.ToUint32(limit);
            return Split(engine, TypeConverter.ToString(thisObject), TypeConverter.ToString(separator), limitUint);
        }

        /// <summary>
        /// Splits this string into an array of strings by separating the string into substrings.
        /// </summary>
        /// <param name="engine"> The current script environment. </param>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="separator"> A string that indicates where to split the string. </param>
        /// <param name="limit"> The maximum number of array items to return.  Defaults to unlimited. </param>
        /// <returns> An array containing the split strings. </returns>
        public static ArrayInstance Split(ScriptEngine engine, string thisObject, string separator, uint limit = uint.MaxValue)
        {
            if (string.IsNullOrEmpty(separator))
            {
                // If the separator is empty, split the string into individual characters.
                var result = engine.Array.New();
                for (int i = 0; i < thisObject.Length; i ++)
                    result[i] = thisObject[i].ToString();
                return result;
            }
            var splitStrings = thisObject.Split(new string[] { separator }, StringSplitOptions.None);
            if (limit < splitStrings.Length)
            {
                var splitStrings2 = new string[limit];
                Array.Copy(splitStrings, splitStrings2, (int)limit);
                splitStrings = splitStrings2;
            }
            return engine.Array.New(splitStrings);
        }

        /// <summary>
        /// Returns the characters in a string beginning at the specified location through the specified number of characters.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="start"> The character position to start extracting. </param>
        /// <param name="length"> The number of characters to extract. </param>
        /// <returns> A substring of this string. </returns>
        [JSInternalFunction(Name = "substr", Flags = JSFunctionFlags.HasThisObject, Deprecated = true)]
        public static string Substr(string thisObject, int start, int length = int.MaxValue)
        {
            // If start is less than zero, it is measured from the end of the string.
            if (start < 0)
                start = Math.Max(start + thisObject.Length, 0);

            // Compute the actual length.
            length = Math.Max(Math.Min(length, thisObject.Length - start), 0);
            if (length <= 0)
                return string.Empty;

            // Extract the substring.
            return thisObject.Substring(start, length);
        }

        /// <summary>
        /// Returns the characters in a string between two indexes into the string.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="start"> The character position to start extracting. </param>
        /// <param name="end"> The character position to stop extracting. </param>
        /// <returns> A substring of this string. </returns>
        [JSInternalFunction(Name = "substring", Flags = JSFunctionFlags.HasThisObject)]
        public static string Substring(string thisObject, int start, int end = int.MaxValue)
        {
            return Slice(thisObject, Math.Max(Math.Min(start, end), 0), Math.Max(Math.Max(start, end), 0));
        }

        /// <summary>
        /// Converts the characters within this string to lowercase while respecting the current
        /// locale.
        /// </summary>
        /// <returns> A copy of this string with the characters converted to lowercase. </returns>
        [JSInternalFunction(Name = "toLocaleLowerCase", Flags = JSFunctionFlags.HasThisObject)]
        public static string ToLocaleLowerCase(string thisObject)
        {
            return thisObject.ToLower();
        }

        /// <summary>
        /// Converts the characters within this string to uppercase while respecting the current
        /// locale.
        /// </summary>
        /// <returns> A copy of this string with the characters converted to uppercase. </returns>
        [JSInternalFunction(Name = "toLocaleUpperCase", Flags = JSFunctionFlags.HasThisObject)]
        public static string ToLocaleUpperCase(string thisObject)
        {
            return thisObject.ToUpper();
        }

        /// <summary>
        /// Returns the calling string value converted to lowercase.
        /// </summary>
        /// <returns> A copy of this string with the characters converted to lowercase. </returns>
        [JSInternalFunction(Name = "toLowerCase", Flags = JSFunctionFlags.HasThisObject)]
        public static string ToLowerCase(string thisObject)
        {
            return thisObject.ToLowerInvariant();
        }

        /// <summary>
        /// Returns a string representing the current object.
        /// </summary>
        /// <returns> A string representing the current object. </returns>
        [JSInternalFunction(Name = "toString")]
        public new string ToString()
        {
            return this.value;
        }

        /// <summary>
        /// Returns the calling string value converted to uppercase.
        /// </summary>
        /// <returns> A copy of this string with the characters converted to uppercase. </returns>
        [JSInternalFunction(Name = "toUpperCase", Flags = JSFunctionFlags.HasThisObject)]
        public static string ToUpperCase(string thisObject)
        {
            return thisObject.ToUpperInvariant();
        }

        private static char[] trimCharacters = new char[] {
            // Whitespace
            '\x09', '\x0B', '\x0C', '\x20', '\xA0', '\xFEFF',

            // Unicode space separator
            '\u1680', '\u180E', '\u2000', '\u2001',
            '\u2002', '\u2003', '\u2004', '\u2005',
            '\u2006', '\u2007', '\u2008', '\u2009',
            '\u200A', '\u202F', '\u205F', '\u3000', 

            // Line terminators
            '\x0A', '\x0D', '\u2028', '\u2029',
        };

        /// <summary>
        /// Trims whitespace from the beginning and end of the string.
        /// </summary>
        /// <returns></returns>
        [JSInternalFunction(Name = "trim", Flags = JSFunctionFlags.HasThisObject)]
        public static string Trim(string thisObject)
        {
            return thisObject.Trim(trimCharacters);
        }

        /// <summary>
        /// Trims whitespace from the beginning of the string.
        /// </summary>
        /// <returns></returns>
        [JSInternalFunction(Name = "trimStart", Flags = JSFunctionFlags.HasThisObject)]
        public static string TrimStart(string thisObject)
        {
            return thisObject.TrimStart(trimCharacters);
        }

        /// <summary>
        /// Trims whitespace from the beginning of the string.
        /// </summary>
        /// <returns></returns>
        [JSInternalFunction(Name = "trimEnd", Flags = JSFunctionFlags.HasThisObject)]
        public static string TrimEnd(string thisObject)
        {
            return thisObject.TrimEnd(trimCharacters);
        }

        /// <summary>
        /// Trims whitespace from the beginning of the string.
        /// </summary>
        /// <returns></returns>
        [JSInternalFunction(Name = "trimLeft", Flags = JSFunctionFlags.HasThisObject, NonStandard = true)]
        public static string TrimLeft(string thisObject)
        {
            return thisObject.TrimStart(trimCharacters);
        }

        /// <summary>
        /// Trims whitespace from the beginning of the string.
        /// </summary>
        /// <returns></returns>
        [JSInternalFunction(Name = "trimRight", Flags = JSFunctionFlags.HasThisObject, NonStandard = true)]
        public static string TrimRight(string thisObject)
        {
            return thisObject.TrimEnd(trimCharacters);
        }

        /// <summary>
        /// Returns the underlying primitive value of the current object.
        /// </summary>
        /// <returns> The underlying primitive value of the current object. </returns>
        [JSInternalFunction(Name = "valueOf")]
        public new string ValueOf()
        {
            return this.value;
        }



        //     ECMASCRIPT 6 FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Determines whether a string begins with the characters of another string.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="searchStringObj"> The characters to be searched for at the start of this string. </param>
        /// <param name="position"> The position at which to begin searching.  Defaults to zero. </param>
        /// <returns> <c>true</c> if this string starts with the given string, <c>false</c> otherwise. </returns>
        [JSInternalFunction(Name = "startsWith", Flags = JSFunctionFlags.HasThisObject, Length = 1)]
        public static bool StartsWith(string thisObject, object searchStringObj, int position = 0)
        {
            if (IsRegExp(searchStringObj))
                throw new JavaScriptException(ErrorType.TypeError, "Substring argument must not be a regular expression.");
            string searchString = TypeConverter.ToString(searchStringObj);
            if (position == 0)
                return thisObject.StartsWith(searchString);
            position = Math.Min(Math.Max(0, position), thisObject.Length);
            if (position + searchString.Length > thisObject.Length)
                return false;
            return thisObject.Substring(position, searchString.Length) == searchString;
        }

        /// <summary>
        /// Determines whether a string ends with the characters of another string.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="searchStringObj"> The characters to be searched for at the end of this string. </param>
        /// <param name="position"> Search within the string as if the string were only this long.
        /// Defaults to the string's actual length. </param>
        /// <returns> <c>true</c> if this string ends with the given string, <c>false</c> otherwise. </returns>
        [JSInternalFunction(Name = "endsWith", Flags = JSFunctionFlags.HasThisObject, Length = 1)]
        public static bool EndsWith(string thisObject, object searchStringObj, int position = int.MaxValue)
        {
            if (IsRegExp(searchStringObj))
                throw new JavaScriptException(ErrorType.TypeError, "Substring argument must not be a regular expression.");
            string searchString = TypeConverter.ToString(searchStringObj);
            if (position == int.MaxValue)
                return thisObject.EndsWith(searchString);
            position = Math.Min(Math.Max(0, position), thisObject.Length);
            if (searchString.Length > position)
                return false;
            return thisObject.Substring(position - searchString.Length, searchString.Length) == searchString;
        }

        /// <summary>
        /// Determines whether a string contains the characters of another string.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="searchString"> The characters to be searched for. </param>
        /// <param name="position"> The position at which to begin searching.  Defaults to zero. </param>
        /// <returns> <c>true</c> if this string contains the given string, <c>false</c> otherwise. </returns>
        [JSInternalFunction(Name = "contains", Flags = JSFunctionFlags.HasThisObject, Length = 1)]
        public static bool Contains(string thisObject, string searchString, int position = 0)
        {
            position = Math.Min(Math.Max(0, position), thisObject.Length);
            return thisObject.IndexOf(searchString, position) >= 0;
        }

        /// <summary>
        /// Repeats this string a number of times and returns the result.
        /// </summary>
        /// <param name="engine"> The current ScriptEngine instance. </param>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="count"> The number of times to repeat the string.  Must be zero or higher. </param>
        /// <returns> A repeated string. </returns>
        [JSInternalFunction(Name = "repeat", Flags = JSFunctionFlags.HasEngineParameter | JSFunctionFlags.HasThisObject)]
        public static string Repeat(ScriptEngine engine, string thisObject, int count)
        {
            if (count < 0 || count == int.MaxValue)
                throw new JavaScriptException(ErrorType.RangeError, "The count parameter is out of range.");
            var result = new StringBuilder();
            for (int i = 0; i < count; i ++)
                result.Append(thisObject);
            return result.ToString();
        }

        /// <summary>
        /// Returns an iterator that iterates over the code points of the string.
        /// </summary>
        /// <returns> An iterator for the string instance. </returns>
        [JSInternalFunction(Name = "@@iterator")]
        public ObjectInstance GetIterator()
        {
            return new StringIterator(Engine.StringIteratorPrototype, this.value);
        }



        //     JAVASCRIPT FUNCTIONS (HTML WRAPPER FUNCTIONS)
        //_________________________________________________________________________________________

        /// <summary>
        /// Wraps the string with an anchor tag.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="name"> The name of the anchor. </param>
        /// <returns> </returns>
        [JSInternalFunction(Name = "anchor", Flags = JSFunctionFlags.HasThisObject, NonStandard = true)]
        public static string Anchor(string thisObject, string name)
        {
            return string.Format(@"<a name=""{1}"">{0}</a>", thisObject, name.Replace("\"", "&quot;"));
        }

        /// <summary>
        /// Wraps the string with a big tag.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "big", Flags = JSFunctionFlags.HasThisObject, NonStandard = true)]
        public static string Big(string thisObject)
        {
            return string.Format("<big>{0}</big>", thisObject);
        }

        /// <summary>
        /// Wraps the string with a blink tag.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "blink", Flags = JSFunctionFlags.HasThisObject, NonStandard = true)]
        public static string Blink(string thisObject)
        {
            return string.Format("<blink>{0}</blink>", thisObject);
        }

        /// <summary>
        /// Wraps the string with a bold (b) tag.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "bold", Flags = JSFunctionFlags.HasThisObject, NonStandard = true)]
        public static string Bold(string thisObject)
        {
            return string.Format("<b>{0}</b>", thisObject);
        }

        /// <summary>
        /// Wraps the string with a tt tag.
        /// </summary>
        /// <returns></returns>
        [JSInternalFunction(Name = "fixed", Flags = JSFunctionFlags.HasThisObject, NonStandard = true)]
        public static string Fixed(string thisObject)
        {
            return string.Format("<tt>{0}</tt>", thisObject);
        }

        /// <summary>
        /// Wraps the string with a font tag that specifies the given color.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="colorValue"> The color value or name. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "fontcolor", Flags = JSFunctionFlags.HasThisObject, NonStandard = true)]
        public static string FontColor(string thisObject, string colorValue)
        {
            return string.Format(@"<font color=""{1}"">{0}</font>", thisObject, colorValue.Replace("\"", "&quot;"));
        }

        /// <summary>
        /// Wraps the string with a font tag that specifies the given font size.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="size"> The font size, specified as an integer. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "fontsize", Flags = JSFunctionFlags.HasThisObject, NonStandard = true)]
        public static string FontSize(string thisObject, string size)
        {
            return string.Format(@"<font size=""{1}"">{0}</font>", thisObject, size.Replace("\"", "&quot;"));
        }

        /// <summary>
        /// Wraps the string with a italics (i) tag.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "italics", Flags = JSFunctionFlags.HasThisObject, NonStandard = true)]
        public static string Italics(string thisObject)
        {
            return string.Format("<i>{0}</i>", thisObject);
        }

        /// <summary>
        /// Wraps the string with a hyperlink.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <param name="href"> The hyperlink URL. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "link", Flags = JSFunctionFlags.HasThisObject, NonStandard = true)]
        public static string Link(string thisObject, string href)
        {
            return string.Format(@"<a href=""{1}"">{0}</a>", thisObject, href.Replace("\"", "&quot;"));
        }

        /// <summary>
        /// Wraps the string in a <c>small</c> tag.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "small", Flags = JSFunctionFlags.HasThisObject, NonStandard = true)]
        public static string Small(string thisObject)
        {
            return string.Format("<small>{0}</small>", thisObject);
        }

        /// <summary>
        /// Wraps the string in a <c>strike</c> tag.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "strike", Flags = JSFunctionFlags.HasThisObject, NonStandard = true)]
        public static string Strike(string thisObject)
        {
            return string.Format("<strike>{0}</strike>", thisObject);
        }

        /// <summary>
        /// Wraps the string in a <c>sub</c> tag.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "sub", Flags = JSFunctionFlags.HasThisObject, NonStandard = true)]
        public static string Sub(string thisObject)
        {
            return string.Format("<sub>{0}</sub>", thisObject);
        }

        /// <summary>
        /// Wraps the string in a <c>sup</c> tag.
        /// </summary>
        /// <param name="thisObject"> The string that is being operated on. </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "sup", Flags = JSFunctionFlags.HasThisObject, NonStandard = true)]
        public static string Sup(string thisObject)
        {
            return string.Format("<sup>{0}</sup>", thisObject);
        }



        //     HELPER METHODS
        //_________________________________________________________________________________________

        /// <summary>
        /// Determines if the given object can be considered a RegExp-like object.
        /// </summary>
        /// <param name="o"> The object instance to check. </param>
        /// <returns> <c>true</c> if the object instance has [Symbol.match] value that is truthy;
        /// <c>false</c> otherwise. </returns>
        private static bool IsRegExp(object o)
        {
            if (o is ObjectInstance objectInstance)
            {
                // Get the [Symbol.match] property value.
                var match = objectInstance[Symbol.Match];
                if (match != Undefined.Value)
                    return TypeConverter.ToBoolean(match);
                return o is RegExpInstance;
            }
            else
                return false;
        }
    }
}
