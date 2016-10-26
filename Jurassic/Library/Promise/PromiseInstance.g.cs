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
			return new List<PropertyNameAndValue>(6)
			{
				new PropertyNameAndValue("catch", new ClrStubFunction(engine.FunctionInstancePrototype, "catch", 1, __STUB__Catch), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("then", new ClrStubFunction(engine.FunctionInstancePrototype, "then", 2, __STUB__Then), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Catch(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is PromiseInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'catch' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return ((PromiseInstance)thisObj).Catch(TypeConverter.ToObject<FunctionInstance>(engine, args[0]));
			}
		}

		private static object __STUB__Then(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is PromiseInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'then' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return ((PromiseInstance)thisObj).Then(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), TypeConverter.ToObject<FunctionInstance>(engine, args[1]));
			}
		}
	}

}
