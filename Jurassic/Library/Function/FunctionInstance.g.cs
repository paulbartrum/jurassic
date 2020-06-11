/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public abstract partial class FunctionInstance
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(8)
			{
				new PropertyNameAndValue("apply", new ClrStubFunction(engine, "apply", 2, __STUB__Apply), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("call", new ClrStubFunction(engine, "call", 1, __STUB__Call), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("bind", new ClrStubFunction(engine, "bind", 1, __STUB__Bind), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toString", new ClrStubFunction(engine, "toString", 0, __STUB__ToStringJS), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Apply(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is FunctionInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'apply' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((FunctionInstance)thisObj).Apply(Undefined.Value, Undefined.Value);
				case 1:
					return ((FunctionInstance)thisObj).Apply(args[0], Undefined.Value);
				default:
					return ((FunctionInstance)thisObj).Apply(args[0], args[1]);
			}
		}

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is FunctionInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'call' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((FunctionInstance)thisObj).Call(Undefined.Value, new object[0]);
				case 1:
					return ((FunctionInstance)thisObj).Call(args[0], new object[0]);
				default:
					return ((FunctionInstance)thisObj).Call(args[0], TypeUtilities.SliceArray(args, 1));
			}
		}

		private static object __STUB__Bind(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is FunctionInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'bind' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((FunctionInstance)thisObj).Bind(Undefined.Value, new object[0]);
				case 1:
					return ((FunctionInstance)thisObj).Bind(args[0], new object[0]);
				default:
					return ((FunctionInstance)thisObj).Bind(args[0], TypeUtilities.SliceArray(args, 1));
			}
		}

		private static object __STUB__ToStringJS(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is FunctionInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'toString' is not generic.");
			return ((FunctionInstance)thisObj).ToStringJS();
		}
	}

}
