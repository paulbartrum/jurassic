﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents the built-in JSON object.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Type = "{DebuggerDisplayType,nq}")]
    [DebuggerTypeProxy(typeof(ObjectInstanceDebugView))]
    public partial class JSONObject : ObjectInstance
    {

        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new JSON object.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        internal JSONObject(ObjectInstance prototype)
            : base(prototype)
        {
            var properties = GetDeclarativeProperties(Engine);
            properties.Add(new PropertyNameAndValue(Symbol.ToStringTag, "JSON", PropertyAttributes.Configurable));
            InitializeProperties(properties);
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Parses the JSON source text and transforms it into a value.
        /// </summary>
        /// <param name="engine"> The current script environment. </param>
        /// <param name="text"> The JSON text to parse. </param>
        /// <param name="reviver"> A function that will be called for each value. </param>
        /// <returns> The value of the JSON text. </returns>
        [JSInternalFunction(Name = "parse", Flags = JSFunctionFlags.HasEngineParameter)]
        public static object Parse(ScriptEngine engine, string text, object reviver = null)
        {
            var parser = new JSONParser(engine, new JSONLexer(engine, new System.IO.StringReader(text)));
            parser.ReviverFunction = reviver as FunctionInstance;
            return parser.Parse();
        }

        /// <summary>
        /// Serializes a value into a JSON string.
        /// </summary>
        /// <param name="engine"> The current script environment. </param>
        /// <param name="value"> The value to serialize. </param>
        /// <param name="replacer"> Either a function that can transform each value before it is
        /// serialized, or an array of the names of the properties to serialize. </param>
        /// <param name="spacer"> Either the number of spaces to use for indentation, or a string
        /// that is used for indentation. </param>
        /// <returns> The JSON string representing the value. </returns>
        [JSInternalFunction(Name = "stringify", Flags = JSFunctionFlags.HasEngineParameter)]
        public static object Stringify(ScriptEngine engine, object value, object replacer = null, object spacer = null)
        {
            var serializer = new JSONSerializer(engine);

            // The replacer object can be either a function or an array.
            serializer.ReplacerFunction = replacer as FunctionInstance;
            if (replacer is ArrayInstance)
            {
                var replacerArray = (ArrayInstance)replacer;
                var serializableProperties = new HashSet<string>(StringComparer.Ordinal);
                foreach (object elementValue in replacerArray.ElementValues)
                {
                    if (elementValue is string || elementValue is int || elementValue is double || elementValue is StringInstance || elementValue is NumberInstance)
                        serializableProperties.Add(TypeConverter.ToString(elementValue));
                }
                serializer.SerializableProperties = serializableProperties;
            }

            // The spacer argument can be the number of spaces or a string.
            if (spacer is NumberInstance)
                spacer = ((NumberInstance)spacer).Value;
            else if (spacer is StringInstance)
                spacer = ((StringInstance)spacer).Value;
            if (spacer is double)
                serializer.Indentation = new string(' ', Math.Max(Math.Min(TypeConverter.ToInteger((double)spacer), 10), 0));
            else if (spacer is int)
                serializer.Indentation = new string(' ', Math.Max(Math.Min(TypeConverter.ToInteger((int)spacer), 10), 0));
            else if (spacer is string)
                serializer.Indentation = ((string)spacer).Substring(0, Math.Min(((string)spacer).Length, 10));

            // Serialize the value.
            return serializer.Serialize(value) ?? (object)Undefined.Value;
        }


        /// <summary>
        /// Gets type, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayType
        {
            get { return "JSON Object"; }
        }
    }
}
