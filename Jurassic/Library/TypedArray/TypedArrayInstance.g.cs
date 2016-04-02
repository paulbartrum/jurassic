/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class TypedArrayInstance
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(8)
			{
				new PropertyNameAndValue("buffer", new PropertyDescriptor(new ClrStubFunction(engine.FunctionInstancePrototype, "get buffer", 0, __GETTER__Buffer), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("byteOffset", new PropertyDescriptor(new ClrStubFunction(engine.FunctionInstancePrototype, "get byteOffset", 0, __GETTER__ByteOffset), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("byteLength", new PropertyDescriptor(new ClrStubFunction(engine.FunctionInstancePrototype, "get byteLength", 0, __GETTER__ByteLength), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("length", new PropertyDescriptor(new ClrStubFunction(engine.FunctionInstancePrototype, "get length", 0, __GETTER__Length), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue(engine.Symbol.ToStringTag, new PropertyDescriptor(new ClrStubFunction(engine.FunctionInstancePrototype, "get [Symbol.toStringTag]", 0, __GETTER__ToStringTag), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("entries", new ClrStubFunction(engine.FunctionInstancePrototype, "entries", 0, __STUB__Entries), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("keys", new ClrStubFunction(engine.FunctionInstancePrototype, "keys", 0, __STUB__Keys), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("subarray", new ClrStubFunction(engine.FunctionInstancePrototype, "subarray", 2, __STUB__Subarray), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("values", new ClrStubFunction(engine.FunctionInstancePrototype, "values", 0, __STUB__Values), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __GETTER__Buffer(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get buffer' is not generic.");
			return ((TypedArrayInstance)thisObj).Buffer;
		}

		private static object __GETTER__ByteOffset(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get byteOffset' is not generic.");
			return ((TypedArrayInstance)thisObj).ByteOffset;
		}

		private static object __GETTER__ByteLength(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get byteLength' is not generic.");
			return ((TypedArrayInstance)thisObj).ByteLength;
		}

		private static object __GETTER__Length(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get length' is not generic.");
			return ((TypedArrayInstance)thisObj).Length;
		}

		private static object __GETTER__ToStringTag(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get [Symbol.toStringTag]' is not generic.");
			return ((TypedArrayInstance)thisObj).ToStringTag;
		}

		private static object __STUB__Entries(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'entries' is not generic.");
			((TypedArrayInstance)thisObj).Entries(); return Undefined.Value;
		}

		private static object __STUB__Keys(ScriptEngine engine, object thisObj, object[] args)
		{
			Keys(); return Undefined.Value;
		}

		private static object __STUB__Subarray(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'subarray' is not generic.");
			switch (args.Length)
			{
				case 0:
					((TypedArrayInstance)thisObj).Subarray(0, int.MaxValue); return Undefined.Value;
				case 1:
					((TypedArrayInstance)thisObj).Subarray(TypeUtilities.IsUndefined(args[0]) ? 0 : TypeConverter.ToInteger(args[0]), int.MaxValue); return Undefined.Value;
				default:
					((TypedArrayInstance)thisObj).Subarray(TypeUtilities.IsUndefined(args[0]) ? 0 : TypeConverter.ToInteger(args[0]), TypeUtilities.IsUndefined(args[1]) ? int.MaxValue : TypeConverter.ToInteger(args[1])); return Undefined.Value;
			}
		}

		private static object __STUB__Values(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'values' is not generic.");
			((TypedArrayInstance)thisObj).Values(); return Undefined.Value;
		}
	}

}
