/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class MathObject
	{
		private List<PropertyNameAndValue> GetDeclarativeProperties()
		{
			return new List<PropertyNameAndValue>(30)
			{
				new PropertyNameAndValue("E", E, PropertyAttributes.Sealed),
				new PropertyNameAndValue("LN2", LN2, PropertyAttributes.Sealed),
				new PropertyNameAndValue("LN10", LN10, PropertyAttributes.Sealed),
				new PropertyNameAndValue("LOG2E", LOG2E, PropertyAttributes.Sealed),
				new PropertyNameAndValue("LOG10E", LOG10E, PropertyAttributes.Sealed),
				new PropertyNameAndValue("PI", PI, PropertyAttributes.Sealed),
				new PropertyNameAndValue("SQRT1_2", SQRT1_2, PropertyAttributes.Sealed),
				new PropertyNameAndValue("SQRT2", SQRT2, PropertyAttributes.Sealed),
				new PropertyNameAndValue("abs", new ClrStubFunction(Engine.FunctionInstancePrototype, "abs", 1, __STUB__abs), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("acos", new ClrStubFunction(Engine.FunctionInstancePrototype, "acos", 1, __STUB__acos), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("asin", new ClrStubFunction(Engine.FunctionInstancePrototype, "asin", 1, __STUB__asin), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("atan", new ClrStubFunction(Engine.FunctionInstancePrototype, "atan", 1, __STUB__atan), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("atan2", new ClrStubFunction(Engine.FunctionInstancePrototype, "atan2", 2, __STUB__atan2), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("ceil", new ClrStubFunction(Engine.FunctionInstancePrototype, "ceil", 1, __STUB__ceil), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("cos", new ClrStubFunction(Engine.FunctionInstancePrototype, "cos", 1, __STUB__cos), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("exp", new ClrStubFunction(Engine.FunctionInstancePrototype, "exp", 1, __STUB__exp), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("floor", new ClrStubFunction(Engine.FunctionInstancePrototype, "floor", 1, __STUB__floor), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("log", new ClrStubFunction(Engine.FunctionInstancePrototype, "log", 1, __STUB__log), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("max", new ClrStubFunction(Engine.FunctionInstancePrototype, "max", 2, __STUB__max), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("min", new ClrStubFunction(Engine.FunctionInstancePrototype, "min", 2, __STUB__min), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("pow", new ClrStubFunction(Engine.FunctionInstancePrototype, "pow", 2, __STUB__pow), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("random", new ClrStubFunction(Engine.FunctionInstancePrototype, "random", 0, __STUB__random), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("round", new ClrStubFunction(Engine.FunctionInstancePrototype, "round", 1, __STUB__round), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("sin", new ClrStubFunction(Engine.FunctionInstancePrototype, "sin", 1, __STUB__sin), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("sqrt", new ClrStubFunction(Engine.FunctionInstancePrototype, "sqrt", 1, __STUB__sqrt), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("tan", new ClrStubFunction(Engine.FunctionInstancePrototype, "tan", 1, __STUB__tan), PropertyAttributes.NonEnumerable),
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
	}

}
