/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;
using Jurassic.Library;

namespace Jurassic.Extensions.Fetch
{

	public abstract partial class BodyInstance
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(9)
			{
				new PropertyNameAndValue("body", new PropertyDescriptor(new ClrStubFunction(engine.FunctionInstancePrototype, "get body", 0, __GETTER__Body), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("bodyUsed", new PropertyDescriptor(new ClrStubFunction(engine.FunctionInstancePrototype, "get bodyUsed", 0, __GETTER__BodyUsed), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("arrayBuffer", new ClrStubFunction(engine.FunctionInstancePrototype, "arrayBuffer", 0, __STUB__ArrayBuffer), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("blob", new ClrStubFunction(engine.FunctionInstancePrototype, "blob", 0, __STUB__Blob), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("formData", new ClrStubFunction(engine.FunctionInstancePrototype, "formData", 0, __STUB__FormData), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("json", new ClrStubFunction(engine.FunctionInstancePrototype, "json", 0, __STUB__Json), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("text", new ClrStubFunction(engine.FunctionInstancePrototype, "text", 0, __STUB__Text), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __GETTER__Body(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is BodyInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get body' is not generic.");
			return ((BodyInstance)thisObj).Body;
		}

		private static object __GETTER__BodyUsed(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is BodyInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get bodyUsed' is not generic.");
			return ((BodyInstance)thisObj).BodyUsed;
		}

		private static object __STUB__ArrayBuffer(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is BodyInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'arrayBuffer' is not generic.");
			return ((BodyInstance)thisObj).ArrayBuffer();
		}

		private static object __STUB__Blob(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is BodyInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'blob' is not generic.");
			return ((BodyInstance)thisObj).Blob();
		}

		private static object __STUB__FormData(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is BodyInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'formData' is not generic.");
			return ((BodyInstance)thisObj).FormData();
		}

		private static object __STUB__Json(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is BodyInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'json' is not generic.");
			return ((BodyInstance)thisObj).Json();
		}

		private static object __STUB__Text(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is BodyInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'text' is not generic.");
			return ((BodyInstance)thisObj).Text();
		}
	}

}
