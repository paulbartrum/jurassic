/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class ObjectConstructor
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(22)
			{
				new PropertyNameAndValue("getPrototypeOf", new ClrStubFunction(engine, "getPrototypeOf", 1, __STUB__GetPrototypeOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("setPrototypeOf", new ClrStubFunction(engine, "setPrototypeOf", 2, __STUB__SetPrototypeOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getOwnPropertyDescriptor", new ClrStubFunction(engine, "getOwnPropertyDescriptor", 2, __STUB__GetOwnPropertyDescriptor), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getOwnPropertyNames", new ClrStubFunction(engine, "getOwnPropertyNames", 1, __STUB__GetOwnPropertyNames), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getOwnPropertySymbols", new ClrStubFunction(engine, "getOwnPropertySymbols", 1, __STUB__GetOwnPropertySymbols), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("create", new ClrStubFunction(engine, "create", 2, __STUB__Create), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("assign", new ClrStubFunction(engine, "assign", 2, __STUB__Assign), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("defineProperty", new ClrStubFunction(engine, "defineProperty", 3, __STUB__DefineProperty), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("defineProperties", new ClrStubFunction(engine, "defineProperties", 2, __STUB__DefineProperties), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("seal", new ClrStubFunction(engine, "seal", 1, __STUB__Seal), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("freeze", new ClrStubFunction(engine, "freeze", 1, __STUB__Freeze), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("preventExtensions", new ClrStubFunction(engine, "preventExtensions", 1, __STUB__PreventExtensions), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("isSealed", new ClrStubFunction(engine, "isSealed", 1, __STUB__IsSealed), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("isFrozen", new ClrStubFunction(engine, "isFrozen", 1, __STUB__IsFrozen), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("isExtensible", new ClrStubFunction(engine, "isExtensible", 1, __STUB__IsExtensible), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("keys", new ClrStubFunction(engine, "keys", 1, __STUB__Keys), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("is", new ClrStubFunction(engine, "is", 2, __STUB__Is), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("fromEntries", new ClrStubFunction(engine, "fromEntries", 1, __STUB__FromEntries), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ObjectConstructor))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'Call' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((ObjectConstructor)thisObj).Call(Undefined.Value);
				default:
					return ((ObjectConstructor)thisObj).Call(args[0]);
			}
		}

		private static ObjectInstance __STUB__Construct(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ObjectConstructor))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'Construct' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((ObjectConstructor)thisObj).Construct();
				default:
					return ((ObjectConstructor)thisObj).Construct(args[0]);
			}
		}

		private static object __STUB__GetPrototypeOf(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return GetPrototypeOf(TypeConverter.ToObject(engine, args[0]));
			}
		}

		private static object __STUB__SetPrototypeOf(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return SetPrototypeOf(TypeConverter.ToObject(engine, args[0]), Undefined.Value);
				default:
					return SetPrototypeOf(TypeConverter.ToObject(engine, args[0]), args[1]);
			}
		}

		private static object __STUB__GetOwnPropertyDescriptor(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return GetOwnPropertyDescriptor(TypeConverter.ToObject(engine, args[0]), Undefined.Value);
				default:
					return GetOwnPropertyDescriptor(TypeConverter.ToObject(engine, args[0]), args[1]);
			}
		}

		private static object __STUB__GetOwnPropertyNames(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return GetOwnPropertyNames(TypeConverter.ToObject(engine, args[0]));
			}
		}

		private static object __STUB__GetOwnPropertySymbols(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return GetOwnPropertySymbols(TypeConverter.ToObject(engine, args[0]));
			}
		}

		private static object __STUB__Create(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Create(engine, Undefined.Value, null);
				case 1:
					return Create(engine, args[0], null);
				default:
					return Create(engine, args[0], TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToObject(engine, args[1]));
			}
		}

		private static object __STUB__Assign(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return Assign(engine, TypeConverter.ToObject(engine, args[0]), new object[0]);
				default:
					return Assign(engine, TypeConverter.ToObject(engine, args[0]), TypeUtilities.SliceArray(args, 1));
			}
		}

		private static object __STUB__DefineProperty(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return DefineProperty(TypeConverter.ToObject(engine, args[0]), Undefined.Value, Undefined.Value);
				case 2:
					return DefineProperty(TypeConverter.ToObject(engine, args[0]), args[1], Undefined.Value);
				default:
					return DefineProperty(TypeConverter.ToObject(engine, args[0]), args[1], args[2]);
			}
		}

		private static object __STUB__DefineProperties(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return DefineProperties(args[0], TypeConverter.ToObject(engine, args[1]));
			}
		}

		private static object __STUB__Seal(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Seal(Undefined.Value);
				default:
					return Seal(args[0]);
			}
		}

		private static object __STUB__Freeze(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Freeze(Undefined.Value);
				default:
					return Freeze(args[0]);
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

		private static object __STUB__IsSealed(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return IsSealed(Undefined.Value);
				default:
					return IsSealed(args[0]);
			}
		}

		private static object __STUB__IsFrozen(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return IsFrozen(Undefined.Value);
				default:
					return IsFrozen(args[0]);
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

		private static object __STUB__Keys(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return Keys(TypeConverter.ToObject(engine, args[0]));
			}
		}

		private static object __STUB__Is(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Is(Undefined.Value, Undefined.Value);
				case 1:
					return Is(args[0], Undefined.Value);
				default:
					return Is(args[0], args[1]);
			}
		}

		private static object __STUB__FromEntries(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return FromEntries(TypeConverter.ToObject(engine, args[0]));
			}
		}
	}

}
