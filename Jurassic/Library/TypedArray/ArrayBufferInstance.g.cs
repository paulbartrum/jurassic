/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class ArrayBufferInstance
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(6)
			{
				new PropertyNameAndValue("byteLength", new PropertyDescriptor(new ClrStubFunction(engine, "get byteLength", 0, __GETTER__ByteLength), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("slice", new ClrStubFunction(engine, "slice", 2, __STUB__Slice), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __GETTER__ByteLength(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ArrayBufferInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'get byteLength' is not generic.");
			return ((ArrayBufferInstance)thisObj).ByteLength;
		}

		private static object __STUB__Slice(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ArrayBufferInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'slice' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((ArrayBufferInstance)thisObj).Slice(0);
				case 1:
					return ((ArrayBufferInstance)thisObj).Slice(TypeConverter.ToInteger(args[0]));
				default:
					return ((ArrayBufferInstance)thisObj).Slice(TypeConverter.ToInteger(args[0]), TypeConverter.ToInteger(args[1]));
			}
		}
	}

}
