/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class PromiseInstance
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(7)
			{
				new PropertyNameAndValue("finally", new ClrStubFunction(engine, "finally", 1, __STUB__Finally), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("catch", new ClrStubFunction(engine, "catch", 1, __STUB__Catch), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("then", new ClrStubFunction(engine, "then", 2, __STUB__Then), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Finally(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is PromiseInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'finally' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((PromiseInstance)thisObj).Finally(Undefined.Value);
				default:
					return ((PromiseInstance)thisObj).Finally(args[0]);
			}
		}

		private static object __STUB__Catch(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is PromiseInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'catch' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((PromiseInstance)thisObj).Catch(Undefined.Value);
				default:
					return ((PromiseInstance)thisObj).Catch(args[0]);
			}
		}

		private static object __STUB__Then(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is PromiseInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'then' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((PromiseInstance)thisObj).Then(Undefined.Value, Undefined.Value);
				case 1:
					return ((PromiseInstance)thisObj).Then(args[0], Undefined.Value);
				default:
					return ((PromiseInstance)thisObj).Then(args[0], args[1]);
			}
		}
	}

}
