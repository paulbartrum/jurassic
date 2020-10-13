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
			return new List<PropertyNameAndValue>(7)
			{
				new PropertyNameAndValue("toString", new ClrStubFunction(engine, "toString", 0, __STUB__ToString), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("valueOf", new ClrStubFunction(engine, "valueOf", 0, __STUB__ValueOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue(Symbol.ToPrimitive, new ClrStubFunction(engine, "[Symbol.toPrimitive]", 1, __STUB__ToPrimitive), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__ToString(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is SymbolInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'toString' is not generic.");
			return ((SymbolInstance)thisObj).ToString();
		}

		private static object __STUB__ValueOf(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is SymbolInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'valueOf' is not generic.");
			return ((SymbolInstance)thisObj).ValueOf();
		}

		private static object __STUB__ToPrimitive(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is SymbolInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method '[Symbol.toPrimitive]' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((SymbolInstance)thisObj).ToPrimitive("undefined");
				default:
					return ((SymbolInstance)thisObj).ToPrimitive(TypeConverter.ToString(args[0]));
			}
		}
	}

}
