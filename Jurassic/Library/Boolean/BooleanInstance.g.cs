/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class BooleanInstance
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(6)
			{
				new PropertyNameAndValue("valueOf", new ClrStubFunction(engine, "valueOf", 0, __STUB__ValueOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toString", new ClrStubFunction(engine, "toString", 0, __STUB__ToStringJS), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__ValueOf(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is BooleanInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'valueOf' is not generic.");
			return ((BooleanInstance)thisObj).ValueOf();
		}

		private static object __STUB__ToStringJS(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is BooleanInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'toString' is not generic.");
			return ((BooleanInstance)thisObj).ToStringJS();
		}
	}

}
