/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class SymbolInstance
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(6)
			{
				new PropertyNameAndValue("toString", new ClrStubFunction(engine.FunctionInstancePrototype, "toString", 0, __STUB__toString), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("valueOf", new ClrStubFunction(engine.FunctionInstancePrototype, "valueOf", 0, __STUB__valueOf), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__toString(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is SymbolInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'toString' is not generic.");
			return ((SymbolInstance)thisObj).ToStringJS();
		}

		private static object __STUB__valueOf(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is SymbolInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'valueOf' is not generic.");
			return ((SymbolInstance)thisObj).ValueOf();
		}
	}

}
