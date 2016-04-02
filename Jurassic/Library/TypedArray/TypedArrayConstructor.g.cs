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
				new PropertyNameAndValue("from", new ClrStubFunction(engine.FunctionInstancePrototype, "from", 3, __STUB__From), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("of", new ClrStubFunction(engine.FunctionInstancePrototype, "of", 1, __STUB__Of), PropertyAttributes.NonEnumerable),
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
					return ((TypedArrayConstructor)thisObj).Construct(Undefined.Value);
				default:
					return ((TypedArrayConstructor)thisObj).Construct(args[0]);
			}
		}

		private static object __STUB__From(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return From(Undefined.Value, null, null);
				case 1:
					return From(args[0], null, null);
				case 2:
					return From(args[0], TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToObject<FunctionInstance>(engine, args[1]), null);
				default:
					return From(args[0], TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToObject<FunctionInstance>(engine, args[1]), TypeUtilities.IsUndefined(args[2]) ? null : args[2]);
			}
		}

		private static object __STUB__Of(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Of(new object[0]);
				default:
					return Of(args);
			}
		}
	}

}
