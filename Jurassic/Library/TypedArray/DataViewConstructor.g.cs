/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class DataViewConstructor
	{

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewConstructor))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'Call' is not generic.");
			return ((DataViewConstructor)thisObj).Call();
		}

		private static ObjectInstance __STUB__Construct(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is DataViewConstructor))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'Construct' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((DataViewConstructor)thisObj).Construct(null, 0, null);
				case 1:
					return ((DataViewConstructor)thisObj).Construct(TypeUtilities.IsUndefined(args[0]) ? null : TypeConverter.ToObject<ArrayBufferInstance>(engine, args[0]), 0, null);
				case 2:
					return ((DataViewConstructor)thisObj).Construct(TypeUtilities.IsUndefined(args[0]) ? null : TypeConverter.ToObject<ArrayBufferInstance>(engine, args[0]), TypeUtilities.IsUndefined(args[1]) ? 0 : TypeConverter.ToInteger(args[1]), null);
				default:
					return ((DataViewConstructor)thisObj).Construct(TypeUtilities.IsUndefined(args[0]) ? null : TypeConverter.ToObject<ArrayBufferInstance>(engine, args[0]), TypeUtilities.IsUndefined(args[1]) ? 0 : TypeConverter.ToInteger(args[1]), TypeUtilities.IsUndefined(args[2]) ? (int?)null : TypeConverter.ToInteger(args[2]));
			}
		}
	}

}
