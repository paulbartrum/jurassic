/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class JSONObject
	{
		internal new List<PropertyNameAndValue> GetDeclarativeProperties()
		{
			return new List<PropertyNameAndValue>(6)
			{
				new PropertyNameAndValue("parse", new ClrStubFunction(this.Engine.Function.InstancePrototype, "parse", 2, __STUB__parse), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("stringify", new ClrStubFunction(this.Engine.Function.InstancePrototype, "stringify", 3, __STUB__stringify), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__parse(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Parse(engine, "undefined", null);
				case 1:
					return Parse(engine, TypeConverter.ToString(args[0]), null);
				default:
					return Parse(engine, TypeConverter.ToString(args[0]), args[1]);
			}
		}

		private static object __STUB__stringify(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Stringify(engine, Undefined.Value, null, null);
				case 1:
					return Stringify(engine, args[0], null, null);
				case 2:
					return Stringify(engine, args[0], args[1], null);
				default:
					return Stringify(engine, args[0], args[1], args[2]);
			}
		}
	}

}
