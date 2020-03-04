/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;
using Jurassic.Library;

namespace Jurassic.Extensions.Fetch
{

	public partial class RequestInstance
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(5)
			{
				new PropertyNameAndValue("cache", new PropertyDescriptor(new ClrStubFunction(engine, "get cache", 0, __GETTER__Cache), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("credentials", new PropertyDescriptor(new ClrStubFunction(engine, "get credentials", 0, __GETTER__Credentials), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("destination", new PropertyDescriptor(new ClrStubFunction(engine, "get destination", 0, __GETTER__Destination), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("headers", new PropertyDescriptor(new ClrStubFunction(engine, "get headers", 0, __GETTER__Headers), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("integrity", new PropertyDescriptor(new ClrStubFunction(engine, "get integrity", 0, __GETTER__Integrity), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("method", new PropertyDescriptor(new ClrStubFunction(engine, "get method", 0, __GETTER__Method), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("mode", new PropertyDescriptor(new ClrStubFunction(engine, "get mode", 0, __GETTER__Mode), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("redirect", new PropertyDescriptor(new ClrStubFunction(engine, "get redirect", 0, __GETTER__Redirect), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("referrer", new PropertyDescriptor(new ClrStubFunction(engine, "get referrer", 0, __GETTER__Referrer), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("referrerPolicy", new PropertyDescriptor(new ClrStubFunction(engine, "get referrerPolicy", 0, __GETTER__ReferrerPolicy), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("url", new PropertyDescriptor(new ClrStubFunction(engine, "get url", 0, __GETTER__Url), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("clone", new ClrStubFunction(engine, "clone", 0, __STUB__Clone), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __GETTER__Cache(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RequestInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get cache' is not generic.");
			return ((RequestInstance)thisObj).Cache;
		}

		private static object __GETTER__Credentials(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RequestInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get credentials' is not generic.");
			return ((RequestInstance)thisObj).Credentials;
		}

		private static object __GETTER__Destination(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RequestInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get destination' is not generic.");
			return ((RequestInstance)thisObj).Destination;
		}

		private static object __GETTER__Headers(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RequestInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get headers' is not generic.");
			return ((RequestInstance)thisObj).Headers;
		}

		private static object __GETTER__Integrity(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RequestInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get integrity' is not generic.");
			return ((RequestInstance)thisObj).Integrity;
		}

		private static object __GETTER__Method(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RequestInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get method' is not generic.");
			return ((RequestInstance)thisObj).Method;
		}

		private static object __GETTER__Mode(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RequestInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get mode' is not generic.");
			return ((RequestInstance)thisObj).Mode;
		}

		private static object __GETTER__Redirect(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RequestInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get redirect' is not generic.");
			return ((RequestInstance)thisObj).Redirect;
		}

		private static object __GETTER__Referrer(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RequestInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get referrer' is not generic.");
			return ((RequestInstance)thisObj).Referrer;
		}

		private static object __GETTER__ReferrerPolicy(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RequestInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get referrerPolicy' is not generic.");
			return ((RequestInstance)thisObj).ReferrerPolicy;
		}

		private static object __GETTER__Url(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RequestInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get url' is not generic.");
			return ((RequestInstance)thisObj).Url;
		}

		private static object __STUB__Clone(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is RequestInstance))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'clone' is not generic.");
			return ((RequestInstance)thisObj).Clone();
		}
	}

}
