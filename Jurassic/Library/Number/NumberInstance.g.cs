/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class NumberInstance
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(10)
			{
				new PropertyNameAndValue("toExponential", new ClrStubFunction(engine, "toExponential", 1, __STUB__ToExponential), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toFixed", new ClrStubFunction(engine, "toFixed", 1, __STUB__ToFixed), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toLocaleString", new ClrStubFunction(engine, "toLocaleString", 0, __STUB__ToLocaleString), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toPrecision", new ClrStubFunction(engine, "toPrecision", 1, __STUB__ToPrecision), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toString", new ClrStubFunction(engine, "toString", 1, __STUB__ToStringJS), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("valueOf", new ClrStubFunction(engine, "valueOf", 0, __STUB__ValueOf), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__ToExponential(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is NumberInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'toExponential' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((NumberInstance)thisObj).ToExponential(Undefined.Value);
				default:
					return ((NumberInstance)thisObj).ToExponential(args[0]);
			}
		}

		private static object __STUB__ToFixed(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is NumberInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'toFixed' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((NumberInstance)thisObj).ToFixed(0);
				default:
					return ((NumberInstance)thisObj).ToFixed(TypeUtilities.IsUndefined(args[0]) ? 0 : TypeConverter.ToInteger(args[0]));
			}
		}

		private static object __STUB__ToLocaleString(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is NumberInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'toLocaleString' is not generic.");
			return ((NumberInstance)thisObj).ToLocaleString();
		}

		private static object __STUB__ToPrecision(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is NumberInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'toPrecision' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((NumberInstance)thisObj).ToPrecision(Undefined.Value);
				default:
					return ((NumberInstance)thisObj).ToPrecision(args[0]);
			}
		}

		private static object __STUB__ToStringJS(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is NumberInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'toString' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((NumberInstance)thisObj).ToStringJS(10);
				default:
					return ((NumberInstance)thisObj).ToStringJS(TypeUtilities.IsUndefined(args[0]) ? 10 : TypeConverter.ToInteger(args[0]));
			}
		}

		private static object __STUB__ValueOf(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is NumberInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'valueOf' is not generic.");
			return ((NumberInstance)thisObj).ValueOf();
		}
	}

}
