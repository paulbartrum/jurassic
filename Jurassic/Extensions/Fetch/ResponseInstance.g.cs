/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;
using Jurassic.Library;

namespace Jurassic.Extensions.Fetch
{

	public partial class ResponseInstance
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(4)
			{
				new PropertyNameAndValue("type", new PropertyDescriptor(new ClrStubFunction(engine.FunctionInstancePrototype, "get type", 0, __GETTER__Type), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("url", new PropertyDescriptor(new ClrStubFunction(engine.FunctionInstancePrototype, "get url", 0, __GETTER__Url), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("redirected", new PropertyDescriptor(new ClrStubFunction(engine.FunctionInstancePrototype, "get redirected", 0, __GETTER__Redirected), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("status", new PropertyDescriptor(new ClrStubFunction(engine.FunctionInstancePrototype, "get status", 0, __GETTER__Status), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("ok", new PropertyDescriptor(new ClrStubFunction(engine.FunctionInstancePrototype, "get ok", 0, __GETTER__Ok), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("statusText", new PropertyDescriptor(new ClrStubFunction(engine.FunctionInstancePrototype, "get statusText", 0, __GETTER__StatusText), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("headers", new PropertyDescriptor(new ClrStubFunction(engine.FunctionInstancePrototype, "get headers", 0, __GETTER__Headers), null, PropertyAttributes.Configurable)),
			};
		}

		private static object __GETTER__Type(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ResponseInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get type' is not generic.");
			return ((ResponseInstance)thisObj).Type;
		}

		private static object __GETTER__Url(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ResponseInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get url' is not generic.");
			return ((ResponseInstance)thisObj).Url;
		}

		private static object __GETTER__Redirected(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ResponseInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get redirected' is not generic.");
			return ((ResponseInstance)thisObj).Redirected;
		}

		private static object __GETTER__Status(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ResponseInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get status' is not generic.");
			return ((ResponseInstance)thisObj).Status;
		}

		private static object __GETTER__Ok(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ResponseInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get ok' is not generic.");
			return ((ResponseInstance)thisObj).Ok;
		}

		private static object __GETTER__StatusText(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ResponseInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get statusText' is not generic.");
			return ((ResponseInstance)thisObj).StatusText;
		}

		private static object __GETTER__Headers(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ResponseInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get headers' is not generic.");
			return ((ResponseInstance)thisObj).Headers;
		}
	}

}
