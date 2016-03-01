/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class BooleanConstructor
	{

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is BooleanConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'Call' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((BooleanConstructor)thisObj).Call(false);
				default:
					return ((BooleanConstructor)thisObj).Call(TypeConverter.ToBoolean(args[0]));
			}
		}

		private static ObjectInstance __STUB__Construct(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is BooleanConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'Construct' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((BooleanConstructor)thisObj).Construct(false);
				default:
					return ((BooleanConstructor)thisObj).Construct(TypeUtilities.IsUndefined(args[0]) ? false : TypeConverter.ToBoolean(args[0]));
			}
		}
	}

}
