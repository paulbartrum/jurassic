/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class ErrorConstructor
	{

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ErrorConstructor))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'Call' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((ErrorConstructor)thisObj).Call("");
				default:
					return ((ErrorConstructor)thisObj).Call(TypeUtilities.IsUndefined(args[0]) ? "" : TypeConverter.ToString(args[0]));
			}
		}

		private static ObjectInstance __STUB__Construct(ScriptEngine engine, FunctionInstance thisObj, FunctionInstance newTarget, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return ((ErrorConstructor)thisObj).Construct("");
				default:
					return ((ErrorConstructor)thisObj).Construct(TypeUtilities.IsUndefined(args[0]) ? "" : TypeConverter.ToString(args[0]));
			}
		}
	}

}
