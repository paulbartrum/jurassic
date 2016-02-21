/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class ObjectInstance
	{
		internal new List<PropertyNameAndValue> GetDeclarativeProperties()
		{
			return new List<PropertyNameAndValue>(10)
			{
				new PropertyNameAndValue("hasOwnProperty", new ClrStubFunction(this.Engine.Function.InstancePrototype, "hasOwnProperty", 1, __STUB__hasOwnProperty), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("isPrototypeOf", new ClrStubFunction(this.Engine.Function.InstancePrototype, "isPrototypeOf", 1, __STUB__isPrototypeOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("propertyIsEnumerable", new ClrStubFunction(this.Engine.Function.InstancePrototype, "propertyIsEnumerable", 1, __STUB__propertyIsEnumerable), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toLocaleString", new ClrStubFunction(this.Engine.Function.InstancePrototype, "toLocaleString", 0, __STUB__toLocaleString), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("valueOf", new ClrStubFunction(this.Engine.Function.InstancePrototype, "valueOf", 0, __STUB__valueOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toString", new ClrStubFunction(this.Engine.Function.InstancePrototype, "toString", 0, __STUB__toString), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__hasOwnProperty(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return HasOwnProperty(engine, thisObj, "undefined");
				default:
					return HasOwnProperty(engine, thisObj, TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__isPrototypeOf(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return IsPrototypeOf(engine, thisObj, Undefined.Value);
				default:
					return IsPrototypeOf(engine, thisObj, args[0]);
			}
		}

		private static object __STUB__propertyIsEnumerable(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return PropertyIsEnumerable(engine, thisObj, "undefined");
				default:
					return PropertyIsEnumerable(engine, thisObj, TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__toLocaleString(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ObjectInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'toLocaleString' is not generic.");
			return ((ObjectInstance)thisObj).ToLocaleString();
		}

		private static object __STUB__valueOf(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ObjectInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'valueOf' is not generic.");
			return ((ObjectInstance)thisObj).ValueOf();
		}

		private static object __STUB__toString(ScriptEngine engine, object thisObj, object[] args)
		{
			return ToStringJS(engine, thisObj);
		}
	}

}
