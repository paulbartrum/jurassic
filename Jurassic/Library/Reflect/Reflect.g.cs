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
				new PropertyNameAndValue("get", new ClrStubFunction(engine, "get", 2, __STUB__Get), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getOwnPropertyDescriptor", new ClrStubFunction(engine, "getOwnPropertyDescriptor", 2, __STUB__GetOwnPropertyDescriptor), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getPrototypeOf", new ClrStubFunction(engine, "getPrototypeOf", 1, __STUB__GetPrototypeOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("has", new ClrStubFunction(engine, "has", 2, __STUB__Has), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("isExtensible", new ClrStubFunction(engine, "isExtensible", 1, __STUB__IsExtensible), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("ownKeys", new ClrStubFunction(engine, "ownKeys", 1, __STUB__OwnKeys), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("preventExtensions", new ClrStubFunction(engine, "preventExtensions", 1, __STUB__PreventExtensions), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("set", new ClrStubFunction(engine, "set", 3, __STUB__Set), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("setPrototypeOf", new ClrStubFunction(engine, "setPrototypeOf", 2, __STUB__SetPrototypeOf), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Apply(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				case 2:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return Apply(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), args[1], TypeConverter.ToObject(engine, args[2]));
			}
		}

		private static object __STUB__Construct(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
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
					return DefineProperty(Undefined.Value, Undefined.Value, Undefined.Value);
				case 1:
					return DefineProperty(args[0], Undefined.Value, Undefined.Value);
				case 2:
					return DefineProperty(args[0], args[1], Undefined.Value);
				default:
					return DefineProperty(args[0], args[1], args[2]);
			}
		}

		private static object __STUB__DeleteProperty(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return DeleteProperty(Undefined.Value, Undefined.Value);
				case 1:
					return DeleteProperty(args[0], Undefined.Value);
				default:
					return DeleteProperty(args[0], args[1]);
			}
		}

		private static object __STUB__Get(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Get(Undefined.Value, Undefined.Value, null);
				case 1:
					return Get(args[0], Undefined.Value, null);
				case 2:
					return Get(args[0], args[1], null);
				default:
					return Get(args[0], args[1], TypeUtilities.IsUndefined(args[2]) ? null : args[2]);
			}
		}

		private static object __STUB__GetOwnPropertyDescriptor(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return GetOwnPropertyDescriptor(Undefined.Value, Undefined.Value);
				case 1:
					return GetOwnPropertyDescriptor(args[0], Undefined.Value);
				default:
					return GetOwnPropertyDescriptor(args[0], args[1]);
			}
		}

		private static object __STUB__GetPrototypeOf(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return GetPrototypeOf(Undefined.Value);
				default:
					return GetPrototypeOf(args[0]);
			}
		}

		private static object __STUB__Has(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Has(Undefined.Value, Undefined.Value);
				case 1:
					return Has(args[0], Undefined.Value);
				default:
					return Has(args[0], args[1]);
			}
		}

		private static object __STUB__IsExtensible(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return IsExtensible(Undefined.Value);
				default:
					return IsExtensible(args[0]);
			}
		}

		private static object __STUB__OwnKeys(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return OwnKeys(Undefined.Value);
				default:
					return OwnKeys(args[0]);
			}
		}

		private static object __STUB__PreventExtensions(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return PreventExtensions(Undefined.Value);
				default:
					return PreventExtensions(args[0]);
			}
		}

		private static object __STUB__Set(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Set(Undefined.Value, Undefined.Value, Undefined.Value, Undefined.Value);
				case 1:
					return Set(args[0], Undefined.Value, Undefined.Value, Undefined.Value);
				case 2:
					return Set(args[0], args[1], Undefined.Value, Undefined.Value);
				case 3:
					return Set(args[0], args[1], args[2], Undefined.Value);
				default:
					return Set(args[0], args[1], args[2], args[3]);
			}
		}

		private static object __STUB__SetPrototypeOf(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return SetPrototypeOf(Undefined.Value, Undefined.Value);
				case 1:
					return SetPrototypeOf(args[0], Undefined.Value);
				default:
					return SetPrototypeOf(args[0], args[1]);
			}
		}
	}

}
