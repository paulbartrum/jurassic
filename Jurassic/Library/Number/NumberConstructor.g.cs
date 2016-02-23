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
			return new List<PropertyNameAndValue>(18)
			{
				new PropertyNameAndValue("MAX_VALUE", MAX_VALUE, PropertyAttributes.Sealed),
				new PropertyNameAndValue("MIN_VALUE", MIN_VALUE, PropertyAttributes.Sealed),
				new PropertyNameAndValue("NaN", NaN, PropertyAttributes.Sealed),
				new PropertyNameAndValue("NEGATIVE_INFINITY", NEGATIVE_INFINITY, PropertyAttributes.Sealed),
				new PropertyNameAndValue("POSITIVE_INFINITY", POSITIVE_INFINITY, PropertyAttributes.Sealed),
				new PropertyNameAndValue("EPSILON", EPSILON, PropertyAttributes.Sealed),
				new PropertyNameAndValue("MAX_SAFE_INTEGER", MAX_SAFE_INTEGER, PropertyAttributes.Sealed),
				new PropertyNameAndValue("MIN_SAFE_INTEGER", MIN_SAFE_INTEGER, PropertyAttributes.Sealed),
				new PropertyNameAndValue("isFinite", new ClrStubFunction(Engine.FunctionInstancePrototype, "isFinite", 1, __STUB__isFinite), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("isNaN", new ClrStubFunction(Engine.FunctionInstancePrototype, "isNaN", 1, __STUB__isNaN), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("isInteger", new ClrStubFunction(Engine.FunctionInstancePrototype, "isInteger", 1, __STUB__isInteger), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("isSafeInteger", new ClrStubFunction(Engine.FunctionInstancePrototype, "isSafeInteger", 1, __STUB__isSafeInteger), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("parseInt", new ClrStubFunction(Engine.FunctionInstancePrototype, "parseInt", 2, __STUB__parseInt), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("parseFloat", new ClrStubFunction(Engine.FunctionInstancePrototype, "parseFloat", 1, __STUB__parseFloat), PropertyAttributes.NonEnumerable),
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

		private static object __STUB__isFinite(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return IsFinite(Undefined.Value);
				default:
					return IsFinite(args[0]);
			}
		}

		private static object __STUB__isNaN(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return IsNaN(Undefined.Value);
				default:
					return IsNaN(args[0]);
			}
		}

		private static object __STUB__isInteger(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return IsInteger(Undefined.Value);
				default:
					return IsInteger(args[0]);
			}
		}

		private static object __STUB__isSafeInteger(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return IsSafeInteger(Undefined.Value);
				default:
					return IsSafeInteger(args[0]);
			}
		}

		private static object __STUB__parseInt(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return ParseInt(engine, "undefined", 0.0);
				case 1:
					return ParseInt(engine, TypeConverter.ToString(args[0]), 0.0);
				default:
					return ParseInt(engine, TypeConverter.ToString(args[0]), TypeConverter.ToNumber(args[1], 0.0));
			}
		}

		private static object __STUB__parseFloat(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return ParseFloat("undefined");
				default:
					return ParseFloat(TypeConverter.ToString(args[0]));
			}
		}
	}

}
