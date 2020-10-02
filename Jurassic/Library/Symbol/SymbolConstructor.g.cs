/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class SymbolConstructor
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(6)
			{
				new PropertyNameAndValue("for", new ClrStubFunction(engine, "for", 1, __STUB__For), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("keyFor", new ClrStubFunction(engine, "keyFor", 1, __STUB__KeyFor), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is SymbolConstructor))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'Call' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((SymbolConstructor)thisObj).Call(null);
				default:
					return ((SymbolConstructor)thisObj).Call(TypeUtilities.IsUndefined(args[0]) ? null : TypeConverter.ToString(args[0]));
			}
		}

		private static ObjectInstance __STUB__Construct(ScriptEngine engine, FunctionInstance thisObj, FunctionInstance newTarget, object[] args)
		{
			return ((SymbolConstructor)thisObj).Construct();
		}

		private static object __STUB__For(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return For("undefined");
				default:
					return For(TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__KeyFor(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(ErrorType.TypeError, "undefined is not a symbol.");
				default:
					return KeyFor(args[0] as Symbol);
			}
		}
	}

}
