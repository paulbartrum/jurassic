/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class RegExpInstance
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(9)
			{
				new PropertyNameAndValue("source", new PropertyDescriptor(new ClrStubFunction(engine, "get source", 0, __GETTER__Source), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("flags", new PropertyDescriptor(new ClrStubFunction(engine, "get flags", 0, __GETTER__Flags), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("global", new PropertyDescriptor(new ClrStubFunction(engine, "get global", 0, __GETTER__Global), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("multiline", new PropertyDescriptor(new ClrStubFunction(engine, "get multiline", 0, __GETTER__Multiline), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("ignoreCase", new PropertyDescriptor(new ClrStubFunction(engine, "get ignoreCase", 0, __GETTER__IgnoreCase), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("compile", new ClrStubFunction(engine, "compile", 2, __STUB__Compile), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("test", new ClrStubFunction(engine, "test", 1, __STUB__Test), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("exec", new ClrStubFunction(engine, "exec", 1, __STUB__Exec), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue(engine.Symbol.Match, new ClrStubFunction(engine, "[Symbol.match]", 1, __STUB__Match), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toString", new ClrStubFunction(engine, "toString", 0, __STUB__ToString), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __GETTER__Source(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RegExpInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'get source' is not generic.");
			return ((RegExpInstance)thisObj).Source;
		}

		private static object __GETTER__Flags(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RegExpInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'get flags' is not generic.");
			return ((RegExpInstance)thisObj).Flags;
		}

		private static object __GETTER__Global(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RegExpInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'get global' is not generic.");
			return ((RegExpInstance)thisObj).Global;
		}

		private static object __GETTER__Multiline(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RegExpInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'get multiline' is not generic.");
			return ((RegExpInstance)thisObj).Multiline;
		}

		private static object __GETTER__IgnoreCase(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RegExpInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'get ignoreCase' is not generic.");
			return ((RegExpInstance)thisObj).IgnoreCase;
		}

		private static object __STUB__Compile(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RegExpInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'compile' is not generic.");
			switch (args.Length)
			{
				case 0:
					((RegExpInstance)thisObj).Compile("undefined", null); return Undefined.Value;
				case 1:
					((RegExpInstance)thisObj).Compile(TypeConverter.ToString(args[0]), null); return Undefined.Value;
				default:
					((RegExpInstance)thisObj).Compile(TypeConverter.ToString(args[0]), TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToString(args[1])); return Undefined.Value;
			}
		}

		private static object __STUB__Test(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RegExpInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'test' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((RegExpInstance)thisObj).Test("undefined");
				default:
					return ((RegExpInstance)thisObj).Test(TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__Exec(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RegExpInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'exec' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((RegExpInstance)thisObj).Exec("undefined");
				default:
					return ((RegExpInstance)thisObj).Exec(TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__Match(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RegExpInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method '[Symbol.match]' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((RegExpInstance)thisObj).Match("undefined");
				default:
					return ((RegExpInstance)thisObj).Match(TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__ToString(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RegExpInstance))
				throw new JavaScriptException(ErrorType.TypeError, "The method 'toString' is not generic.");
			return ((RegExpInstance)thisObj).ToString();
		}
	}

}
