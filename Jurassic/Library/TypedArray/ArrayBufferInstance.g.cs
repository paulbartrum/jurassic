/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class ArrayBufferInstance
	{
		private List<PropertyNameAndValue> GetDeclarativeProperties()
		{
			return new List<PropertyNameAndValue>(5)
			{
				new PropertyNameAndValue("slice", new ClrStubFunction(Engine.FunctionInstancePrototype, "slice", 2, __STUB__slice), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__slice(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Slice(0, 0);
				case 1:
					return Slice(TypeConverter.ToInteger(args[0]), 0);
				default:
					return Slice(TypeConverter.ToInteger(args[0]), TypeConverter.ToInteger(args[1]));
			}
		}
	}

}
