/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class NumberInstance
	{
		private List<PropertyNameAndValue> GetDeclarativeProperties()
		{
			return new List<PropertyNameAndValue>(11)
			{
				new PropertyNameAndValue("toExponential", new ClrStubFunction(Engine.FunctionInstancePrototype, "toExponential", 1, __STUB__toExponential), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toFixed", new ClrStubFunction(Engine.FunctionInstancePrototype, "toFixed", 1, __STUB__toFixed), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toLocaleString", new ClrStubFunction(Engine.FunctionInstancePrototype, "toLocaleString", 0, __STUB__toLocaleString), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toPrecision", new ClrStubFunction(Engine.FunctionInstancePrototype, "toPrecision", 1, __STUB__toPrecision), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toString", new ClrStubFunction(Engine.FunctionInstancePrototype, "toString", 1, __STUB__toString), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("valueOf", new ClrStubFunction(Engine.FunctionInstancePrototype, "valueOf", 0, __STUB__valueOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("clz", new ClrStubFunction(Engine.FunctionInstancePrototype, "clz", 0, __STUB__clz), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__toExponential(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is NumberInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'toExponential' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((NumberInstance)thisObj).ToExponential(Undefined.Value);
				default:
					return ((NumberInstance)thisObj).ToExponential(args[0]);
			}
		}

		private static object __STUB__toFixed(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is NumberInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'toFixed' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((NumberInstance)thisObj).ToFixed(0);
				default:
					return ((NumberInstance)thisObj).ToFixed(TypeUtilities.IsUndefined(args[0]) ? 0 : TypeConverter.ToInteger(args[0]));
			}
		}

		private static object __STUB__toLocaleString(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is NumberInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'toLocaleString' is not generic.");
			return ((NumberInstance)thisObj).ToLocaleString();
		}

		private static object __STUB__toPrecision(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is NumberInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'toPrecision' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((NumberInstance)thisObj).ToPrecision(Undefined.Value);
				default:
					return ((NumberInstance)thisObj).ToPrecision(args[0]);
			}
		}

		private static object __STUB__toString(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is NumberInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'toString' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((NumberInstance)thisObj).ToStringJS(10);
				default:
					return ((NumberInstance)thisObj).ToStringJS(TypeUtilities.IsUndefined(args[0]) ? 10 : TypeConverter.ToInteger(args[0]));
			}
		}

		private static object __STUB__valueOf(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is NumberInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'valueOf' is not generic.");
			return ((NumberInstance)thisObj).ValueOf();
		}

		private static object __STUB__clz(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is NumberInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'clz' is not generic.");
			return ((NumberInstance)thisObj).Clz();
		}
	}

}
