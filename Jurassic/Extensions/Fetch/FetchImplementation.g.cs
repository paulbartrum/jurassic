/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;
using Jurassic.Library;

namespace Jurassic.Extensions.Fetch
{

	public static partial class FetchImplementation
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(5)
			{
				new PropertyNameAndValue("fetch", new ClrStubFunction(engine.FunctionInstancePrototype, "fetch", 2, __STUB__Fetch), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Fetch(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				default:
					return Fetch(engine, args[0], TypeConverter.ToObject(engine, args[1]));
			}
		}
	}

}
