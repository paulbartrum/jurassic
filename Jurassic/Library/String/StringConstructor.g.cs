/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class StringConstructor
	{
		internal new List<PropertyNameAndValue> GetDeclarativeProperties()
		{
			return new List<PropertyNameAndValue>(5)
			{
				new PropertyNameAndValue("fromCharCode", new ClrStubFunction(this.Engine.Function.InstancePrototype, "fromCharCode", 1, __STUB__fromCharCode), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is StringConstructor))
				throw new JavaScriptException(engine, "TypeError", "The method 'Call' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((StringConstructor)thisObj).Call();
				default:
					return ((StringConstructor)thisObj).Call(TypeConverter.ToString(args[0]));
			}
		}

		private static ObjectInstance __STUB__Construct(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is StringConstructor))
				throw new JavaScriptException(engine, "TypeError", "The method 'Construct' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((StringConstructor)thisObj).Construct();
				default:
					return ((StringConstructor)thisObj).Construct(TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__fromCharCode(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return FromCharCode(new double[0]);
				default:
					return FromCharCode(TypeConverter.ConvertParameterArrayTo<double>(engine, args, 0));
			}
		}
	}

}
