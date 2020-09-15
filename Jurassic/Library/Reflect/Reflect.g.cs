/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class ReflectObject
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(17)
			{
				new PropertyNameAndValue("apply", new ClrStubFunction(engine, "apply", 3, __STUB__Apply), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("construct", new ClrStubFunction(engine, "construct", 2, __STUB__Construct), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("defineProperty", new ClrStubFunction(engine, "defineProperty", 3, __STUB__DefineProperty), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("deleteProperty", new ClrStubFunction(engine, "deleteProperty", 2, __STUB__DeleteProperty), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("get", new ClrStubFunction(engine, "get", 3, __STUB__Get), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getOwnPropertyDescriptor", new ClrStubFunction(engine, "getOwnPropertyDescriptor", 2, __STUB__GetOwnPropertyDescriptor), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getPrototypeOf", new ClrStubFunction(engine, "getPrototypeOf", 1, __STUB__GetPrototypeOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("has", new ClrStubFunction(engine, "has", 2, __STUB__Has), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("isExtensible", new ClrStubFunction(engine, "isExtensible", 1, __STUB__IsExtensible), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("ownKeys", new ClrStubFunction(engine, "ownKeys", 1, __STUB__OwnKeys), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("preventExtensions", new ClrStubFunction(engine, "preventExtensions", 1, __STUB__PreventExtensions), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("set", new ClrStubFunction(engine, "set", 4, __STUB__Set), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("setPrototypeOf", new ClrStubFunction(engine, "setPrototypeOf", 2, __STUB__SetPrototypeOf), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Apply(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return Apply(TypeConverter.ToObject(engine, args[0]), Undefined.Value, Undefined.Value);
				case 2:
					return Apply(TypeConverter.ToObject(engine, args[0]), args[1], Undefined.Value);
				default:
					return Apply(TypeConverter.ToObject(engine, args[0]), args[1], args[2]);
			}
		}

		private static object __STUB__Construct(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 2:
					return Construct(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), TypeConverter.ToObject(engine, args[1]), null);
				default:
					return Construct(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), TypeConverter.ToObject(engine, args[1]), TypeUtilities.IsUndefined(args[2]) ? null : TypeConverter.ToObject<FunctionInstance>(engine, args[2]));
			}
		}

		private static object __STUB__DefineProperty(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return DefineProperty(TypeConverter.ToObject(engine, args[0]), Undefined.Value, Undefined.Value);
				case 2:
					return DefineProperty(TypeConverter.ToObject(engine, args[0]), args[1], Undefined.Value);
				default:
					return DefineProperty(TypeConverter.ToObject(engine, args[0]), args[1], args[2]);
			}
		}

		private static object __STUB__DeleteProperty(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return DeleteProperty(TypeConverter.ToObject(engine, args[0]), Undefined.Value);
				default:
					return DeleteProperty(TypeConverter.ToObject(engine, args[0]), args[1]);
			}
		}

		private static object __STUB__Get(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return Get(TypeConverter.ToObject(engine, args[0]), Undefined.Value, Undefined.Value);
				case 2:
					return Get(TypeConverter.ToObject(engine, args[0]), args[1], Undefined.Value);
				default:
					return Get(TypeConverter.ToObject(engine, args[0]), args[1], args[2]);
			}
		}

		private static object __STUB__GetOwnPropertyDescriptor(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return GetOwnPropertyDescriptor(TypeConverter.ToObject(engine, args[0]), Undefined.Value);
				default:
					return GetOwnPropertyDescriptor(TypeConverter.ToObject(engine, args[0]), args[1]);
			}
		}

		private static object __STUB__GetPrototypeOf(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return GetPrototypeOf(TypeConverter.ToObject(engine, args[0]));
			}
		}

		private static object __STUB__Has(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return Has(TypeConverter.ToObject(engine, args[0]), Undefined.Value);
				default:
					return Has(TypeConverter.ToObject(engine, args[0]), args[1]);
			}
		}

		private static object __STUB__IsExtensible(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return IsExtensible(TypeConverter.ToObject(engine, args[0]));
			}
		}

		private static object __STUB__OwnKeys(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return OwnKeys(TypeConverter.ToObject(engine, args[0]));
			}
		}

		private static object __STUB__PreventExtensions(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return PreventExtensions(TypeConverter.ToObject(engine, args[0]));
			}
		}

		private static object __STUB__Set(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return Set(TypeConverter.ToObject(engine, args[0]), Undefined.Value, Undefined.Value, Undefined.Value);
				case 2:
					return Set(TypeConverter.ToObject(engine, args[0]), args[1], Undefined.Value, Undefined.Value);
				case 3:
					return Set(TypeConverter.ToObject(engine, args[0]), args[1], args[2], Undefined.Value);
				default:
					return Set(TypeConverter.ToObject(engine, args[0]), args[1], args[2], args[3]);
			}
		}

		private static object __STUB__SetPrototypeOf(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return SetPrototypeOf(TypeConverter.ToObject(engine, args[0]), Undefined.Value);
				default:
					return SetPrototypeOf(TypeConverter.ToObject(engine, args[0]), args[1]);
			}
		}
	}

}
