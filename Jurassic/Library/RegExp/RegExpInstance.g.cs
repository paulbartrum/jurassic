/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class RegExpInstance
	{
		private List<PropertyNameAndValue> GetDeclarativeProperties()
		{
			return new List<PropertyNameAndValue>(8)
			{
				new PropertyNameAndValue("source", new PropertyDescriptor(new ClrStubFunction(Engine.FunctionInstancePrototype, "get source", 0, __GETTER__source), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("flags", new PropertyDescriptor(new ClrStubFunction(Engine.FunctionInstancePrototype, "get flags", 0, __GETTER__flags), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("global", new PropertyDescriptor(new ClrStubFunction(Engine.FunctionInstancePrototype, "get global", 0, __GETTER__global), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("multiline", new PropertyDescriptor(new ClrStubFunction(Engine.FunctionInstancePrototype, "get multiline", 0, __GETTER__multiline), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("ignoreCase", new PropertyDescriptor(new ClrStubFunction(Engine.FunctionInstancePrototype, "get ignoreCase", 0, __GETTER__ignoreCase), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("compile", new ClrStubFunction(Engine.FunctionInstancePrototype, "compile", 2, __STUB__compile), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("test", new ClrStubFunction(Engine.FunctionInstancePrototype, "test", 1, __STUB__test), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("exec", new ClrStubFunction(Engine.FunctionInstancePrototype, "exec", 1, __STUB__exec), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toString", new ClrStubFunction(Engine.FunctionInstancePrototype, "toString", 0, __STUB__toString), PropertyAttributes.NonEnumerable),
			};
		}

		object __GETTER__source(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RegExpInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get source' is not generic.");
			return ((RegExpInstance)thisObj).Source;
		}

		object __GETTER__flags(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RegExpInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get flags' is not generic.");
			return ((RegExpInstance)thisObj).Flags;
		}

		object __GETTER__global(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RegExpInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get global' is not generic.");
			return ((RegExpInstance)thisObj).Global;
		}

		object __GETTER__multiline(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RegExpInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get multiline' is not generic.");
			return ((RegExpInstance)thisObj).Multiline;
		}

		object __GETTER__ignoreCase(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RegExpInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get ignoreCase' is not generic.");
			return ((RegExpInstance)thisObj).IgnoreCase;
		}

		private static object __STUB__compile(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RegExpInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'compile' is not generic.");
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

		private static object __STUB__test(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RegExpInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'test' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((RegExpInstance)thisObj).Test("undefined");
				default:
					return ((RegExpInstance)thisObj).Test(TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__exec(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RegExpInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'exec' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((RegExpInstance)thisObj).Exec("undefined");
				default:
					return ((RegExpInstance)thisObj).Exec(TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__toString(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RegExpInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'toString' is not generic.");
			return ((RegExpInstance)thisObj).ToString();
		}
	}

}
