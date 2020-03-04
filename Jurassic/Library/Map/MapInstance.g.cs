/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class MapInstance
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(13)
			{
				new PropertyNameAndValue("size", new PropertyDescriptor(new ClrStubFunction(engine, "get size", 0, __GETTER__Size), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("clear", new ClrStubFunction(engine, "clear", 0, __STUB__Clear), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("delete", new ClrStubFunction(engine, "delete", 1, __STUB__Delete), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("entries", new ClrStubFunction(engine, "entries", 0, __STUB__Entries), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("forEach", new ClrStubFunction(engine, "forEach", 1, __STUB__ForEach), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("get", new ClrStubFunction(engine, "get", 1, __STUB__Get), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("has", new ClrStubFunction(engine, "has", 1, __STUB__Has), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("keys", new ClrStubFunction(engine, "keys", 0, __STUB__Keys), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("set", new ClrStubFunction(engine, "set", 2, __STUB__Set), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("values", new ClrStubFunction(engine, "values", 0, __STUB__Values), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __GETTER__Size(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is MapInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get size' is not generic.");
			return ((MapInstance)thisObj).Size;
		}

		private static object __STUB__Clear(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is MapInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'clear' is not generic.");
			((MapInstance)thisObj).Clear(); return Undefined.Value;
		}

		private static object __STUB__Delete(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is MapInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'delete' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((MapInstance)thisObj).Delete(Undefined.Value);
				default:
					return ((MapInstance)thisObj).Delete(args[0]);
			}
		}

		private static object __STUB__Entries(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is MapInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'entries' is not generic.");
			return ((MapInstance)thisObj).Entries();
		}

		private static object __STUB__ForEach(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is MapInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'forEach' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					((MapInstance)thisObj).ForEach(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), Undefined.Value); return Undefined.Value;
				default:
					((MapInstance)thisObj).ForEach(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), args[1]); return Undefined.Value;
			}
		}

		private static object __STUB__Get(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is MapInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((MapInstance)thisObj).Get(Undefined.Value);
				default:
					return ((MapInstance)thisObj).Get(args[0]);
			}
		}

		private static object __STUB__Has(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is MapInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'has' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((MapInstance)thisObj).Has(Undefined.Value);
				default:
					return ((MapInstance)thisObj).Has(args[0]);
			}
		}

		private static object __STUB__Keys(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is MapInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'keys' is not generic.");
			return ((MapInstance)thisObj).Keys();
		}

		private static object __STUB__Set(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is MapInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'set' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((MapInstance)thisObj).Set(Undefined.Value, Undefined.Value);
				case 1:
					return ((MapInstance)thisObj).Set(args[0], Undefined.Value);
				default:
					return ((MapInstance)thisObj).Set(args[0], args[1]);
			}
		}

		private static object __STUB__Values(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is MapInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'values' is not generic.");
			return ((MapInstance)thisObj).Values();
		}
	}

}
