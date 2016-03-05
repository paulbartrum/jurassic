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
			return new List<PropertyNameAndValue>(8)
			{
				new PropertyNameAndValue("toString", new ClrStubFunction(engine.FunctionInstancePrototype, "toString", 0, __STUB__toString), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("valueOf", new ClrStubFunction(engine.FunctionInstancePrototype, "valueOf", 0, __STUB__valueOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue(engine.Symbol.ToPrimitive, new ClrStubFunction(engine.FunctionInstancePrototype, "[Symbol.toPrimitive]", 1, __STUB__toPrimitive), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue(engine.Symbol.ToStringTag, new ClrStubFunction(engine.FunctionInstancePrototype, "[Symbol.toStringTag]", 0, __STUB__toStringTag), PropertyAttributes.NonEnumerable),
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

		private static object __STUB__toPrimitive(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is SymbolInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method '@@toPrimitive' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((SymbolInstance)thisObj).ToPrimitive("undefined");
				default:
					return ((SymbolInstance)thisObj).ToPrimitive(TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__toStringTag(ScriptEngine engine, object thisObj, object[] args)
		{
			return ToStringTag();
		}
	}

}
