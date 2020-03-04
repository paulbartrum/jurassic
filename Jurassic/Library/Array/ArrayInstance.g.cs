/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class ArrayInstance
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(38)
			{
				new PropertyNameAndValue("concat", new ClrStubFunction(engine, "concat", 1, __STUB__Concat), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("pop", new ClrStubFunction(engine, "pop", 0, __STUB__Pop), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("push", new ClrStubFunction(engine, "push", 1, __STUB__Push), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("shift", new ClrStubFunction(engine, "shift", 0, __STUB__Shift), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("splice", new ClrStubFunction(engine, "splice", 2, __STUB__Splice), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("unshift", new ClrStubFunction(engine, "unshift", 1, __STUB__Unshift), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("flat", new ClrStubFunction(engine, "flat", 0, __STUB__Flat), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("flatMap", new ClrStubFunction(engine, "flatMap", 1, __STUB__FlatMap), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("join", new ClrStubFunction(engine, "join", 1, __STUB__Join), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("reverse", new ClrStubFunction(engine, "reverse", 0, __STUB__Reverse), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("slice", new ClrStubFunction(engine, "slice", 2, __STUB__Slice), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("sort", new ClrStubFunction(engine, "sort", 1, __STUB__Sort), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toLocaleString", new ClrStubFunction(engine, "toLocaleString", 0, __STUB__ToLocaleString), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toString", new ClrStubFunction(engine, "toString", 0, __STUB__ToString), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("indexOf", new ClrStubFunction(engine, "indexOf", 1, __STUB__IndexOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("lastIndexOf", new ClrStubFunction(engine, "lastIndexOf", 1, __STUB__LastIndexOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("every", new ClrStubFunction(engine, "every", 1, __STUB__Every), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("some", new ClrStubFunction(engine, "some", 1, __STUB__Some), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("forEach", new ClrStubFunction(engine, "forEach", 1, __STUB__ForEach), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("map", new ClrStubFunction(engine, "map", 1, __STUB__Map), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("find", new ClrStubFunction(engine, "find", 1, __STUB__Find), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("filter", new ClrStubFunction(engine, "filter", 1, __STUB__Filter), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("reduce", new ClrStubFunction(engine, "reduce", 1, __STUB__Reduce), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("reduceRight", new ClrStubFunction(engine, "reduceRight", 1, __STUB__ReduceRight), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("copyWithin", new ClrStubFunction(engine, "copyWithin", 2, __STUB__CopyWithin), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("fill", new ClrStubFunction(engine, "fill", 1, __STUB__Fill), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("findIndex", new ClrStubFunction(engine, "findIndex", 1, __STUB__FindIndex), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("entries", new ClrStubFunction(engine, "entries", 0, __STUB__Entries), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("keys", new ClrStubFunction(engine, "keys", 0, __STUB__Keys), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("values", new ClrStubFunction(engine, "values", 0, __STUB__Values), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("includes", new ClrStubFunction(engine, "includes", 1, __STUB__Includes), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Concat(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return Concat(TypeConverter.ToObject(engine, thisObj), new object[0]);
				default:
					return Concat(TypeConverter.ToObject(engine, thisObj), args);
			}
		}

		private static object __STUB__Pop(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			return Pop(TypeConverter.ToObject(engine, thisObj));
		}

		private static object __STUB__Push(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return Push(TypeConverter.ToObject(engine, thisObj), new object[0]);
				default:
					return Push(TypeConverter.ToObject(engine, thisObj), args);
			}
		}

		private static object __STUB__Shift(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			return Shift(TypeConverter.ToObject(engine, thisObj));
		}

		private static object __STUB__Splice(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return Splice(TypeConverter.ToObject(engine, thisObj));
				case 1:
					return Splice(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToInteger(args[0]));
				case 2:
					return Splice(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToInteger(args[0]), TypeConverter.ToInteger(args[1]), new object[0]);
				default:
					return Splice(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToInteger(args[0]), TypeConverter.ToInteger(args[1]), TypeUtilities.SliceArray(args, 2));
			}
		}

		private static object __STUB__Unshift(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return Unshift(TypeConverter.ToObject(engine, thisObj), new object[0]);
				default:
					return Unshift(TypeConverter.ToObject(engine, thisObj), args);
			}
		}

		private static object __STUB__Flat(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return Flat(TypeConverter.ToObject(engine, thisObj), 1);
				default:
					return Flat(TypeConverter.ToObject(engine, thisObj), TypeUtilities.IsUndefined(args[0]) ? 1 : TypeConverter.ToInteger(args[0]));
			}
		}

		private static object __STUB__FlatMap(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return FlatMap(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToObject<FunctionInstance>(engine, args[0]), Undefined.Value);
				default:
					return FlatMap(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToObject<FunctionInstance>(engine, args[0]), args[1]);
			}
		}

		private static object __STUB__Join(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return Join(TypeConverter.ToObject(engine, thisObj), ",");
				default:
					return Join(TypeConverter.ToObject(engine, thisObj), TypeUtilities.IsUndefined(args[0]) ? "," : TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__Reverse(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			return Reverse(TypeConverter.ToObject(engine, thisObj));
		}

		private static object __STUB__Slice(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return Slice(TypeConverter.ToObject(engine, thisObj), 0, int.MaxValue);
				case 1:
					return Slice(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToInteger(args[0]), int.MaxValue);
				default:
					return Slice(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToInteger(args[0]), TypeUtilities.IsUndefined(args[1]) ? int.MaxValue : TypeConverter.ToInteger(args[1]));
			}
		}

		private static object __STUB__Sort(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return Sort(TypeConverter.ToObject(engine, thisObj), null);
				default:
					return Sort(TypeConverter.ToObject(engine, thisObj), TypeUtilities.IsUndefined(args[0]) ? null : TypeConverter.ToObject<FunctionInstance>(engine, args[0]));
			}
		}

		private static object __STUB__ToLocaleString(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			return ToLocaleString(TypeConverter.ToObject(engine, thisObj));
		}

		private static object __STUB__ToString(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			return ToString(TypeConverter.ToObject(engine, thisObj));
		}

		private static object __STUB__IndexOf(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return IndexOf(TypeConverter.ToObject(engine, thisObj), Undefined.Value, 0);
				case 1:
					return IndexOf(TypeConverter.ToObject(engine, thisObj), args[0], 0);
				default:
					return IndexOf(TypeConverter.ToObject(engine, thisObj), args[0], TypeUtilities.IsUndefined(args[1]) ? 0 : TypeConverter.ToInteger(args[1]));
			}
		}

		private static object __STUB__LastIndexOf(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return LastIndexOf(TypeConverter.ToObject(engine, thisObj), Undefined.Value);
				case 1:
					return LastIndexOf(TypeConverter.ToObject(engine, thisObj), args[0]);
				default:
					return LastIndexOf(TypeConverter.ToObject(engine, thisObj), args[0], TypeConverter.ToInteger(args[1]));
			}
		}

		private static object __STUB__Every(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return Every(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToObject<FunctionInstance>(engine, args[0]), null);
				default:
					return Every(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToObject<FunctionInstance>(engine, args[0]), TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToObject(engine, args[1]));
			}
		}

		private static object __STUB__Some(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return Some(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToObject<FunctionInstance>(engine, args[0]), null);
				default:
					return Some(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToObject<FunctionInstance>(engine, args[0]), TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToObject(engine, args[1]));
			}
		}

		private static object __STUB__ForEach(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					ForEach(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToObject<FunctionInstance>(engine, args[0]), null); return Undefined.Value;
				default:
					ForEach(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToObject<FunctionInstance>(engine, args[0]), TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToObject(engine, args[1])); return Undefined.Value;
			}
		}

		private static object __STUB__Map(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return Map(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToObject<FunctionInstance>(engine, args[0]), null);
				default:
					return Map(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToObject<FunctionInstance>(engine, args[0]), TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToObject(engine, args[1]));
			}
		}

		private static object __STUB__Find(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return Find(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToObject<FunctionInstance>(engine, args[0]), null);
				default:
					return Find(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToObject<FunctionInstance>(engine, args[0]), TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToObject(engine, args[1]));
			}
		}

		private static object __STUB__Filter(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return Filter(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToObject<FunctionInstance>(engine, args[0]), null);
				default:
					return Filter(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToObject<FunctionInstance>(engine, args[0]), TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToObject(engine, args[1]));
			}
		}

		private static object __STUB__Reduce(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return Reduce(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToObject<FunctionInstance>(engine, args[0]), null);
				default:
					return Reduce(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToObject<FunctionInstance>(engine, args[0]), TypeUtilities.IsUndefined(args[1]) ? null : args[1]);
			}
		}

		private static object __STUB__ReduceRight(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return ReduceRight(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToObject<FunctionInstance>(engine, args[0]), null);
				default:
					return ReduceRight(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToObject<FunctionInstance>(engine, args[0]), TypeUtilities.IsUndefined(args[1]) ? null : args[1]);
			}
		}

		private static object __STUB__CopyWithin(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return CopyWithin(TypeConverter.ToObject(engine, thisObj), 0, 0, int.MaxValue);
				case 1:
					return CopyWithin(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToInteger(args[0]), 0, int.MaxValue);
				case 2:
					return CopyWithin(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToInteger(args[0]), TypeConverter.ToInteger(args[1]), int.MaxValue);
				default:
					return CopyWithin(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToInteger(args[0]), TypeConverter.ToInteger(args[1]), TypeUtilities.IsUndefined(args[2]) ? int.MaxValue : TypeConverter.ToInteger(args[2]));
			}
		}

		private static object __STUB__Fill(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return Fill(TypeConverter.ToObject(engine, thisObj), Undefined.Value, 0, int.MaxValue);
				case 1:
					return Fill(TypeConverter.ToObject(engine, thisObj), args[0], 0, int.MaxValue);
				case 2:
					return Fill(TypeConverter.ToObject(engine, thisObj), args[0], TypeUtilities.IsUndefined(args[1]) ? 0 : TypeConverter.ToInteger(args[1]), int.MaxValue);
				default:
					return Fill(TypeConverter.ToObject(engine, thisObj), args[0], TypeUtilities.IsUndefined(args[1]) ? 0 : TypeConverter.ToInteger(args[1]), TypeUtilities.IsUndefined(args[2]) ? int.MaxValue : TypeConverter.ToInteger(args[2]));
			}
		}

		private static object __STUB__FindIndex(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return FindIndex(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToObject<FunctionInstance>(engine, args[0]), null);
				default:
					return FindIndex(TypeConverter.ToObject(engine, thisObj), TypeConverter.ToObject<FunctionInstance>(engine, args[0]), TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToObject(engine, args[1]));
			}
		}

		private static object __STUB__Entries(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ArrayInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'entries' is not generic.");
			return ((ArrayInstance)thisObj).Entries();
		}

		private static object __STUB__Keys(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ArrayInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'keys' is not generic.");
			return ((ArrayInstance)thisObj).Keys();
		}

		private static object __STUB__Values(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ArrayInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'values' is not generic.");
			return ((ArrayInstance)thisObj).Values();
		}

		private static object __STUB__Includes(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, ErrorType.TypeError, "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return Includes(TypeConverter.ToObject(engine, thisObj), Undefined.Value, 0);
				case 1:
					return Includes(TypeConverter.ToObject(engine, thisObj), args[0], 0);
				default:
					return Includes(TypeConverter.ToObject(engine, thisObj), args[0], TypeUtilities.IsUndefined(args[1]) ? 0 : TypeConverter.ToInteger(args[1]));
			}
		}
	}

}
