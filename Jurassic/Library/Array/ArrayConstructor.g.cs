/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class ArrayConstructor
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(8)
			{
				new PropertyNameAndValue(engine.Symbol.Species, new PropertyDescriptor(new ClrStubFunction(engine, "get [Symbol.species]", 0, __GETTER__Species), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("isArray", new ClrStubFunction(engine, "isArray", 1, __STUB__IsArray), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("of", new ClrStubFunction(engine, "of", 0, __STUB__Of), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("from", new ClrStubFunction(engine, "from", 1, __STUB__From), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ArrayConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'Call' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((ArrayConstructor)thisObj).Call(new object[0]);
				default:
					return ((ArrayConstructor)thisObj).Call(args);
			}
		}

		private static ObjectInstance __STUB__Construct(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ArrayConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'Construct' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((ArrayConstructor)thisObj).Construct(new object[0]);
				default:
					return ((ArrayConstructor)thisObj).Construct(args);
			}
		}

		private static object __GETTER__Species(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ArrayConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get [Symbol.species]' is not generic.");
			return ((ArrayConstructor)thisObj).Species;
		}

		private static object __STUB__IsArray(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return IsArray(Undefined.Value);
				default:
					return IsArray(args[0]);
			}
		}

		private static object __STUB__Of(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Of(engine, new object[0]);
				default:
					return Of(engine, args);
			}
		}

		private static object __STUB__From(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "Required argument 'iterable' was not specified.");
				case 1:
					return From(engine, TypeConverter.ToObject(engine, args[0]));
				case 2:
					return From(engine, TypeConverter.ToObject(engine, args[0]), TypeConverter.ToObject<FunctionInstance>(engine, args[1]), Undefined.Value);
				default:
					return From(engine, TypeConverter.ToObject(engine, args[0]), TypeConverter.ToObject<FunctionInstance>(engine, args[1]), args[2]);
			}
		}
	}

}
