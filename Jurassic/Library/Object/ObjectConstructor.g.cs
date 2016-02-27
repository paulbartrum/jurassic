/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class ObjectConstructor
	{
		private List<PropertyNameAndValue> GetDeclarativeProperties()
		{
			return new List<PropertyNameAndValue>(19)
			{
				new PropertyNameAndValue("getPrototypeOf", new ClrStubFunction(Engine.FunctionInstancePrototype, "getPrototypeOf", 1, __STUB__getPrototypeOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getOwnPropertyDescriptor", new ClrStubFunction(Engine.FunctionInstancePrototype, "getOwnPropertyDescriptor", 2, __STUB__getOwnPropertyDescriptor), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("getOwnPropertyNames", new ClrStubFunction(Engine.FunctionInstancePrototype, "getOwnPropertyNames", 1, __STUB__getOwnPropertyNames), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("create", new ClrStubFunction(Engine.FunctionInstancePrototype, "create", 2, __STUB__create), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("assign", new ClrStubFunction(Engine.FunctionInstancePrototype, "assign", 2, __STUB__assign), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("defineProperty", new ClrStubFunction(Engine.FunctionInstancePrototype, "defineProperty", 3, __STUB__defineProperty), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("defineProperties", new ClrStubFunction(Engine.FunctionInstancePrototype, "defineProperties", 2, __STUB__defineProperties), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("seal", new ClrStubFunction(Engine.FunctionInstancePrototype, "seal", 1, __STUB__seal), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("freeze", new ClrStubFunction(Engine.FunctionInstancePrototype, "freeze", 1, __STUB__freeze), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("preventExtensions", new ClrStubFunction(Engine.FunctionInstancePrototype, "preventExtensions", 1, __STUB__preventExtensions), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("isSealed", new ClrStubFunction(Engine.FunctionInstancePrototype, "isSealed", 1, __STUB__isSealed), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("isFrozen", new ClrStubFunction(Engine.FunctionInstancePrototype, "isFrozen", 1, __STUB__isFrozen), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("isExtensible", new ClrStubFunction(Engine.FunctionInstancePrototype, "isExtensible", 1, __STUB__isExtensible), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("keys", new ClrStubFunction(Engine.FunctionInstancePrototype, "keys", 1, __STUB__keys), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("is", new ClrStubFunction(Engine.FunctionInstancePrototype, "is", 2, __STUB__is), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ObjectConstructor))
				throw new JavaScriptException(engine, "TypeError", "The method 'Call' is not generic.");
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
				throw new JavaScriptException(engine, "TypeError", "The method 'Construct' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((ObjectConstructor)thisObj).Construct();
				default:
					return ((ObjectConstructor)thisObj).Construct(args[0]);
			}
		}

		private static object __STUB__getPrototypeOf(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "undefined cannot be converted to an object");
				default:
					return GetPrototypeOf(TypeConverter.ToObject(engine, args[0]));
			}
		}

		private static object __STUB__getOwnPropertyDescriptor(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "undefined cannot be converted to an object");
				case 1:
					return GetOwnPropertyDescriptor(TypeConverter.ToObject(engine, args[0]), "undefined");
				default:
					return GetOwnPropertyDescriptor(TypeConverter.ToObject(engine, args[0]), TypeConverter.ToString(args[1]));
			}
		}

		private static object __STUB__getOwnPropertyNames(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "undefined cannot be converted to an object");
				default:
					return GetOwnPropertyNames(TypeConverter.ToObject(engine, args[0]));
			}
		}

		private static object __STUB__create(ScriptEngine engine, object thisObj, object[] args)
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

		private static object __STUB__assign(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "undefined cannot be converted to an object");
				case 1:
					return Assign(engine, TypeConverter.ToObject(engine, args[0]), new object[0]);
				default:
					return Assign(engine, TypeConverter.ToObject(engine, args[0]), TypeUtilities.SliceArray(args, 1));
			}
		}

		private static object __STUB__defineProperty(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "undefined cannot be converted to an object");
				case 1:
					return DefineProperty(TypeConverter.ToObject(engine, args[0]), "undefined", Undefined.Value);
				case 2:
					return DefineProperty(TypeConverter.ToObject(engine, args[0]), TypeConverter.ToString(args[1]), Undefined.Value);
				default:
					return DefineProperty(TypeConverter.ToObject(engine, args[0]), TypeConverter.ToString(args[1]), args[2]);
			}
		}

		private static object __STUB__defineProperties(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "undefined cannot be converted to an object");
				case 1:
					throw new JavaScriptException(engine, "TypeError", "undefined cannot be converted to an object");
				default:
					return DefineProperties(args[0], TypeConverter.ToObject(engine, args[1]));
			}
		}

		private static object __STUB__seal(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "undefined cannot be converted to an object");
				default:
					return Seal(TypeConverter.ToObject(engine, args[0]));
			}
		}

		private static object __STUB__freeze(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "undefined cannot be converted to an object");
				default:
					return Freeze(TypeConverter.ToObject(engine, args[0]));
			}
		}

		private static object __STUB__preventExtensions(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "undefined cannot be converted to an object");
				default:
					return PreventExtensions(TypeConverter.ToObject(engine, args[0]));
			}
		}

		private static object __STUB__isSealed(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "undefined cannot be converted to an object");
				default:
					return IsSealed(TypeConverter.ToObject(engine, args[0]));
			}
		}

		private static object __STUB__isFrozen(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "undefined cannot be converted to an object");
				default:
					return IsFrozen(TypeConverter.ToObject(engine, args[0]));
			}
		}

		private static object __STUB__isExtensible(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "undefined cannot be converted to an object");
				default:
					return IsExtensible(TypeConverter.ToObject(engine, args[0]));
			}
		}

		private static object __STUB__keys(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, "TypeError", "undefined cannot be converted to an object");
				default:
					return Keys(TypeConverter.ToObject(engine, args[0]));
			}
		}

		private static object __STUB__is(ScriptEngine engine, object thisObj, object[] args)
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
	}

}
