/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class TypedArrayInstance
	{
		private List<PropertyNameAndValue> GetDeclarativeProperties()
		{
			return new List<PropertyNameAndValue>(5)
			{
				new PropertyNameAndValue("fill", new ClrStubFunction(Engine.FunctionInstancePrototype, "fill", 3, __STUB__fill), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__fill(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Fill(Undefined.Value, 0, -1);
				case 1:
					return Fill(args[0], 0, -1);
				case 2:
					return Fill(args[0], TypeUtilities.IsUndefined(args[1]) ? 0 : TypeConverter.ToInteger(args[1]), -1);
				default:
					return Fill(args[0], TypeUtilities.IsUndefined(args[1]) ? 0 : TypeConverter.ToInteger(args[1]), TypeUtilities.IsUndefined(args[2]) ? -1 : TypeConverter.ToInteger(args[2]));
			}
		}
	}

}
