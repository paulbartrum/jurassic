/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class SetInstance
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(12)
			{
				new PropertyNameAndValue("size", new PropertyDescriptor(new ClrStubFunction(engine, "get size", 0, __GETTER__Size), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("add", new ClrStubFunction(engine, "add", 1, __STUB__Add), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("clear", new ClrStubFunction(engine, "clear", 0, __STUB__Clear), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("delete", new ClrStubFunction(engine, "delete", 1, __STUB__Delete), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("entries", new ClrStubFunction(engine, "entries", 0, __STUB__Entries), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("forEach", new ClrStubFunction(engine, "forEach", 1, __STUB__ForEach), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("has", new ClrStubFunction(engine, "has", 1, __STUB__Has), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("keys", new ClrStubFunction(engine, "keys", 0, __STUB__Keys), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("values", new ClrStubFunction(engine, "values", 0, __STUB__Values), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __GETTER__Size(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is SetInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'get size' is not generic.");
			return ((SetInstance)thisObj).Size;
		}

		private static object __STUB__Add(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is SetInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'add' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((SetInstance)thisObj).Add(Undefined.Value);
				default:
					return ((SetInstance)thisObj).Add(args[0]);
			}
		}

		private static object __STUB__Clear(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is SetInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'clear' is not generic.");
			((SetInstance)thisObj).Clear(); return Undefined.Value;
		}

		private static object __STUB__Delete(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is SetInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'delete' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((SetInstance)thisObj).Delete(Undefined.Value);
				default:
					return ((SetInstance)thisObj).Delete(args[0]);
			}
		}

		private static object __STUB__Entries(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is SetInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'entries' is not generic.");
			return ((SetInstance)thisObj).Entries();
		}

		private static object __STUB__ForEach(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is SetInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'forEach' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					((SetInstance)thisObj).ForEach(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), Undefined.Value); return Undefined.Value;
				default:
					((SetInstance)thisObj).ForEach(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), args[1]); return Undefined.Value;
			}
		}

		private static object __STUB__Has(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is SetInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'has' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((SetInstance)thisObj).Has(Undefined.Value);
				default:
					return ((SetInstance)thisObj).Has(args[0]);
			}
		}

		private static object __STUB__Keys(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is SetInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'keys' is not generic.");
			return ((SetInstance)thisObj).Keys();
		}

		private static object __STUB__Values(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is SetInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'values' is not generic.");
			return ((SetInstance)thisObj).Values();
		}
	}

}
