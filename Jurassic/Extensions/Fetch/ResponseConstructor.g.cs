/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;
using Jurassic.Library;

namespace Jurassic.Extensions.Fetch
{

	public partial class ResponseConstructor
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(6)
			{
				new PropertyNameAndValue(engine.Symbol.Species, new PropertyDescriptor(new ClrStubFunction(engine.FunctionInstancePrototype, "get [Symbol.species]", 0, __GETTER__Species), null, PropertyAttributes.Configurable)),
				new PropertyNameAndValue("error", new ClrStubFunction(engine.FunctionInstancePrototype, "error", 0, __STUB__Error), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("redirect", new ClrStubFunction(engine.FunctionInstancePrototype, "redirect", 2, __STUB__Redirect), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Call(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ResponseConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'Call' is not generic.");
			return ((ResponseConstructor)thisObj).Call();
		}

		private static ObjectInstance __STUB__Construct(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ResponseConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'Construct' is not generic.");
			switch (args.Length)
			{
				case 0:
					return ((ResponseConstructor)thisObj).Construct(Undefined.Value, null);
				case 1:
					return ((ResponseConstructor)thisObj).Construct(args[0], null);
				default:
					return ((ResponseConstructor)thisObj).Construct(args[0], TypeUtilities.IsUndefined(args[1]) ? null : TypeConverter.ToObject(engine, args[1]));
			}
		}

		private static object __GETTER__Species(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is ResponseConstructor))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'get [Symbol.species]' is not generic.");
			return ((ResponseConstructor)thisObj).Species;
		}

		private static object __STUB__Error(ScriptEngine engine, object thisObj, object[] args)
		{
			return Error(engine);
		}

		private static object __STUB__Redirect(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Redirect(engine, "undefined", 302);
				case 1:
					return Redirect(engine, TypeConverter.ToString(args[0]), 302);
				default:
					return Redirect(engine, TypeConverter.ToString(args[0]), TypeUtilities.IsUndefined(args[1]) ? 302 : TypeConverter.ToInteger(args[1]));
			}
		}
	}

}
