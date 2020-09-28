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
			return new List<PropertyNameAndValue>(29)
			{
				new PropertyNameAndValue("buffer", new PropertyDescriptor(new ClrStubFunction(engine, "get buffer", 0, __GETTER__Buffer), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("byteOffset", new PropertyDescriptor(new ClrStubFunction(engine, "get byteOffset", 0, __GETTER__ByteOffset), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("byteLength", new PropertyDescriptor(new ClrStubFunction(engine, "get byteLength", 0, __GETTER__ByteLength), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("length", new PropertyDescriptor(new ClrStubFunction(engine, "get length", 0, __GETTER__Length), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue(engine.Symbol.ToStringTag, new PropertyDescriptor(new ClrStubFunction(engine, "get [Symbol.toStringTag]", 0, __GETTER__ToStringTag), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("entries", new ClrStubFunction(engine, "entries", 0, __STUB__Entries), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("keys", new ClrStubFunction(engine, "keys", 0, __STUB__Keys), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("subarray", new ClrStubFunction(engine, "subarray", 2, __STUB__Subarray), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("values", new ClrStubFunction(engine, "values", 0, __STUB__Values), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("copyWithin", new ClrStubFunction(engine, "copyWithin", 2, __STUB__CopyWithin), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("every", new ClrStubFunction(engine, "every", 1, __STUB__Every), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("fill", new ClrStubFunction(engine, "fill", 1, __STUB__Fill), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("filter", new ClrStubFunction(engine, "filter", 1, __STUB__Filter), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("find", new ClrStubFunction(engine, "find", 1, __STUB__Find), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("findIndex", new ClrStubFunction(engine, "findIndex", 1, __STUB__FindIndex), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("forEach", new ClrStubFunction(engine, "forEach", 1, __STUB__ForEach), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("indexOf", new ClrStubFunction(engine, "indexOf", 1, __STUB__IndexOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("join", new ClrStubFunction(engine, "join", 1, __STUB__Join), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("lastIndexOf", new ClrStubFunction(engine, "lastIndexOf", 1, __STUB__LastIndexOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("map", new ClrStubFunction(engine, "map", 1, __STUB__Map), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("reduce", new ClrStubFunction(engine, "reduce", 1, __STUB__Reduce), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("reduceRight", new ClrStubFunction(engine, "reduceRight", 1, __STUB__ReduceRight), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("reverse", new ClrStubFunction(engine, "reverse", 0, __STUB__Reverse), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("set", new ClrStubFunction(engine, "set", 1, __STUB__Set), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("slice", new ClrStubFunction(engine, "slice", 2, __STUB__Slice), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("some", new ClrStubFunction(engine, "some", 1, __STUB__Some), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("sort", new ClrStubFunction(engine, "sort", 1, __STUB__Sort), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toLocaleString", new ClrStubFunction(engine, "toLocaleString", 0, __STUB__ToLocaleString), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toString", new ClrStubFunction(engine, "toString", 0, __STUB__ToString), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __GETTER__Buffer(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'get buffer' is not generic.");
			return ((TypedArrayInstance)thisObj).Buffer;
		}

		private static object __GETTER__ByteOffset(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'get byteOffset' is not generic.");
			return ((TypedArrayInstance)thisObj).ByteOffset;
		}

		private static object __GETTER__ByteLength(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'get byteLength' is not generic.");
			return ((TypedArrayInstance)thisObj).ByteLength;
		}

		private static object __GETTER__Length(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'get length' is not generic.");
			return ((TypedArrayInstance)thisObj).Length;
		}

		private static object __GETTER__ToStringTag(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'get [Symbol.toStringTag]' is not generic.");
			return ((TypedArrayInstance)thisObj).ToStringTag;
		}

		private static object __STUB__Entries(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'entries' is not generic.");
			return ((TypedArrayInstance)thisObj).Entries();
		}

		private static object __STUB__Keys(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'keys' is not generic.");
			return ((TypedArrayInstance)thisObj).Keys();
		}

		private static object __STUB__Subarray(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'subarray' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((TypedArrayInstance)thisObj).Subarray(0, int.MaxValue);
				case 1:
					return ((TypedArrayInstance)thisObj).Subarray(TypeUtilities.IsUndefined(args[0]) ? 0 : TypeConverter.ToInteger(args[0]), int.MaxValue);
				default:
					return ((TypedArrayInstance)thisObj).Subarray(TypeUtilities.IsUndefined(args[0]) ? 0 : TypeConverter.ToInteger(args[0]), TypeUtilities.IsUndefined(args[1]) ? int.MaxValue : TypeConverter.ToInteger(args[1]));
			}
		}

		private static object __STUB__Values(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'values' is not generic.");
			return ((TypedArrayInstance)thisObj).Values();
		}

		private static object __STUB__CopyWithin(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'copyWithin' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((TypedArrayInstance)thisObj).CopyWithin(0, 0, int.MaxValue);
				case 1:
					return ((TypedArrayInstance)thisObj).CopyWithin(TypeConverter.ToInteger(args[0]), 0, int.MaxValue);
				case 2:
					return ((TypedArrayInstance)thisObj).CopyWithin(TypeConverter.ToInteger(args[0]), TypeConverter.ToInteger(args[1]), int.MaxValue);
				default:
					return ((TypedArrayInstance)thisObj).CopyWithin(TypeConverter.ToInteger(args[0]), TypeConverter.ToInteger(args[1]), TypeUtilities.IsUndefined(args[2]) ? int.MaxValue : TypeConverter.ToInteger(args[2]));
			}
		}

		private static object __STUB__Every(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'every' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return ((TypedArrayInstance)thisObj).Every(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), null);
				default:
					return ((TypedArrayInstance)thisObj).Every(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToObject(engine, args[1]));
			}
		}

		private static object __STUB__Fill(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'fill' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((TypedArrayInstance)thisObj).Fill(Undefined.Value, 0, int.MaxValue);
				case 1:
					return ((TypedArrayInstance)thisObj).Fill(args[0], 0, int.MaxValue);
				case 2:
					return ((TypedArrayInstance)thisObj).Fill(args[0], TypeUtilities.IsUndefined(args[1]) ? 0 : TypeConverter.ToInteger(args[1]), int.MaxValue);
				default:
					return ((TypedArrayInstance)thisObj).Fill(args[0], TypeUtilities.IsUndefined(args[1]) ? 0 : TypeConverter.ToInteger(args[1]), TypeUtilities.IsUndefined(args[2]) ? int.MaxValue : TypeConverter.ToInteger(args[2]));
			}
		}

		private static object __STUB__Filter(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'filter' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return ((TypedArrayInstance)thisObj).Filter(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), null);
				default:
					return ((TypedArrayInstance)thisObj).Filter(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToObject(engine, args[1]));
			}
		}

		private static object __STUB__Find(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'find' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return ((TypedArrayInstance)thisObj).Find(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), null);
				default:
					return ((TypedArrayInstance)thisObj).Find(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToObject(engine, args[1]));
			}
		}

		private static object __STUB__FindIndex(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'findIndex' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return ((TypedArrayInstance)thisObj).FindIndex(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), null);
				default:
					return ((TypedArrayInstance)thisObj).FindIndex(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToObject(engine, args[1]));
			}
		}

		private static object __STUB__ForEach(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'forEach' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					((TypedArrayInstance)thisObj).ForEach(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), null); return Undefined.Value;
				default:
					((TypedArrayInstance)thisObj).ForEach(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToObject(engine, args[1])); return Undefined.Value;
			}
		}

		private static object __STUB__IndexOf(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'indexOf' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((TypedArrayInstance)thisObj).IndexOf(Undefined.Value, 0);
				case 1:
					return ((TypedArrayInstance)thisObj).IndexOf(args[0], 0);
				default:
					return ((TypedArrayInstance)thisObj).IndexOf(args[0], TypeUtilities.IsUndefined(args[1]) ? 0 : TypeConverter.ToInteger(args[1]));
			}
		}

		private static object __STUB__Join(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'join' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((TypedArrayInstance)thisObj).Join(",");
				default:
					return ((TypedArrayInstance)thisObj).Join(TypeUtilities.IsUndefined(args[0]) ? "," : TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__LastIndexOf(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'lastIndexOf' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((TypedArrayInstance)thisObj).LastIndexOf(Undefined.Value);
				case 1:
					return ((TypedArrayInstance)thisObj).LastIndexOf(args[0]);
				default:
					return ((TypedArrayInstance)thisObj).LastIndexOf(args[0], TypeConverter.ToInteger(args[1]));
			}
		}

		private static object __STUB__Map(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'map' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return ((TypedArrayInstance)thisObj).Map(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), null);
				default:
					return ((TypedArrayInstance)thisObj).Map(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToObject(engine, args[1]));
			}
		}

		private static object __STUB__Reduce(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'reduce' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return ((TypedArrayInstance)thisObj).Reduce(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), null);
				default:
					return ((TypedArrayInstance)thisObj).Reduce(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), TypeUtilities.IsUndefined(args[1]) ? null : args[1]);
			}
		}

		private static object __STUB__ReduceRight(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'reduceRight' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return ((TypedArrayInstance)thisObj).ReduceRight(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), null);
				default:
					return ((TypedArrayInstance)thisObj).ReduceRight(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), TypeUtilities.IsUndefined(args[1]) ? null : args[1]);
			}
		}

		private static object __STUB__Reverse(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'reverse' is not generic.");
			return ((TypedArrayInstance)thisObj).Reverse();
		}

		private static object __STUB__Set(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'set' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					((TypedArrayInstance)thisObj).Set(TypeConverter.ToObject(engine, args[0]), 0); return Undefined.Value;
				default:
					((TypedArrayInstance)thisObj).Set(TypeConverter.ToObject(engine, args[0]), TypeUtilities.IsUndefined(args[1]) ? 0 : TypeConverter.ToInteger(args[1])); return Undefined.Value;
			}
		}

		private static object __STUB__Slice(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'slice' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((TypedArrayInstance)thisObj).Slice(0, int.MaxValue);
				case 1:
					return ((TypedArrayInstance)thisObj).Slice(TypeConverter.ToInteger(args[0]), int.MaxValue);
				default:
					return ((TypedArrayInstance)thisObj).Slice(TypeConverter.ToInteger(args[0]), TypeUtilities.IsUndefined(args[1]) ? int.MaxValue : TypeConverter.ToInteger(args[1]));
			}
		}

		private static object __STUB__Some(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'some' is not generic.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return ((TypedArrayInstance)thisObj).Some(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), null);
				default:
					return ((TypedArrayInstance)thisObj).Some(TypeConverter.ToObject<FunctionInstance>(engine, args[0]), TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToObject(engine, args[1]));
			}
		}

		private static object __STUB__Sort(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'sort' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((TypedArrayInstance)thisObj).Sort(null);
				default:
					return ((TypedArrayInstance)thisObj).Sort(TypeUtilities.IsUndefined(args[0]) ? null : TypeConverter.ToObject<FunctionInstance>(engine, args[0]));
			}
		}

		private static object __STUB__ToLocaleString(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'toLocaleString' is not generic.");
			return ((TypedArrayInstance)thisObj).ToLocaleString();
		}

		private static object __STUB__ToString(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(ErrorType.TypeError, "Cannot convert undefined or null to object.");
			return ToString(TypeConverter.ToObject(engine, thisObj));
		}
	}

}
