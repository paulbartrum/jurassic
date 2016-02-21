/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class ErrorInstance
	{
		internal new List<PropertyNameAndValue> GetDeclarativeProperties()
		{
			return new List<PropertyNameAndValue>(5)
			{
				new PropertyNameAndValue("toString", new ClrStubFunction(this.Engine.Function.InstancePrototype, "toString", 0, __STUB__toString), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__toString(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ErrorInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'toString' is not generic.");
			return ((ErrorInstance)thisObj).ToStringJS();
		}
	}

}
