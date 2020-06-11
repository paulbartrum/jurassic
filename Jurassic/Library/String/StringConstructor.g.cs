/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class StringConstructor
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(7)
			{
				new PropertyNameAndValue("fromCharCode", new ClrStubFunction(engine, "fromCharCode", 1, __STUB__FromCharCode), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("fromCodePoint", new ClrStubFunction(engine, "fromCodePoint", 1, __STUB__FromCodePoint), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("raw", new ClrStubFunction(engine, "raw", 2, __STUB__Raw), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is StringConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'Call' is not generic.");
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
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'Construct' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((StringConstructor)thisObj).Construct();
				default:
					return ((StringConstructor)thisObj).Construct(TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__FromCharCode(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return FromCharCode(new double[0]);
				default:
					return FromCharCode(TypeConverter.ConvertParameterArrayTo<double>(engine, args, 0));
			}
		}

		private static object __STUB__FromCodePoint(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return FromCodePoint(engine, new double[0]);
				default:
					return FromCodePoint(engine, TypeConverter.ConvertParameterArrayTo<double>(engine, args, 0));
			}
		}

		private static object __STUB__Raw(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					throw new JavaScriptException(engine, ErrorType.TypeError, "undefined cannot be converted to an object");
				case 1:
					return Raw(engine, TypeConverter.ToObject(engine, args[0]), new object[0]);
				default:
					return Raw(engine, TypeConverter.ToObject(engine, args[0]), TypeUtilities.SliceArray(args, 1));
			}
		}
	}

}
