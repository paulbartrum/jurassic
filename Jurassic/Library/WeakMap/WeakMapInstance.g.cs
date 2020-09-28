/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class WeakMapInstance
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(8)
			{
				new PropertyNameAndValue("delete", new ClrStubFunction(engine, "delete", 1, __STUB__Delete), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("get", new ClrStubFunction(engine, "get", 1, __STUB__Get), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("has", new ClrStubFunction(engine, "has", 1, __STUB__Has), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("set", new ClrStubFunction(engine, "set", 2, __STUB__Set), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Delete(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is WeakMapInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'delete' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((WeakMapInstance)thisObj).Delete(Undefined.Value);
				default:
					return ((WeakMapInstance)thisObj).Delete(args[0]);
			}
		}

		private static object __STUB__Get(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is WeakMapInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'get' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((WeakMapInstance)thisObj).Get(Undefined.Value);
				default:
					return ((WeakMapInstance)thisObj).Get(args[0]);
			}
		}

		private static object __STUB__Has(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is WeakMapInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'has' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((WeakMapInstance)thisObj).Has(Undefined.Value);
				default:
					return ((WeakMapInstance)thisObj).Has(args[0]);
			}
		}

		private static object __STUB__Set(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is WeakMapInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'set' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((WeakMapInstance)thisObj).Set(Undefined.Value, Undefined.Value);
				case 1:
					return ((WeakMapInstance)thisObj).Set(args[0], Undefined.Value);
				default:
					return ((WeakMapInstance)thisObj).Set(args[0], args[1]);
			}
		}
	}

}
