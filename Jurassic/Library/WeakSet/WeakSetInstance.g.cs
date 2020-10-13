/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class WeakSetInstance
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(7)
			{
				new PropertyNameAndValue("add", new ClrStubFunction(engine, "add", 1, __STUB__Add), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("delete", new ClrStubFunction(engine, "delete", 1, __STUB__Delete), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("has", new ClrStubFunction(engine, "has", 1, __STUB__Has), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Add(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is WeakSetInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'add' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((WeakSetInstance)thisObj).Add(Undefined.Value);
				default:
					return ((WeakSetInstance)thisObj).Add(args[0]);
			}
		}

		private static object __STUB__Delete(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is WeakSetInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'delete' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((WeakSetInstance)thisObj).Delete(Undefined.Value);
				default:
					return ((WeakSetInstance)thisObj).Delete(args[0]);
			}
		}

		private static object __STUB__Has(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is WeakSetInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'has' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((WeakSetInstance)thisObj).Has(Undefined.Value);
				default:
					return ((WeakSetInstance)thisObj).Has(args[0]);
			}
		}
	}

}
