/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;
using Jurassic.Library;

namespace Jurassic.Extensions.SimpleTiming
{

	public static partial class SimpleTimingImplementation
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(8)
			{
				new PropertyNameAndValue("setTimeout", new ClrStubFunction(engine, "setTimeout", 3, __STUB__SetTimeout), PropertyAttributes.Writable | PropertyAttributes.Enumerable | PropertyAttributes.Configurable),
				new PropertyNameAndValue("clearTimeout", new ClrStubFunction(engine, "clearTimeout", 1, __STUB__ClearTimeout), PropertyAttributes.Writable | PropertyAttributes.Enumerable | PropertyAttributes.Configurable),
				new PropertyNameAndValue("setInterval", new ClrStubFunction(engine, "setInterval", 3, __STUB__SetInterval), PropertyAttributes.Writable | PropertyAttributes.Enumerable | PropertyAttributes.Configurable),
				new PropertyNameAndValue("clearInterval", new ClrStubFunction(engine, "clearInterval", 1, __STUB__ClearInterval), PropertyAttributes.Writable | PropertyAttributes.Enumerable | PropertyAttributes.Configurable),
			};
		}

		private static object __STUB__SetTimeout(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return SetTimeout(engine, Undefined.Value, 0, new object[0]);
				case 1:
					return SetTimeout(engine, args[0], 0, new object[0]);
				case 2:
					return SetTimeout(engine, args[0], TypeUtilities.IsUndefined(args[1]) ? 0 : TypeConverter.ToInteger(args[1]), new object[0]);
				default:
					return SetTimeout(engine, args[0], TypeUtilities.IsUndefined(args[1]) ? 0 : TypeConverter.ToInteger(args[1]), TypeUtilities.SliceArray(args, 2));
			}
		}

		private static object __STUB__ClearTimeout(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					ClearTimeout(engine, 0); return Undefined.Value;
				default:
					ClearTimeout(engine, TypeConverter.ToInteger(args[0])); return Undefined.Value;
			}
		}

		private static object __STUB__SetInterval(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return SetInterval(engine, Undefined.Value, 0, new object[0]);
				case 1:
					return SetInterval(engine, args[0], 0, new object[0]);
				case 2:
					return SetInterval(engine, args[0], TypeUtilities.IsUndefined(args[1]) ? 0 : TypeConverter.ToInteger(args[1]), new object[0]);
				default:
					return SetInterval(engine, args[0], TypeUtilities.IsUndefined(args[1]) ? 0 : TypeConverter.ToInteger(args[1]), TypeUtilities.SliceArray(args, 2));
			}
		}

		private static object __STUB__ClearInterval(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					ClearInterval(engine, 0); return Undefined.Value;
				default:
					ClearInterval(engine, TypeConverter.ToInteger(args[0])); return Undefined.Value;
			}
		}
	}

}
