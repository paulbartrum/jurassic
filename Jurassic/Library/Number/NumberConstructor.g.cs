/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class NumberConstructor
	{
		private List<PropertyNameAndValue> GetDeclarativeProperties()
		{
			return new List<PropertyNameAndValue>(9)
			{
				new PropertyNameAndValue("MAX_VALUE", MAX_VALUE, PropertyAttributes.Sealed),
				new PropertyNameAndValue("MIN_VALUE", MIN_VALUE, PropertyAttributes.Sealed),
				new PropertyNameAndValue("NaN", NaN, PropertyAttributes.Sealed),
				new PropertyNameAndValue("NEGATIVE_INFINITY", NEGATIVE_INFINITY, PropertyAttributes.Sealed),
				new PropertyNameAndValue("POSITIVE_INFINITY", POSITIVE_INFINITY, PropertyAttributes.Sealed),
			};
		}

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is NumberConstructor))
				throw new JavaScriptException(engine, "TypeError", "The method 'Call' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((NumberConstructor)thisObj).Call();
				default:
					return ((NumberConstructor)thisObj).Call(TypeConverter.ToNumber(args[0]));
			}
		}

		private static ObjectInstance __STUB__Construct(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is NumberConstructor))
				throw new JavaScriptException(engine, "TypeError", "The method 'Construct' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((NumberConstructor)thisObj).Construct();
				default:
					return ((NumberConstructor)thisObj).Construct(TypeConverter.ToNumber(args[0]));
			}
		}
	}

}
