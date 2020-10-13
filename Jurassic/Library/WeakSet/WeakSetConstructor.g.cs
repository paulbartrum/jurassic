/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class WeakSetConstructor
	{

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is WeakSetConstructor))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'Call' is not generic.");
			return ((WeakSetConstructor)thisObj).Call();
		}

		private static ObjectInstance __STUB__Construct(ScriptEngine engine, FunctionInstance thisObj, FunctionInstance newTarget, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return ((WeakSetConstructor)thisObj).Construct(Undefined.Value);
				default:
					return ((WeakSetConstructor)thisObj).Construct(args[0]);
			}
		}
	}

}
