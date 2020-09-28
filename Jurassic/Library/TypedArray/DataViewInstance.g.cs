/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class DataViewInstance
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(20)
			{
				new PropertyNameAndValue("buffer", new PropertyDescriptor(new ClrStubFunction(engine, "get buffer", 0, __GETTER__Buffer), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("byteOffset", new PropertyDescriptor(new ClrStubFunction(engine, "get byteOffset", 0, __GETTER__ByteOffset), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("byteLength", new PropertyDescriptor(new ClrStubFunction(engine, "get byteLength", 0, __GETTER__ByteLength), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("getFloat32", new ClrStubFunction(engine, "getFloat32", 2, __STUB__GetFloat32), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getFloat64", new ClrStubFunction(engine, "getFloat64", 2, __STUB__GetFloat64), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getInt16", new ClrStubFunction(engine, "getInt16", 2, __STUB__GetInt16), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getInt32", new ClrStubFunction(engine, "getInt32", 2, __STUB__GetInt32), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getInt8", new ClrStubFunction(engine, "getInt8", 1, __STUB__GetInt8), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getUint16", new ClrStubFunction(engine, "getUint16", 2, __STUB__GetUint16), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getUint32", new ClrStubFunction(engine, "getUint32", 2, __STUB__GetUint32), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getUint8", new ClrStubFunction(engine, "getUint8", 1, __STUB__GetUint8), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("setFloat32", new ClrStubFunction(engine, "setFloat32", 3, __STUB__SetFloat32), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("setFloat64", new ClrStubFunction(engine, "setFloat64", 3, __STUB__SetFloat64), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("setInt16", new ClrStubFunction(engine, "setInt16", 3, __STUB__SetInt16), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("setInt32", new ClrStubFunction(engine, "setInt32", 3, __STUB__SetInt32), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("setInt8", new ClrStubFunction(engine, "setInt8", 2, __STUB__SetInt8), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("setUint16", new ClrStubFunction(engine, "setUint16", 3, __STUB__SetUint16), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("setUint32", new ClrStubFunction(engine, "setUint32", 3, __STUB__SetUint32), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("setUint8", new ClrStubFunction(engine, "setUint8", 2, __STUB__SetUint8), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __GETTER__Buffer(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'get buffer' is not generic.");
			return ((DataViewInstance)thisObj).Buffer;
		}

		private static object __GETTER__ByteOffset(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'get byteOffset' is not generic.");
			return ((DataViewInstance)thisObj).ByteOffset;
		}

		private static object __GETTER__ByteLength(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'get byteLength' is not generic.");
			return ((DataViewInstance)thisObj).ByteLength;
		}

		private static object __STUB__GetFloat32(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'getFloat32' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'byteOffset' was not specified.");
				case 1:
					return ((DataViewInstance)thisObj).GetFloat32(TypeConverter.ToInteger(args[0]), false);
				default:
					return ((DataViewInstance)thisObj).GetFloat32(TypeConverter.ToInteger(args[0]), TypeConverter.ToBoolean(args[1]));
			}
		}

		private static object __STUB__GetFloat64(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'getFloat64' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'byteOffset' was not specified.");
				case 1:
					return ((DataViewInstance)thisObj).GetFloat64(TypeConverter.ToInteger(args[0]), false);
				default:
					return ((DataViewInstance)thisObj).GetFloat64(TypeConverter.ToInteger(args[0]), TypeConverter.ToBoolean(args[1]));
			}
		}

		private static object __STUB__GetInt16(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'getInt16' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'byteOffset' was not specified.");
				case 1:
					return ((DataViewInstance)thisObj).GetInt16(TypeConverter.ToInteger(args[0]), false);
				default:
					return ((DataViewInstance)thisObj).GetInt16(TypeConverter.ToInteger(args[0]), TypeConverter.ToBoolean(args[1]));
			}
		}

		private static object __STUB__GetInt32(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'getInt32' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'byteOffset' was not specified.");
				case 1:
					return ((DataViewInstance)thisObj).GetInt32(TypeConverter.ToInteger(args[0]), false);
				default:
					return ((DataViewInstance)thisObj).GetInt32(TypeConverter.ToInteger(args[0]), TypeConverter.ToBoolean(args[1]));
			}
		}

		private static object __STUB__GetInt8(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'getInt8' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'byteOffset' was not specified.");
				default:
					return ((DataViewInstance)thisObj).GetInt8(TypeConverter.ToInteger(args[0]));
			}
		}

		private static object __STUB__GetUint16(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'getUint16' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'byteOffset' was not specified.");
				case 1:
					return ((DataViewInstance)thisObj).GetUint16(TypeConverter.ToInteger(args[0]), false);
				default:
					return ((DataViewInstance)thisObj).GetUint16(TypeConverter.ToInteger(args[0]), TypeConverter.ToBoolean(args[1]));
			}
		}

		private static object __STUB__GetUint32(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'getUint32' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'byteOffset' was not specified.");
				case 1:
					return ((DataViewInstance)thisObj).GetUint32(TypeConverter.ToInteger(args[0]), false);
				default:
					return ((DataViewInstance)thisObj).GetUint32(TypeConverter.ToInteger(args[0]), TypeConverter.ToBoolean(args[1]));
			}
		}

		private static object __STUB__GetUint8(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'getUint8' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'byteOffset' was not specified.");
				default:
					return ((DataViewInstance)thisObj).GetUint8(TypeConverter.ToInteger(args[0]));
			}
		}

		private static object __STUB__SetFloat32(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'setFloat32' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'byteOffset' was not specified.");
				case 1:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'value' was not specified.");
				case 2:
					((DataViewInstance)thisObj).SetFloat32(TypeConverter.ToInteger(args[0]), TypeConverter.ToNumber(args[1]), false); return Undefined.Value;
				default:
					((DataViewInstance)thisObj).SetFloat32(TypeConverter.ToInteger(args[0]), TypeConverter.ToNumber(args[1]), TypeConverter.ToBoolean(args[2])); return Undefined.Value;
			}
		}

		private static object __STUB__SetFloat64(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'setFloat64' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'byteOffset' was not specified.");
				case 1:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'value' was not specified.");
				case 2:
					((DataViewInstance)thisObj).SetFloat64(TypeConverter.ToInteger(args[0]), TypeConverter.ToNumber(args[1]), false); return Undefined.Value;
				default:
					((DataViewInstance)thisObj).SetFloat64(TypeConverter.ToInteger(args[0]), TypeConverter.ToNumber(args[1]), TypeConverter.ToBoolean(args[2])); return Undefined.Value;
			}
		}

		private static object __STUB__SetInt16(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'setInt16' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'byteOffset' was not specified.");
				case 1:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'value' was not specified.");
				case 2:
					((DataViewInstance)thisObj).SetInt16(TypeConverter.ToInteger(args[0]), args[1], false); return Undefined.Value;
				default:
					((DataViewInstance)thisObj).SetInt16(TypeConverter.ToInteger(args[0]), args[1], TypeConverter.ToBoolean(args[2])); return Undefined.Value;
			}
		}

		private static object __STUB__SetInt32(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'setInt32' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'byteOffset' was not specified.");
				case 1:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'value' was not specified.");
				case 2:
					((DataViewInstance)thisObj).SetInt32(TypeConverter.ToInteger(args[0]), args[1], false); return Undefined.Value;
				default:
					((DataViewInstance)thisObj).SetInt32(TypeConverter.ToInteger(args[0]), args[1], TypeConverter.ToBoolean(args[2])); return Undefined.Value;
			}
		}

		private static object __STUB__SetInt8(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'setInt8' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'byteOffset' was not specified.");
				case 1:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'value' was not specified.");
				default:
					((DataViewInstance)thisObj).SetInt8(TypeConverter.ToInteger(args[0]), args[1]); return Undefined.Value;
			}
		}

		private static object __STUB__SetUint16(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'setUint16' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'byteOffset' was not specified.");
				case 1:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'value' was not specified.");
				case 2:
					((DataViewInstance)thisObj).SetUint16(TypeConverter.ToInteger(args[0]), args[1], false); return Undefined.Value;
				default:
					((DataViewInstance)thisObj).SetUint16(TypeConverter.ToInteger(args[0]), args[1], TypeConverter.ToBoolean(args[2])); return Undefined.Value;
			}
		}

		private static object __STUB__SetUint32(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'setUint32' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'byteOffset' was not specified.");
				case 1:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'value' was not specified.");
				case 2:
					((DataViewInstance)thisObj).SetUint32(TypeConverter.ToInteger(args[0]), args[1], false); return Undefined.Value;
				default:
					((DataViewInstance)thisObj).SetUint32(TypeConverter.ToInteger(args[0]), args[1], TypeConverter.ToBoolean(args[2])); return Undefined.Value;
			}
		}

		private static object __STUB__SetUint8(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'setUint8' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'byteOffset' was not specified.");
				case 1:
					throw new JavaScriptException(ErrorType.TypeError, "Required argument 'value' was not specified.");
				default:
					((DataViewInstance)thisObj).SetUint8(TypeConverter.ToInteger(args[0]), args[1]); return Undefined.Value;
			}
		}
	}

}
