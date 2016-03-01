/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class ArrayBufferConstructor
	{
		private List<PropertyNameAndValue> GetDeclarativeProperties()
		{
			return new List<PropertyNameAndValue>(5)
			{
				new PropertyNameAndValue("isView", new ClrStubFunction(Engine.FunctionInstancePrototype, "isView", 1, __STUB__isView), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ArrayBufferConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'Call' is not generic.");
			return ((ArrayBufferConstructor)thisObj).Call();
		}

		private static ObjectInstance __STUB__Construct(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ArrayBufferConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'Construct' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((ArrayBufferConstructor)thisObj).Construct(0);
				default:
					return ((ArrayBufferConstructor)thisObj).Construct(TypeConverter.ToInteger(args[0]));
			}
		}

		private static object __STUB__isView(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return IsView(Undefined.Value);
				default:
					return IsView(args[0]);
			}
		}
	}

}
