/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class PromiseConstructor
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(8)
			{
				new PropertyNameAndValue(engine.Symbol.Species, new PropertyDescriptor(new ClrStubFunction(engine.FunctionInstancePrototype, "get [Symbol.species]", 0, __GETTER__Species), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("reject", new ClrStubFunction(engine.FunctionInstancePrototype, "reject", 1, __STUB__Reject), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("resolve", new ClrStubFunction(engine.FunctionInstancePrototype, "resolve", 1, __STUB__Resolve), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("race", new ClrStubFunction(engine.FunctionInstancePrototype, "race", 1, __STUB__Race), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("all", new ClrStubFunction(engine.FunctionInstancePrototype, "all", 1, __STUB__All), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is PromiseConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'Call' is not generic.");
			return ((PromiseConstructor)thisObj).Call();
		}

		private static ObjectInstance __STUB__Construct(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is PromiseConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'Construct' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return ((PromiseConstructor)thisObj).Construct(TypeConverter.ToObject<FunctionInstance>(engine, args[0]));
			}
		}

		private static object __GETTER__Species(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is PromiseConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get [Symbol.species]' is not generic.");
			return ((PromiseConstructor)thisObj).Species;
		}

		private static object __STUB__Reject(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is PromiseConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'reject' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((PromiseConstructor)thisObj).Reject(Undefined.Value);
				default:
					return ((PromiseConstructor)thisObj).Reject(args[0]);
			}
		}

		private static object __STUB__Resolve(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is PromiseConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'resolve' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((PromiseConstructor)thisObj).Resolve(Undefined.Value);
				default:
					return ((PromiseConstructor)thisObj).Resolve(args[0]);
			}
		}

		private static object __STUB__Race(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is PromiseConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'race' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return ((PromiseConstructor)thisObj).Race(TypeConverter.ToObject(engine, args[0]));
			}
		}

		private static object __STUB__All(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is PromiseConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'all' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return ((PromiseConstructor)thisObj).All(TypeConverter.ToObject(engine, args[0]));
			}
		}
	}

}
