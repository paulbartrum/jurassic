/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class ArgumentsInstance
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(5)
			{
				new PropertyNameAndValue(engine.Symbol.Iterator, new ClrStubFunction(engine, "[Symbol.iterator]", 0, __STUB__GetIterator), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__GetIterator(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ArgumentsInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method '[Symbol.iterator]' is not generic.");
			return ((ArgumentsInstance)thisObj).GetIterator();
		}
	}

}
