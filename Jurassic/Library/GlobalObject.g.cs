/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class GlobalObject
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(15)
			{
				new PropertyNameAndValue("decodeURI", new ClrStubFunction(engine.FunctionInstancePrototype, "decodeURI", 1, __STUB__DecodeURI), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("decodeURIComponent", new ClrStubFunction(engine.FunctionInstancePrototype, "decodeURIComponent", 1, __STUB__DecodeURIComponent), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("encodeURI", new ClrStubFunction(engine.FunctionInstancePrototype, "encodeURI", 1, __STUB__EncodeURI), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("encodeURIComponent", new ClrStubFunction(engine.FunctionInstancePrototype, "encodeURIComponent", 1, __STUB__EncodeURIComponent), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("escape", new ClrStubFunction(engine.FunctionInstancePrototype, "escape", 1, __STUB__Escape), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("eval", new ClrStubFunction(engine.FunctionInstancePrototype, "eval", 1, __STUB__Eval), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("isFinite", new ClrStubFunction(engine.FunctionInstancePrototype, "isFinite", 1, __STUB__IsFinite), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("isNaN", new ClrStubFunction(engine.FunctionInstancePrototype, "isNaN", 1, __STUB__IsNaN), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("parseFloat", new ClrStubFunction(engine.FunctionInstancePrototype, "parseFloat", 1, __STUB__ParseFloat), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("parseInt", new ClrStubFunction(engine.FunctionInstancePrototype, "parseInt", 2, __STUB__ParseInt), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("unescape", new ClrStubFunction(engine.FunctionInstancePrototype, "unescape", 1, __STUB__Unescape), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__DecodeURI(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return DecodeURI(engine, "undefined");
				default:
					return DecodeURI(engine, TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__DecodeURIComponent(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return DecodeURIComponent(engine, "undefined");
				default:
					return DecodeURIComponent(engine, TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__EncodeURI(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return EncodeURI(engine, "undefined");
				default:
					return EncodeURI(engine, TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__EncodeURIComponent(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return EncodeURIComponent(engine, "undefined");
				default:
					return EncodeURIComponent(engine, TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__Escape(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Escape("undefined");
				default:
					return Escape(TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__Eval(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Eval(engine, Undefined.Value);
				default:
					return Eval(engine, args[0]);
			}
		}

		private static object __STUB__IsFinite(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return IsFinite(double.NaN);
				default:
					return IsFinite(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__IsNaN(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return IsNaN(double.NaN);
				default:
					return IsNaN(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__ParseFloat(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return ParseFloat("undefined");
				default:
					return ParseFloat(TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__ParseInt(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return ParseInt(engine, "undefined", 0.0);
				case 1:
					return ParseInt(engine, TypeConverter.ToString(args[0]), 0.0);
				default:
					return ParseInt(engine, TypeConverter.ToString(args[0]), TypeUtilities.IsUndefined(args[1]) ? 0.0 : TypeConverter.ToNumber(args[1]));
			}
		}

		private static object __STUB__Unescape(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Unescape("undefined");
				default:
					return Unescape(TypeConverter.ToString(args[0]));
			}
		}
	}

}
