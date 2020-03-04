/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;
using Jurassic.Library;

namespace Jurassic.Extensions.Fetch
{

	public partial class HeadersConstructor
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(4)
			{
				new PropertyNameAndValue(engine.Symbol.Species, new PropertyDescriptor(new ClrStubFunction(engine, "get [Symbol.species]", 0, __GETTER__Species), null, PropertyAttributes.Configurable)),
			};
		}

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is HeadersConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'Call' is not generic.");
			return ((HeadersConstructor)thisObj).Call();
		}

		private static ObjectInstance __STUB__Construct(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is HeadersConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'Construct' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((HeadersConstructor)thisObj).Construct(null);
				default:
					return ((HeadersConstructor)thisObj).Construct(TypeUtilities.IsUndefined(args[0]) ? null : args[0]);
			}
		}

		private static object __GETTER__Species(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is HeadersConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get [Symbol.species]' is not generic.");
			return ((HeadersConstructor)thisObj).Species;
		}
	}

}
