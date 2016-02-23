/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class BooleanInstance
	{
		private List<PropertyNameAndValue> GetDeclarativeProperties()
		{
			return new List<PropertyNameAndValue>(6)
			{
				new PropertyNameAndValue("valueOf", new ClrStubFunction(Engine.FunctionInstancePrototype, "valueOf", 0, __STUB__valueOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toString", new ClrStubFunction(Engine.FunctionInstancePrototype, "toString", 0, __STUB__toString), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__valueOf(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is BooleanInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'valueOf' is not generic.");
			return ((BooleanInstance)thisObj).ValueOf();
		}

		private static object __STUB__toString(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is BooleanInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'toString' is not generic.");
			return ((BooleanInstance)thisObj).ToStringJS();
		}
	}

}
