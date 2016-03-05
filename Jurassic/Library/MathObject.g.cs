/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class MathObject
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(48)
			{
				new PropertyNameAndValue("E", E, PropertyAttributes.Sealed),
				new PropertyNameAndValue("LN2", LN2, PropertyAttributes.Sealed),
				new PropertyNameAndValue("LN10", LN10, PropertyAttributes.Sealed),
				new PropertyNameAndValue("LOG2E", LOG2E, PropertyAttributes.Sealed),
				new PropertyNameAndValue("LOG10E", LOG10E, PropertyAttributes.Sealed),
				new PropertyNameAndValue("PI", PI, PropertyAttributes.Sealed),
				new PropertyNameAndValue("SQRT1_2", SQRT1_2, PropertyAttributes.Sealed),
				new PropertyNameAndValue("SQRT2", SQRT2, PropertyAttributes.Sealed),
				new PropertyNameAndValue("abs", new ClrStubFunction(engine.FunctionInstancePrototype, "abs", 1, __STUB__abs), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("acos", new ClrStubFunction(engine.FunctionInstancePrototype, "acos", 1, __STUB__acos), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("asin", new ClrStubFunction(engine.FunctionInstancePrototype, "asin", 1, __STUB__asin), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("atan", new ClrStubFunction(engine.FunctionInstancePrototype, "atan", 1, __STUB__atan), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("atan2", new ClrStubFunction(engine.FunctionInstancePrototype, "atan2", 2, __STUB__atan2), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("ceil", new ClrStubFunction(engine.FunctionInstancePrototype, "ceil", 1, __STUB__ceil), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("cos", new ClrStubFunction(engine.FunctionInstancePrototype, "cos", 1, __STUB__cos), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("exp", new ClrStubFunction(engine.FunctionInstancePrototype, "exp", 1, __STUB__exp), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("floor", new ClrStubFunction(engine.FunctionInstancePrototype, "floor", 1, __STUB__floor), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("log", new ClrStubFunction(engine.FunctionInstancePrototype, "log", 1, __STUB__log), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("max", new ClrStubFunction(engine.FunctionInstancePrototype, "max", 2, __STUB__max), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("min", new ClrStubFunction(engine.FunctionInstancePrototype, "min", 2, __STUB__min), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("pow", new ClrStubFunction(engine.FunctionInstancePrototype, "pow", 2, __STUB__pow), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("random", new ClrStubFunction(engine.FunctionInstancePrototype, "random", 0, __STUB__random), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("round", new ClrStubFunction(engine.FunctionInstancePrototype, "round", 1, __STUB__round), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("sin", new ClrStubFunction(engine.FunctionInstancePrototype, "sin", 1, __STUB__sin), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("sqrt", new ClrStubFunction(engine.FunctionInstancePrototype, "sqrt", 1, __STUB__sqrt), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("tan", new ClrStubFunction(engine.FunctionInstancePrototype, "tan", 1, __STUB__tan), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("log10", new ClrStubFunction(engine.FunctionInstancePrototype, "log10", 1, __STUB__log10), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("log2", new ClrStubFunction(engine.FunctionInstancePrototype, "log2", 1, __STUB__log2), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("log1p", new ClrStubFunction(engine.FunctionInstancePrototype, "log1p", 1, __STUB__log1p), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("expm1", new ClrStubFunction(engine.FunctionInstancePrototype, "expm1", 1, __STUB__expm1), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("cosh", new ClrStubFunction(engine.FunctionInstancePrototype, "cosh", 1, __STUB__cosh), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("sinh", new ClrStubFunction(engine.FunctionInstancePrototype, "sinh", 1, __STUB__sinh), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("tanh", new ClrStubFunction(engine.FunctionInstancePrototype, "tanh", 1, __STUB__tanh), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("acosh", new ClrStubFunction(engine.FunctionInstancePrototype, "acosh", 1, __STUB__acosh), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("asinh", new ClrStubFunction(engine.FunctionInstancePrototype, "asinh", 1, __STUB__asinh), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("atanh", new ClrStubFunction(engine.FunctionInstancePrototype, "atanh", 1, __STUB__atanh), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("hypot", new ClrStubFunction(engine.FunctionInstancePrototype, "hypot", 2, __STUB__hypot), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("trunc", new ClrStubFunction(engine.FunctionInstancePrototype, "trunc", 1, __STUB__trunc), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("sign", new ClrStubFunction(engine.FunctionInstancePrototype, "sign", 1, __STUB__sign), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("imul", new ClrStubFunction(engine.FunctionInstancePrototype, "imul", 2, __STUB__imul), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("fround", new ClrStubFunction(engine.FunctionInstancePrototype, "fround", 1, __STUB__fround), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("clz32", new ClrStubFunction(engine.FunctionInstancePrototype, "clz32", 1, __STUB__clz32), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("cbrt", new ClrStubFunction(engine.FunctionInstancePrototype, "cbrt", 1, __STUB__cbrt), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue(engine.Symbol.ToStringTag, new ClrStubFunction(engine.FunctionInstancePrototype, "[Symbol.toStringTag]", 0, __STUB__toStringTag), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__abs(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Abs(double.NaN);
				default:
					return Abs(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__acos(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Acos(double.NaN);
				default:
					return Acos(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__asin(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Asin(double.NaN);
				default:
					return Asin(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__atan(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Atan(double.NaN);
				default:
					return Atan(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__atan2(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Atan2(double.NaN, double.NaN);
				case 1:
					return Atan2(TypeConverter.ToNumber(args[0]), double.NaN);
				default:
					return Atan2(TypeConverter.ToNumber(args[0]), TypeConverter.ToNumber(args[1]));
			}
		}

		private static object __STUB__ceil(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Ceil(double.NaN);
				default:
					return Ceil(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__cos(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Cos(double.NaN);
				default:
					return Cos(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__exp(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Exp(double.NaN);
				default:
					return Exp(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__floor(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Floor(double.NaN);
				default:
					return Floor(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__log(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Log(double.NaN);
				default:
					return Log(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__max(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Max(new double[0]);
				default:
					return Max(TypeConverter.ConvertParameterArrayTo<double>(engine, args, 0));
			}
		}

		private static object __STUB__min(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Min(new double[0]);
				default:
					return Min(TypeConverter.ConvertParameterArrayTo<double>(engine, args, 0));
			}
		}

		private static object __STUB__pow(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Pow(double.NaN, double.NaN);
				case 1:
					return Pow(TypeConverter.ToNumber(args[0]), double.NaN);
				default:
					return Pow(TypeConverter.ToNumber(args[0]), TypeConverter.ToNumber(args[1]));
			}
		}

		private static object __STUB__random(ScriptEngine engine, object thisObj, object[] args)
		{
			return Random();
		}

		private static object __STUB__round(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Round(double.NaN);
				default:
					return Round(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__sin(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Sin(double.NaN);
				default:
					return Sin(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__sqrt(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Sqrt(double.NaN);
				default:
					return Sqrt(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__tan(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Tan(double.NaN);
				default:
					return Tan(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__log10(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Log10(double.NaN);
				default:
					return Log10(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__log2(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Log2(double.NaN);
				default:
					return Log2(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__log1p(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Log1p(double.NaN);
				default:
					return Log1p(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__expm1(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Expm1(double.NaN);
				default:
					return Expm1(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__cosh(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Cosh(double.NaN);
				default:
					return Cosh(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__sinh(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Sinh(double.NaN);
				default:
					return Sinh(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__tanh(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Tanh(double.NaN);
				default:
					return Tanh(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__acosh(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Acosh(double.NaN);
				default:
					return Acosh(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__asinh(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Asinh(double.NaN);
				default:
					return Asinh(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__atanh(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Atanh(double.NaN);
				default:
					return Atanh(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__hypot(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Hypot(new double[0]);
				default:
					return Hypot(TypeConverter.ConvertParameterArrayTo<double>(engine, args, 0));
			}
		}

		private static object __STUB__trunc(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Trunc(double.NaN);
				default:
					return Trunc(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__sign(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Sign(double.NaN);
				default:
					return Sign(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__imul(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return IMul(double.NaN, double.NaN);
				case 1:
					return IMul(TypeConverter.ToNumber(args[0]), double.NaN);
				default:
					return IMul(TypeConverter.ToNumber(args[0]), TypeConverter.ToNumber(args[1]));
			}
		}

		private static object __STUB__fround(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Fround(double.NaN);
				default:
					return Fround(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__clz32(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Clz32(double.NaN);
				default:
					return Clz32(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__cbrt(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Cbrt(double.NaN);
				default:
					return Cbrt(TypeConverter.ToNumber(args[0]));
			}
		}

		private static object __STUB__toStringTag(ScriptEngine engine, object thisObj, object[] args)
		{
			return ToStringTag();
		}
	}

}
