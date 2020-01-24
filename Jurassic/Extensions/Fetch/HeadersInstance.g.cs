/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;
using Jurassic.Library;

namespace Jurassic.Extensions.Fetch
{

	public partial class HeadersInstance
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(12)
			{
				new PropertyNameAndValue("append", new ClrStubFunction(engine.FunctionInstancePrototype, "append", 2, __STUB__Append), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("delete", new ClrStubFunction(engine.FunctionInstancePrototype, "delete", 1, __STUB__Delete), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("entries", new ClrStubFunction(engine.FunctionInstancePrototype, "entries", 0, __STUB__Entries), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("get", new ClrStubFunction(engine.FunctionInstancePrototype, "get", 1, __STUB__Get), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("has", new ClrStubFunction(engine.FunctionInstancePrototype, "has", 1, __STUB__Has), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("keys", new ClrStubFunction(engine.FunctionInstancePrototype, "keys", 0, __STUB__Keys), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("set", new ClrStubFunction(engine.FunctionInstancePrototype, "set", 2, __STUB__Set), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("values", new ClrStubFunction(engine.FunctionInstancePrototype, "values", 0, __STUB__Values), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Append(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is HeadersInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'append' is not generic.");
			switch (args.Length)
			{
				case 0:
					((HeadersInstance)thisObj).Append("undefined", "undefined"); return Undefined.Value;
				case 1:
					((HeadersInstance)thisObj).Append(TypeConverter.ToString(args[0]), "undefined"); return Undefined.Value;
				default:
					((HeadersInstance)thisObj).Append(TypeConverter.ToString(args[0]), TypeConverter.ToString(args[1])); return Undefined.Value;
			}
		}

		private static object __STUB__Delete(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is HeadersInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'delete' is not generic.");
			switch (args.Length)
			{
				case 0:
					((HeadersInstance)thisObj).Delete("undefined"); return Undefined.Value;
				default:
					((HeadersInstance)thisObj).Delete(TypeConverter.ToString(args[0])); return Undefined.Value;
			}
		}

		private static object __STUB__Entries(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is HeadersInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'entries' is not generic.");
			return ((HeadersInstance)thisObj).Entries();
		}

		private static object __STUB__Get(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is HeadersInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((HeadersInstance)thisObj).Get("undefined");
				default:
					return ((HeadersInstance)thisObj).Get(TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__Has(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is HeadersInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'has' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((HeadersInstance)thisObj).Has("undefined");
				default:
					return ((HeadersInstance)thisObj).Has(TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__Keys(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is HeadersInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'keys' is not generic.");
			return ((HeadersInstance)thisObj).Keys();
		}

		private static object __STUB__Set(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is HeadersInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'set' is not generic.");
			switch (args.Length)
			{
				case 0:
					((HeadersInstance)thisObj).Set("undefined", "undefined"); return Undefined.Value;
				case 1:
					((HeadersInstance)thisObj).Set(TypeConverter.ToString(args[0]), "undefined"); return Undefined.Value;
				default:
					((HeadersInstance)thisObj).Set(TypeConverter.ToString(args[0]), TypeConverter.ToString(args[1])); return Undefined.Value;
			}
		}

		private static object __STUB__Values(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is HeadersInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'values' is not generic.");
			return ((HeadersInstance)thisObj).Values();
		}
	}

}
