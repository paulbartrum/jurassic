/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class DataViewInstance
	{
		private List<PropertyNameAndValue> GetDeclarativeProperties()
		{
			return new List<PropertyNameAndValue>(20)
			{
				new PropertyNameAndValue("buffer", new PropertyDescriptor(new ClrStubFunction(Engine.FunctionInstancePrototype, "get buffer", 0, __GETTER__buffer), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("byteOffset", new PropertyDescriptor(new ClrStubFunction(Engine.FunctionInstancePrototype, "get byteOffset", 0, __GETTER__byteOffset), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("byteLength", new PropertyDescriptor(new ClrStubFunction(Engine.FunctionInstancePrototype, "get byteLength", 0, __GETTER__byteLength), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("getFloat32", new ClrStubFunction(Engine.FunctionInstancePrototype, "getFloat32", 2, __STUB__getFloat32), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getFloat64", new ClrStubFunction(Engine.FunctionInstancePrototype, "getFloat64", 2, __STUB__getFloat64), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getInt16", new ClrStubFunction(Engine.FunctionInstancePrototype, "getInt16", 2, __STUB__getInt16), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getInt32", new ClrStubFunction(Engine.FunctionInstancePrototype, "getInt32", 2, __STUB__getInt32), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getInt8", new ClrStubFunction(Engine.FunctionInstancePrototype, "getInt8", 1, __STUB__getInt8), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getUint16", new ClrStubFunction(Engine.FunctionInstancePrototype, "getUint16", 2, __STUB__getUint16), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getUint32", new ClrStubFunction(Engine.FunctionInstancePrototype, "getUint32", 2, __STUB__getUint32), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getUint8", new ClrStubFunction(Engine.FunctionInstancePrototype, "getUint8", 1, __STUB__getUint8), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("setFloat32", new ClrStubFunction(Engine.FunctionInstancePrototype, "setFloat32", 3, __STUB__setFloat32), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("setFloat64", new ClrStubFunction(Engine.FunctionInstancePrototype, "setFloat64", 3, __STUB__setFloat64), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("setInt16", new ClrStubFunction(Engine.FunctionInstancePrototype, "setInt16", 3, __STUB__setInt16), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("setInt32", new ClrStubFunction(Engine.FunctionInstancePrototype, "setInt32", 3, __STUB__setInt32), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("setInt8", new ClrStubFunction(Engine.FunctionInstancePrototype, "setInt8", 2, __STUB__setInt8), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("setUint16", new ClrStubFunction(Engine.FunctionInstancePrototype, "setUint16", 3, __STUB__setUint16), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("setUint32", new ClrStubFunction(Engine.FunctionInstancePrototype, "setUint32", 3, __STUB__setUint32), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("setUint8", new ClrStubFunction(Engine.FunctionInstancePrototype, "setUint8", 2, __STUB__setUint8), PropertyAttributes.NonEnumerable),
			};
		}

		object __GETTER__buffer(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'get buffer' is not generic.");
			return ((DataViewInstance)thisObj).Buffer;
		}

		object __GETTER__byteOffset(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'get byteOffset' is not generic.");
			return ((DataViewInstance)thisObj).ByteOffset;
		}

		object __GETTER__byteLength(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'get byteLength' is not generic.");
			return ((DataViewInstance)thisObj).ByteLength;
		}

		private static object __STUB__getFloat32(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'getFloat32' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'byteOffset' was not specified.");
				case 1:
					return ((DataViewInstance)thisObj).GetFloat32(TypeConverter.ToInteger(args[0]), false);
				default:
					return ((DataViewInstance)thisObj).GetFloat32(TypeConverter.ToInteger(args[0]), TypeConverter.ToBoolean(args[1]));
			}
		}

		private static object __STUB__getFloat64(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'getFloat64' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'byteOffset' was not specified.");
				case 1:
					return ((DataViewInstance)thisObj).GetFloat64(TypeConverter.ToInteger(args[0]), false);
				default:
					return ((DataViewInstance)thisObj).GetFloat64(TypeConverter.ToInteger(args[0]), TypeConverter.ToBoolean(args[1]));
			}
		}

		private static object __STUB__getInt16(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'getInt16' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'byteOffset' was not specified.");
				case 1:
					return ((DataViewInstance)thisObj).GetInt16(TypeConverter.ToInteger(args[0]), false);
				default:
					return ((DataViewInstance)thisObj).GetInt16(TypeConverter.ToInteger(args[0]), TypeConverter.ToBoolean(args[1]));
			}
		}

		private static object __STUB__getInt32(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'getInt32' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'byteOffset' was not specified.");
				case 1:
					return ((DataViewInstance)thisObj).GetInt32(TypeConverter.ToInteger(args[0]), false);
				default:
					return ((DataViewInstance)thisObj).GetInt32(TypeConverter.ToInteger(args[0]), TypeConverter.ToBoolean(args[1]));
			}
		}

		private static object __STUB__getInt8(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'getInt8' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'byteOffset' was not specified.");
				default:
					return ((DataViewInstance)thisObj).GetInt8(TypeConverter.ToInteger(args[0]));
			}
		}

		private static object __STUB__getUint16(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'getUint16' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'byteOffset' was not specified.");
				case 1:
					return ((DataViewInstance)thisObj).GetUint16(TypeConverter.ToInteger(args[0]), false);
				default:
					return ((DataViewInstance)thisObj).GetUint16(TypeConverter.ToInteger(args[0]), TypeConverter.ToBoolean(args[1]));
			}
		}

		private static object __STUB__getUint32(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'getUint32' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'byteOffset' was not specified.");
				case 1:
					return ((DataViewInstance)thisObj).GetUint32(TypeConverter.ToInteger(args[0]), false);
				default:
					return ((DataViewInstance)thisObj).GetUint32(TypeConverter.ToInteger(args[0]), TypeConverter.ToBoolean(args[1]));
			}
		}

		private static object __STUB__getUint8(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'getUint8' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'byteOffset' was not specified.");
				default:
					return ((DataViewInstance)thisObj).GetUint8(TypeConverter.ToInteger(args[0]));
			}
		}

		private static object __STUB__setFloat32(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'setFloat32' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'byteOffset' was not specified.");
				case 1:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'value' was not specified.");
				case 2:
					((DataViewInstance)thisObj).SetFloat32(TypeConverter.ToInteger(args[0]), TypeConverter.ToNumber(args[1]), false); return Undefined.Value;
				default:
					((DataViewInstance)thisObj).SetFloat32(TypeConverter.ToInteger(args[0]), TypeConverter.ToNumber(args[1]), TypeConverter.ToBoolean(args[2])); return Undefined.Value;
			}
		}

		private static object __STUB__setFloat64(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'setFloat64' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'byteOffset' was not specified.");
				case 1:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'value' was not specified.");
				case 2:
					((DataViewInstance)thisObj).SetFloat64(TypeConverter.ToInteger(args[0]), TypeConverter.ToNumber(args[1]), false); return Undefined.Value;
				default:
					((DataViewInstance)thisObj).SetFloat64(TypeConverter.ToInteger(args[0]), TypeConverter.ToNumber(args[1]), TypeConverter.ToBoolean(args[2])); return Undefined.Value;
			}
		}

		private static object __STUB__setInt16(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'setInt16' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'byteOffset' was not specified.");
				case 1:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'value' was not specified.");
				case 2:
					((DataViewInstance)thisObj).SetInt16(TypeConverter.ToInteger(args[0]), args[1], false); return Undefined.Value;
				default:
					((DataViewInstance)thisObj).SetInt16(TypeConverter.ToInteger(args[0]), args[1], TypeConverter.ToBoolean(args[2])); return Undefined.Value;
			}
		}

		private static object __STUB__setInt32(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'setInt32' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'byteOffset' was not specified.");
				case 1:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'value' was not specified.");
				case 2:
					((DataViewInstance)thisObj).SetInt32(TypeConverter.ToInteger(args[0]), args[1], false); return Undefined.Value;
				default:
					((DataViewInstance)thisObj).SetInt32(TypeConverter.ToInteger(args[0]), args[1], TypeConverter.ToBoolean(args[2])); return Undefined.Value;
			}
		}

		private static object __STUB__setInt8(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'setInt8' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'byteOffset' was not specified.");
				case 1:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'value' was not specified.");
				default:
					((DataViewInstance)thisObj).SetInt8(TypeConverter.ToInteger(args[0]), args[1]); return Undefined.Value;
			}
		}

		private static object __STUB__setUint16(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'setUint16' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'byteOffset' was not specified.");
				case 1:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'value' was not specified.");
				case 2:
					((DataViewInstance)thisObj).SetUint16(TypeConverter.ToInteger(args[0]), args[1], false); return Undefined.Value;
				default:
					((DataViewInstance)thisObj).SetUint16(TypeConverter.ToInteger(args[0]), args[1], TypeConverter.ToBoolean(args[2])); return Undefined.Value;
			}
		}

		private static object __STUB__setUint32(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'setUint32' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'byteOffset' was not specified.");
				case 1:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'value' was not specified.");
				case 2:
					((DataViewInstance)thisObj).SetUint32(TypeConverter.ToInteger(args[0]), args[1], false); return Undefined.Value;
				default:
					((DataViewInstance)thisObj).SetUint32(TypeConverter.ToInteger(args[0]), args[1], TypeConverter.ToBoolean(args[2])); return Undefined.Value;
			}
		}

		private static object __STUB__setUint8(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'setUint8' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'byteOffset' was not specified.");
				case 1:
					throw new JavaScriptException(engine, "TypeError", "Required argument 'value' was not specified.");
				default:
					((DataViewInstance)thisObj).SetUint8(TypeConverter.ToInteger(args[0]), args[1]); return Undefined.Value;
			}
		}
	}

}
