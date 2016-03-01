/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class RegExpConstructor
	{

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RegExpConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'Call' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((RegExpConstructor)thisObj).Call(Undefined.Value, null);
				case 1:
					return ((RegExpConstructor)thisObj).Call(args[0], null);
				default:
					return ((RegExpConstructor)thisObj).Call(args[0], TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToString(args[1]));
			}
		}

		private static ObjectInstance __STUB__Construct(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RegExpConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'Construct' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((RegExpConstructor)thisObj).Construct(Undefined.Value, null);
				case 1:
					return ((RegExpConstructor)thisObj).Construct(args[0], null);
				default:
					return ((RegExpConstructor)thisObj).Construct(args[0], TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToString(args[1]));
			}
		}
	}

}
