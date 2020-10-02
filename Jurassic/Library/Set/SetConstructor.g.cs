/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class SetConstructor
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(4)
			{
				new PropertyNameAndValue(Symbol.Species, new PropertyDescriptor(new ClrStubFunction(engine, "get [Symbol.species]", 0, __GETTER__Species), null, PropertyAttributes.Configurable)),
			};
		}

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is SetConstructor))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'Call' is not generic.");
			return ((SetConstructor)thisObj).Call();
		}

		private static ObjectInstance __STUB__Construct(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is SetConstructor))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'Construct' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((SetConstructor)thisObj).Construct(Undefined.Value);
				default:
					return ((SetConstructor)thisObj).Construct(args[0]);
			}
		}

		private static object __GETTER__Species(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is SetConstructor))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'get [Symbol.species]' is not generic.");
			return ((SetConstructor)thisObj).Species;
		}
	}

}
