/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class FunctionConstructor
	{

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is FunctionConstructor))
				throw new JavaScriptException(engine, "TypeError", "The method 'Call' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((FunctionConstructor)thisObj).Call(new string[0]);
				default:
					return ((FunctionConstructor)thisObj).Call(TypeConverter.ConvertParameterArrayTo<string>(engine, args, 0));
			}
		}

		private static ObjectInstance __STUB__Construct(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is FunctionConstructor))
				throw new JavaScriptException(engine, "TypeError", "The method 'Construct' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((FunctionConstructor)thisObj).Construct(new string[0]);
				default:
					return ((FunctionConstructor)thisObj).Construct(TypeConverter.ConvertParameterArrayTo<string>(engine, args, 0));
			}
		}
	}

}
