/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class TypedArrayConstructor
	{
		private List<PropertyNameAndValue> GetDeclarativeProperties()
		{
			return new List<PropertyNameAndValue>(6)
			{
				new PropertyNameAndValue("from", new ClrStubFunction(Engine.FunctionInstancePrototype, "from", 3, __STUB__from), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("of", new ClrStubFunction(Engine.FunctionInstancePrototype, "of", 1, __STUB__of), PropertyAttributes.NonEnumerable),
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

		private static object __STUB__from(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 2:
					return From(args[0], TypeConverter.ToFunction(engine, args[1]), Undefined.Value);
				default:
					return From(args[0], TypeConverter.ToFunction(engine, args[1]), args[2]);
			}
		}

		private static object __STUB__of(ScriptEngine engine, object thisObj, object[] args)
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
