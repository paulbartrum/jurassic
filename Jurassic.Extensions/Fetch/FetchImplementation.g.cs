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
				new PropertyNameAndValue("fetch", new ClrStubFunction(engine, "fetch", 2, __STUB__Fetch), PropertyAttributes.Writable | PropertyAttributes.Enumerable | PropertyAttributes.Configurable),
			};
		}

		private static object __STUB__Fetch(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Fetch(engine, Undefined.Value, null);
				case 1:
					return Fetch(engine, args[0], null);
				default:
					return Fetch(engine, args[0], TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToObject(engine, args[1]));
			}
		}
	}

}
