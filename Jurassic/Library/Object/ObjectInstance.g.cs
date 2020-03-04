/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class ObjectInstance
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(10)
			{
				new PropertyNameAndValue("hasOwnProperty", new ClrStubFunction(engine, "hasOwnProperty", 1, __STUB__HasOwnProperty), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("isPrototypeOf", new ClrStubFunction(engine, "isPrototypeOf", 1, __STUB__IsPrototypeOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("propertyIsEnumerable", new ClrStubFunction(engine, "propertyIsEnumerable", 1, __STUB__PropertyIsEnumerable), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toLocaleString", new ClrStubFunction(engine, "toLocaleString", 0, __STUB__ToLocaleString), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("valueOf", new ClrStubFunction(engine, "valueOf", 0, __STUB__ValueOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toString", new ClrStubFunction(engine, "toString", 0, __STUB__ToStringJS), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__HasOwnProperty(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return HasOwnProperty(engine, thisObj, Undefined.Value);
				default:
					return HasOwnProperty(engine, thisObj, args[0]);
			}
		}

		private static object __STUB__IsPrototypeOf(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return IsPrototypeOf(engine, thisObj, Undefined.Value);
				default:
					return IsPrototypeOf(engine, thisObj, args[0]);
			}
		}

		private static object __STUB__PropertyIsEnumerable(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return PropertyIsEnumerable(engine, thisObj, Undefined.Value);
				default:
					return PropertyIsEnumerable(engine, thisObj, args[0]);
			}
		}

		private static object __STUB__ToLocaleString(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ObjectInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'toLocaleString' is not generic.");
			return ((ObjectInstance)thisObj).ToLocaleString();
		}

		private static object __STUB__ValueOf(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ObjectInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'valueOf' is not generic.");
			return ((ObjectInstance)thisObj).ValueOf();
		}

		private static object __STUB__ToStringJS(ScriptEngine engine, object thisObj, object[] args)
		{
			return ToStringJS(engine, thisObj);
		}
	}

}
