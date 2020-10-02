/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class ArrayBufferConstructor
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(5)
			{
				new PropertyNameAndValue(Symbol.Species, new PropertyDescriptor(new ClrStubFunction(engine, "get [Symbol.species]", 0, __GETTER__Species), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("isView", new ClrStubFunction(engine, "isView", 1, __STUB__IsView), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ArrayBufferConstructor))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'Call' is not generic.");
			return ((ArrayBufferConstructor)thisObj).Call();
		}

		private static ObjectInstance __STUB__Construct(ScriptEngine engine, FunctionInstance thisObj, FunctionInstance newTarget, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return ((ArrayBufferConstructor)thisObj).Construct(0);
				default:
					return ((ArrayBufferConstructor)thisObj).Construct(TypeConverter.ToInteger(args[0]));
			}
		}

		private static object __GETTER__Species(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ArrayBufferConstructor))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'get [Symbol.species]' is not generic.");
			return ((ArrayBufferConstructor)thisObj).Species;
		}

		private static object __STUB__IsView(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return IsView(Undefined.Value);
				default:
					return IsView(args[0]);
			}
		}
	}

}
