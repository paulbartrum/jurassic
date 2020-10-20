/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class ProxyConstructor
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(5)
			{
				new PropertyNameAndValue("revocable", new ClrStubFunction(engine, "revocable", 2, __STUB__Revocable), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ProxyConstructor))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'Call' is not generic.");
			return ((ProxyConstructor)thisObj).Call();
		}

		private static ObjectInstance __STUB__Construct(ScriptEngine engine, FunctionInstance thisObj, FunctionInstance newTarget, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return ((ProxyConstructor)thisObj).Construct(Undefined.Value, Undefined.Value);
				case 1:
					return ((ProxyConstructor)thisObj).Construct(args[0], Undefined.Value);
				default:
					return ((ProxyConstructor)thisObj).Construct(args[0], args[1]);
			}
		}

		private static object __STUB__Revocable(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Revocable(Undefined.Value, Undefined.Value);
				case 1:
					return Revocable(args[0], Undefined.Value);
				default:
					return Revocable(args[0], args[1]);
			}
		}
	}

}
