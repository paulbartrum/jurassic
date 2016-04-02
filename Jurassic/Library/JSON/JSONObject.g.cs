/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class JSONObject
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(6)
			{
				new PropertyNameAndValue("parse", new ClrStubFunction(engine.FunctionInstancePrototype, "parse", 2, __STUB__Parse), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("stringify", new ClrStubFunction(engine.FunctionInstancePrototype, "stringify", 3, __STUB__Stringify), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Parse(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Parse(engine, "undefined", null);
				case 1:
					return Parse(engine, TypeConverter.ToString(args[0]), null);
				default:
					return Parse(engine, TypeConverter.ToString(args[0]), TypeUtilities.IsUndefined(args[1]) ? null : args[1]);
			}
		}

		private static object __STUB__Stringify(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Stringify(engine, Undefined.Value, null, null);
				case 1:
					return Stringify(engine, args[0], null, null);
				case 2:
					return Stringify(engine, args[0], TypeUtilities.IsUndefined(args[1]) ? null : args[1], null);
				default:
					return Stringify(engine, args[0], TypeUtilities.IsUndefined(args[1]) ? null : args[1], TypeUtilities.IsUndefined(args[2]) ? null : args[2]);
			}
		}
	}

}
