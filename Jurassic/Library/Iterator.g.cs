/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	internal partial class Iterator
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(6)
			{
				new PropertyNameAndValue(Symbol.ToStringTag, new PropertyDescriptor(new ClrStubFunction(engine, "get [Symbol.toStringTag]", 0, __GETTER__ToStringTag), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue(Symbol.Iterator, new ClrStubFunction(engine, "[Symbol.iterator]", 0, __STUB__GetIterator), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("next", new ClrStubFunction(engine, "next", 0, __STUB__Next), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __GETTER__ToStringTag(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is Iterator))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'get [Symbol.toStringTag]' is not generic.");
			return ((Iterator)thisObj).ToStringTag;
		}

		private static object __STUB__GetIterator(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is Iterator))
				throw new JavaScriptException(ErrorType.TypeError, "The method '[Symbol.iterator]' is not generic.");
			return ((Iterator)thisObj).GetIterator();
		}

		private static object __STUB__Next(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is Iterator))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'next' is not generic.");
			return ((Iterator)thisObj).Next();
		}
	}

}
