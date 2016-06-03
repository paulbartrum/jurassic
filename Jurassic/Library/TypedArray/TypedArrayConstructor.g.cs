/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class TypedArrayConstructor
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(6)
			{
				new PropertyNameAndValue("from", new ClrStubFunction(engine.FunctionInstancePrototype, "from", 1, __STUB__From), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("of", new ClrStubFunction(engine.FunctionInstancePrototype, "of", 0, __STUB__Of), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'Call' is not generic.");
			return ((TypedArrayConstructor)thisObj).Call();
		}

		private static ObjectInstance __STUB__Construct(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'Construct' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((TypedArrayConstructor)thisObj).Construct();
				case 1:
					return ((TypedArrayConstructor)thisObj).Construct(args[0], 0, null);
				case 2:
					return ((TypedArrayConstructor)thisObj).Construct(args[0], TypeUtilities.IsUndefined(args[1]) ? 0 : TypeConverter.ToInteger(args[1]), null);
				default:
					return ((TypedArrayConstructor)thisObj).Construct(args[0], TypeUtilities.IsUndefined(args[1]) ? 0 : TypeConverter.ToInteger(args[1]), TypeUtilities.IsUndefined(args[2]) ? (int?)null : TypeConverter.ToInteger(args[2]));
			}
		}

		private static object __STUB__From(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'from' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((TypedArrayConstructor)thisObj).From(Undefined.Value, null, null);
				case 1:
					return ((TypedArrayConstructor)thisObj).From(args[0], null, null);
				case 2:
					return ((TypedArrayConstructor)thisObj).From(args[0], TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToObject<FunctionInstance>(engine, args[1]), null);
				default:
					return ((TypedArrayConstructor)thisObj).From(args[0], TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToObject<FunctionInstance>(engine, args[1]), TypeUtilities.IsUndefined(args[2]) ? null : args[2]);
			}
		}

		private static object __STUB__Of(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is TypedArrayConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'of' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((TypedArrayConstructor)thisObj).Of(new object[0]);
				default:
					return ((TypedArrayConstructor)thisObj).Of(args);
			}
		}
	}

}
