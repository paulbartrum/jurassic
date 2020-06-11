using System;
using System.Collections.Generic;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents the built-in javascript RegExp object.
    /// </summary>
    public partial class RegExpConstructor : ClrStubFunction
    {
        private string lastInput;
        private System.Text.RegularExpressions.Match lastMatch;


        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new RegExp object.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal RegExpConstructor(ObjectInstance prototype)
            : base(prototype, __STUB__Construct, __STUB__Call)
        {
            // Initialize the constructor properties.
            var properties = GetDeclarativeProperties(Engine);
            InitializeConstructorProperties(properties, "RegExp", 2, RegExpInstance.CreatePrototype(Engine, this));
            AddDeprecatedProperties(properties);
            InitializeProperties(properties);
        }


        //     JAVASCRIPT PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// A reference to the constructor function that is used to create derived objects.
        /// </summary>
        [JSProperty(Name = "@@species")]
        public FunctionInstance Species
        {
            get { return this; }
        }



        //     DEPRECATED PROPERTIES
        //_________________________________________________________________________________________

        // Deprecated properties:
        // $1, ..., $9          Parenthesized substring matches, if any.
        // input ($_)           The string against which a regular expression is matched.
        // lastMatch ($&)       The last matched characters.
        // lastParen ($+)       The last parenthesized substring match, if any.
        // leftContext ($`)     The substring preceding the most recent match.
        // rightContext ($')    The substring following the most recent match.

        /// <summary>
        /// Adds the deprecated RegExp properties to the given list.
        /// </summary>
        /// <param name="properties"> The list to add to. </param>
        private void AddDeprecatedProperties(List<PropertyNameAndValue> properties)
        {
            // Set the deprecated properties to their default values.
            this.AddDeprecatedProperty(properties, "$1", GetGroup1Adapter, PropertyAttributes.Enumerable);
            this.AddDeprecatedProperty(properties, "$2", GetGroup2Adapter, PropertyAttributes.Enumerable);
            this.AddDeprecatedProperty(properties, "$3", GetGroup3Adapter, PropertyAttributes.Enumerable);
            this.AddDeprecatedProperty(properties, "$4", GetGroup4Adapter, PropertyAttributes.Enumerable);
            this.AddDeprecatedProperty(properties, "$5", GetGroup5Adapter, PropertyAttributes.Enumerable);
            this.AddDeprecatedProperty(properties, "$6", GetGroup6Adapter, PropertyAttributes.Enumerable);
            this.AddDeprecatedProperty(properties, "$7", GetGroup7Adapter, PropertyAttributes.Enumerable);
            this.AddDeprecatedProperty(properties, "$8", GetGroup8Adapter, PropertyAttributes.Enumerable);
            this.AddDeprecatedProperty(properties, "$9", GetGroup9Adapter, PropertyAttributes.Enumerable);
            this.AddDeprecatedProperty(properties, "input", GetInputAdapter, PropertyAttributes.Enumerable);
            this.AddDeprecatedProperty(properties, "$_", GetInputAdapter, PropertyAttributes.Sealed);
            this.AddDeprecatedProperty(properties, "lastMatch", GetLastMatchAdapter, PropertyAttributes.Enumerable);
            this.AddDeprecatedProperty(properties, "$&", GetLastMatchAdapter, PropertyAttributes.Sealed);
            this.AddDeprecatedProperty(properties, "lastParen", GetLastParenAdapter, PropertyAttributes.Enumerable);
            this.AddDeprecatedProperty(properties, "$+", GetLastParenAdapter, PropertyAttributes.Sealed);
            this.AddDeprecatedProperty(properties, "leftContext", GetLeftContextAdapter, PropertyAttributes.Enumerable);
            this.AddDeprecatedProperty(properties, "$`", GetLeftContextAdapter, PropertyAttributes.Sealed);
            this.AddDeprecatedProperty(properties, "rightContext", GetRightContextAdapter, PropertyAttributes.Enumerable);
            this.AddDeprecatedProperty(properties, "$'", GetRightContextAdapter, PropertyAttributes.Sealed);
        }

        /// <summary>
        /// Initializes a single deprecated property.
        /// </summary>
        /// <param name="properties"> The list to add to. </param>
        /// <param name="propertyName"> The name of the property. </param>
        /// <param name="getter"> The property getter. </param>
        /// <param name="attributes"> The property attributes (determines whether the property is enumerable). </param>
        private void AddDeprecatedProperty(List<PropertyNameAndValue> properties, string propertyName, Func<ScriptEngine, object, object[], object> getter, PropertyAttributes attributes)
        {
            var getterFunction = new ClrStubFunction(this.Engine.Function.InstancePrototype, getter);
            properties.Add(new PropertyNameAndValue(propertyName, new PropertyDescriptor(new PropertyAccessorValue(getterFunction, null), attributes)));
        }

        /// <summary>
        /// Sets the deprecated RegExp properties.
        /// </summary>
        /// <param name="input"> The string against which a regular expression is matched. </param>
        /// <param name="match"> The regular expression match to base the properties on. </param>
        internal void SetDeprecatedProperties(string input, System.Text.RegularExpressions.Match match)
        {
            this.lastInput = input;
            this.lastMatch = match;
        }

        /// <summary>
        /// Gets the value of RegExp.input and RegExp.$_.
        /// </summary>
        /// <returns> The value of RegExp.input and RegExp.$_. </returns>
        public string GetInput()
        {
            if (this.lastMatch == null)
                return string.Empty;
            return this.lastInput;
        }

        /// <summary>
        /// Adapter for GetInput().
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="thisObj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private object GetInputAdapter(ScriptEngine engine, object thisObj, object[] args)
        {
            return GetInput();
        }

        /// <summary>
        /// Gets the value of RegExp.$1.
        /// </summary>
        /// <returns> The value of RegExp.$1. </returns>
        public string GetGroup1()
        {
            if (this.lastMatch == null || this.lastMatch.Groups.Count < 1)
                return string.Empty;
            return this.lastMatch.Groups[1].Value;
        }

        /// <summary>
        /// Adapter for GetGroup1().
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="thisObj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private object GetGroup1Adapter(ScriptEngine engine, object thisObj, object[] args)
        {
            return GetGroup1();
        }

        /// <summary>
        /// Gets the value of RegExp.$2.
        /// </summary>
        /// <returns> The value of RegExp.$2. </returns>
        public string GetGroup2()
        {
            if (this.lastMatch == null || this.lastMatch.Groups.Count < 2)
                return string.Empty;
            return this.lastMatch.Groups[2].Value;
        }

        /// <summary>
        /// Adapter for GetGroup2().
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="thisObj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private object GetGroup2Adapter(ScriptEngine engine, object thisObj, object[] args)
        {
            return GetGroup2();
        }

        /// <summary>
        /// Gets the value of RegExp.$3.
        /// </summary>
        /// <returns> The value of RegExp.$3. </returns>
        public string GetGroup3()
        {
            if (this.lastMatch == null || this.lastMatch.Groups.Count < 3)
                return string.Empty;
            return this.lastMatch.Groups[3].Value;
        }

        /// <summary>
        /// Adapter for GetGroup3().
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="thisObj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private object GetGroup3Adapter(ScriptEngine engine, object thisObj, object[] args)
        {
            return GetGroup3();
        }

        /// <summary>
        /// Gets the value of RegExp.$4.
        /// </summary>
        /// <returns> The value of RegExp.$4. </returns>
        public string GetGroup4()
        {
            if (this.lastMatch == null || this.lastMatch.Groups.Count < 4)
                return string.Empty;
            return this.lastMatch.Groups[4].Value;
        }

        /// <summary>
        /// Adapter for GetGroup4().
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="thisObj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private object GetGroup4Adapter(ScriptEngine engine, object thisObj, object[] args)
        {
            return GetGroup4();
        }

        /// <summary>
        /// Gets the value of RegExp.$5.
        /// </summary>
        /// <returns> The value of RegExp.$5. </returns>
        public string GetGroup5()
        {
            if (this.lastMatch == null || this.lastMatch.Groups.Count < 5)
                return string.Empty;
            return this.lastMatch.Groups[5].Value;
        }

        /// <summary>
        /// Adapter for GetGroup5().
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="thisObj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private object GetGroup5Adapter(ScriptEngine engine, object thisObj, object[] args)
        {
            return GetGroup5();
        }

        /// <summary>
        /// Gets the value of RegExp.$6.
        /// </summary>
        /// <returns> The value of RegExp.$6. </returns>
        public string GetGroup6()
        {
            if (this.lastMatch == null || this.lastMatch.Groups.Count < 6)
                return string.Empty;
            return this.lastMatch.Groups[6].Value;
        }

        /// <summary>
        /// Adapter for GetGroup6().
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="thisObj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private object GetGroup6Adapter(ScriptEngine engine, object thisObj, object[] args)
        {
            return GetGroup6();
        }

        /// <summary>
        /// Gets the value of RegExp.$7.
        /// </summary>
        /// <returns> The value of RegExp.$7. </returns>
        public string GetGroup7()
        {
            if (this.lastMatch == null || this.lastMatch.Groups.Count < 7)
                return string.Empty;
            return this.lastMatch.Groups[7].Value;
        }

        /// <summary>
        /// Adapter for GetGroup7().
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="thisObj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private object GetGroup7Adapter(ScriptEngine engine, object thisObj, object[] args)
        {
            return GetGroup7();
        }

        /// <summary>
        /// Gets the value of RegExp.$8.
        /// </summary>
        /// <returns> The value of RegExp.$8. </returns>
        public string GetGroup8()
        {
            if (this.lastMatch == null || this.lastMatch.Groups.Count < 8)
                return string.Empty;
            return this.lastMatch.Groups[8].Value;
        }

        /// <summary>
        /// Adapter for GetGroup8().
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="thisObj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private object GetGroup8Adapter(ScriptEngine engine, object thisObj, object[] args)
        {
            return GetGroup8();
        }

        /// <summary>
        /// Gets the value of RegExp.$9.
        /// </summary>
        /// <returns> The value of RegExp.$9. </returns>
        public string GetGroup9()
        {
            if (this.lastMatch == null || this.lastMatch.Groups.Count < 9)
                return string.Empty;
            return this.lastMatch.Groups[9].Value;
        }

        /// <summary>
        /// Adapter for GetGroup9().
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="thisObj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private object GetGroup9Adapter(ScriptEngine engine, object thisObj, object[] args)
        {
            return GetGroup9();
        }

        /// <summary>
        /// Gets the value of RegExp.lastMatch and RegExp.$&amp;.
        /// </summary>
        /// <returns> The value of RegExp.lastMatch and RegExp.$&amp;. </returns>
        public string GetLastMatch()
        {
            if (this.lastMatch == null)
                return string.Empty;
            return this.lastMatch.Value;
        }

        /// <summary>
        /// Adapter for GetLastMatch().
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="thisObj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private object GetLastMatchAdapter(ScriptEngine engine, object thisObj, object[] args)
        {
            return GetLastMatch();
        }

        /// <summary>
        /// Gets the value of RegExp.lastParen and RegExp.$+.
        /// </summary>
        /// <returns> The value of RegExp.lastParen and RegExp.$+. </returns>
        public string GetLastParen()
        {
            if (this.lastMatch == null)
                return string.Empty;
            return this.lastMatch.Groups.Count > 1 ?
                this.lastMatch.Groups[this.lastMatch.Groups.Count - 1].Value :
                string.Empty;
        }

        /// <summary>
        /// Adapter for GetLastParen().
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="thisObj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private object GetLastParenAdapter(ScriptEngine engine, object thisObj, object[] args)
        {
            return GetLastParen();
        }

        /// <summary>
        /// Gets the value of RegExp.leftContext and RegExp.$`.
        /// </summary>
        /// <returns> The value of RegExp.leftContext and RegExp.$`. </returns>
        public string GetLeftContext()
        {
            if (this.lastMatch == null)
                return string.Empty;
            return this.lastInput.Substring(0, this.lastMatch.Index);
        }

        /// <summary>
        /// Adapter for GetLeftContext().
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="thisObj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private object GetLeftContextAdapter(ScriptEngine engine, object thisObj, object[] args)
        {
            return GetLeftContext();
        }

        /// <summary>
        /// Gets the value of RegExp.rightContext and RegExp.$'.
        /// </summary>
        /// <returns> The value of RegExp.rightContext and RegExp.$'. </returns>
        public string GetRightContext()
        {
            if (this.lastMatch == null)
                return string.Empty;
            return this.lastInput.Substring(this.lastMatch.Index + this.lastMatch.Length);
        }

        /// <summary>
        /// Adapter for GetRightContext().
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="thisObj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private object GetRightContextAdapter(ScriptEngine engine, object thisObj, object[] args)
        {
            return GetRightContext();
        }




        //     JAVASCRIPT INTERNAL FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Called when the RegExp object is invoked like a function e.g. RegExp('abc', 'g') or
        /// RegExp(/abc/).  If a string is passed as the first parameter it creates a new regular
        /// expression instance.  Otherwise, if a regular expression is passed it returns the given
        /// regular expression verbatim.
        /// </summary>
        /// <param name="patternOrRegExp"> A regular expression pattern or a regular expression. </param>
        /// <param name="flags"> Available flags, which may be combined, are:
        /// g (global search for all occurrences of pattern)
        /// i (ignore case)
        /// m (multiline search)</param>
        [JSCallFunction]
        public RegExpInstance Call(object patternOrRegExp, string flags = null)
        {
            if (patternOrRegExp is RegExpInstance)
            {
                // RegExp(/abc/) or RegExp(/abc/, "g")
                if (flags != null)
                    return new RegExpInstance(this.InstancePrototype, ((RegExpInstance)patternOrRegExp).Source, flags);
                return (RegExpInstance)patternOrRegExp;
            }
            else
            {
                // RegExp('abc', 'g')
                var pattern = string.Empty;
                if (TypeUtilities.IsUndefined(patternOrRegExp) == false)
                    pattern = TypeConverter.ToString(patternOrRegExp);
                return new RegExpInstance(this.InstancePrototype, pattern, flags);
            }
        }

        /// <summary>
        /// Called when the new keyword is used on the RegExp object e.g. new RegExp(/abc/).
        /// Creates a new regular expression instance.
        /// </summary>
        /// <param name="patternOrRegExp"> The regular expression pattern, or a regular expression
        /// to clone. </param>
        /// <param name="flags"> Available flags, which may be combined, are:
        /// g (global search for all occurrences of pattern)
        /// i (ignore case)
        /// m (multiline search)</param>
        [JSConstructorFunction]
        public RegExpInstance Construct(object patternOrRegExp, string flags = null)
        {
            if (patternOrRegExp is RegExpInstance)
            {
                // new RegExp(regExp, flags)
                if (flags != null)
                    return new RegExpInstance(this.InstancePrototype, ((RegExpInstance)patternOrRegExp).Source, flags);
                return new RegExpInstance(this.InstancePrototype, (RegExpInstance)patternOrRegExp);
            }
            else
            {
                // new RegExp(pattern, flags)
                var pattern = string.Empty;
                if (TypeUtilities.IsUndefined(patternOrRegExp) == false)
                    pattern = TypeConverter.ToString(patternOrRegExp);
                return new RegExpInstance(this.InstancePrototype, pattern, flags);
            }
        }
    }
}
