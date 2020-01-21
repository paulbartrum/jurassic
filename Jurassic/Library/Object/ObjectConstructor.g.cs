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
			return new List<PropertyNameAndValue>(21)
			{
				new PropertyNameAndValue("getPrototypeOf", new ClrStubFunction(engine.FunctionInstancePrototype, "getPrototypeOf", 1, __STUB__GetPrototypeOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getOwnPropertyDescriptor", new ClrStubFunction(engine.FunctionInstancePrototype, "getOwnPropertyDescriptor", 2, __STUB__GetOwnPropertyDescriptor), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getOwnPropertyNames", new ClrStubFunction(engine.FunctionInstancePrototype, "getOwnPropertyNames", 1, __STUB__GetOwnPropertyNames), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getOwnPropertySymbols", new ClrStubFunction(engine.FunctionInstancePrototype, "getOwnPropertySymbols", 1, __STUB__GetOwnPropertySymbols), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("create", new ClrStubFunction(engine.FunctionInstancePrototype, "create", 2, __STUB__Create), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("assign", new ClrStubFunction(engine.FunctionInstancePrototype, "assign", 2, __STUB__Assign), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("defineProperty", new ClrStubFunction(engine.FunctionInstancePrototype, "defineProperty", 3, __STUB__DefineProperty), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("defineProperties", new ClrStubFunction(engine.FunctionInstancePrototype, "defineProperties", 2, __STUB__DefineProperties), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("seal", new ClrStubFunction(engine.FunctionInstancePrototype, "seal", 1, __STUB__Seal), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("freeze", new ClrStubFunction(engine.FunctionInstancePrototype, "freeze", 1, __STUB__Freeze), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("preventExtensions", new ClrStubFunction(engine.FunctionInstancePrototype, "preventExtensions", 1, __STUB__PreventExtensions), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("isSealed", new ClrStubFunction(engine.FunctionInstancePrototype, "isSealed", 1, __STUB__IsSealed), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("isFrozen", new ClrStubFunction(engine.FunctionInstancePrototype, "isFrozen", 1, __STUB__IsFrozen), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("isExtensible", new ClrStubFunction(engine.FunctionInstancePrototype, "isExtensible", 1, __STUB__IsExtensible), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("keys", new ClrStubFunction(engine.FunctionInstancePrototype, "keys", 1, __STUB__Keys), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("is", new ClrStubFunction(engine.FunctionInstancePrototype, "is", 2, __STUB__Is), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("fromEntries", new ClrStubFunction(engine.FunctionInstancePrototype, "fromEntries", 1, __STUB__FromEntries), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ObjectConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'Call' is not generic.");
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
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'Construct' is not generic.");
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
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return GetPrototypeOf(TypeConverter.ToObject(engine, args[0]));
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

		private static object __STUB__GetOwnPropertyNames(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return GetOwnPropertyNames(TypeConverter.ToObject(engine, args[0]));
			}
		}

		private static object __STUB__GetOwnPropertySymbols(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
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
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
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
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
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
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return DefineProperties(args[0], TypeConverter.ToObject(engine, args[1]));
			}
		}

		private static object __STUB__Seal(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return Seal(TypeConverter.ToObject(engine, args[0]));
			}
		}

		private static object __STUB__Freeze(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return Freeze(TypeConverter.ToObject(engine, args[0]));
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

		private static object __STUB__IsSealed(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return IsSealed(TypeConverter.ToObject(engine, args[0]));
			}
		}

		private static object __STUB__IsFrozen(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return IsFrozen(TypeConverter.ToObject(engine, args[0]));
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

		private static object __STUB__Keys(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
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
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return FromEntries(TypeConverter.ToObject(engine, args[0]));
			}
		}
	}

}
