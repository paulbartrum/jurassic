/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;

namespace Jurassic.Library
{

	public partial class StringInstance
	{
		internal new List<PropertyNameAndValue> GetDeclarativeProperties()
		{
			return new List<PropertyNameAndValue>(40)
			{
				new PropertyNameAndValue("charAt", new ClrStubFunction(this.Engine.Function.InstancePrototype, "charAt", 1, __STUB__charAt), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("charCodeAt", new ClrStubFunction(this.Engine.Function.InstancePrototype, "charCodeAt", 1, __STUB__charCodeAt), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("concat", new ClrStubFunction(this.Engine.Function.InstancePrototype, "concat", 1, __STUB__concat), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("indexOf", new ClrStubFunction(this.Engine.Function.InstancePrototype, "indexOf", 1, __STUB__indexOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("lastIndexOf", new ClrStubFunction(this.Engine.Function.InstancePrototype, "lastIndexOf", 1, __STUB__lastIndexOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("localeCompare", new ClrStubFunction(this.Engine.Function.InstancePrototype, "localeCompare", 1, __STUB__localeCompare), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("match", new ClrStubFunction(this.Engine.Function.InstancePrototype, "match", 1, __STUB__match), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("quote", new ClrStubFunction(this.Engine.Function.InstancePrototype, "quote", 0, __STUB__quote), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("replace", new ClrStubFunction(this.Engine.Function.InstancePrototype, "replace", 2, __STUB__replace), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("search", new ClrStubFunction(this.Engine.Function.InstancePrototype, "search", 1, __STUB__search), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("slice", new ClrStubFunction(this.Engine.Function.InstancePrototype, "slice", 2, __STUB__slice), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("split", new ClrStubFunction(this.Engine.Function.InstancePrototype, "split", 2, __STUB__split), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("substr", new ClrStubFunction(this.Engine.Function.InstancePrototype, "substr", 2, __STUB__substr), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("substring", new ClrStubFunction(this.Engine.Function.InstancePrototype, "substring", 2, __STUB__substring), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toLocaleLowerCase", new ClrStubFunction(this.Engine.Function.InstancePrototype, "toLocaleLowerCase", 0, __STUB__toLocaleLowerCase), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toLocaleUpperCase", new ClrStubFunction(this.Engine.Function.InstancePrototype, "toLocaleUpperCase", 0, __STUB__toLocaleUpperCase), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toLowerCase", new ClrStubFunction(this.Engine.Function.InstancePrototype, "toLowerCase", 0, __STUB__toLowerCase), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toString", new ClrStubFunction(this.Engine.Function.InstancePrototype, "toString", 0, __STUB__toString), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("toUpperCase", new ClrStubFunction(this.Engine.Function.InstancePrototype, "toUpperCase", 0, __STUB__toUpperCase), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("trim", new ClrStubFunction(this.Engine.Function.InstancePrototype, "trim", 0, __STUB__trim), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("trimLeft", new ClrStubFunction(this.Engine.Function.InstancePrototype, "trimLeft", 0, __STUB__trimLeft), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("trimRight", new ClrStubFunction(this.Engine.Function.InstancePrototype, "trimRight", 0, __STUB__trimRight), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("valueOf", new ClrStubFunction(this.Engine.Function.InstancePrototype, "valueOf", 0, __STUB__valueOf), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("anchor", new ClrStubFunction(this.Engine.Function.InstancePrototype, "anchor", 1, __STUB__anchor), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("big", new ClrStubFunction(this.Engine.Function.InstancePrototype, "big", 0, __STUB__big), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("blink", new ClrStubFunction(this.Engine.Function.InstancePrototype, "blink", 0, __STUB__blink), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("bold", new ClrStubFunction(this.Engine.Function.InstancePrototype, "bold", 0, __STUB__bold), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("fixed", new ClrStubFunction(this.Engine.Function.InstancePrototype, "fixed", 0, __STUB__fixed), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("fontcolor", new ClrStubFunction(this.Engine.Function.InstancePrototype, "fontcolor", 1, __STUB__fontcolor), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("fontsize", new ClrStubFunction(this.Engine.Function.InstancePrototype, "fontsize", 1, __STUB__fontsize), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("italics", new ClrStubFunction(this.Engine.Function.InstancePrototype, "italics", 0, __STUB__italics), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("link", new ClrStubFunction(this.Engine.Function.InstancePrototype, "link", 1, __STUB__link), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("small", new ClrStubFunction(this.Engine.Function.InstancePrototype, "small", 0, __STUB__small), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("strike", new ClrStubFunction(this.Engine.Function.InstancePrototype, "strike", 0, __STUB__strike), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("sub", new ClrStubFunction(this.Engine.Function.InstancePrototype, "sub", 0, __STUB__sub), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("sup", new ClrStubFunction(this.Engine.Function.InstancePrototype, "sup", 0, __STUB__sup), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__charAt(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return CharAt(TypeConverter.ToString(thisObj), 0);
				default:
					return CharAt(TypeConverter.ToString(thisObj), TypeConverter.ToInteger(args[0]));
			}
		}

		private static object __STUB__charCodeAt(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return CharCodeAt(TypeConverter.ToString(thisObj), 0);
				default:
					return CharCodeAt(TypeConverter.ToString(thisObj), TypeConverter.ToInteger(args[0]));
			}
		}

		private static object __STUB__concat(ScriptEngine engine, object thisObj, object[] args)
		{
			switch (args.Length)
			{
				case 0:
					return Concat(engine, thisObj, new object[0]);
				default:
					return Concat(engine, thisObj, args);
			}
		}

		private static object __STUB__indexOf(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return IndexOf(TypeConverter.ToString(thisObj), "undefined", 0);
				case 1:
					return IndexOf(TypeConverter.ToString(thisObj), TypeConverter.ToString(args[0]), 0);
				default:
					return IndexOf(TypeConverter.ToString(thisObj), TypeConverter.ToString(args[0]), TypeConverter.ToInteger(args[1], 0));
			}
		}

		private static object __STUB__lastIndexOf(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return LastIndexOf(TypeConverter.ToString(thisObj), "undefined", double.NaN);
				case 1:
					return LastIndexOf(TypeConverter.ToString(thisObj), TypeConverter.ToString(args[0]), double.NaN);
				default:
					return LastIndexOf(TypeConverter.ToString(thisObj), TypeConverter.ToString(args[0]), TypeConverter.ToNumber(args[1], double.NaN));
			}
		}

		private static object __STUB__localeCompare(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return LocaleCompare(TypeConverter.ToString(thisObj), "undefined");
				default:
					return LocaleCompare(TypeConverter.ToString(thisObj), TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__match(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return Match(engine, TypeConverter.ToString(thisObj), Undefined.Value);
				default:
					return Match(engine, TypeConverter.ToString(thisObj), args[0]);
			}
		}

		private static object __STUB__quote(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			return Quote(TypeConverter.ToString(thisObj));
		}

		private static object __STUB__replace(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return Replace(TypeConverter.ToString(thisObj), Undefined.Value, Undefined.Value);
				case 1:
					return Replace(TypeConverter.ToString(thisObj), args[0], Undefined.Value);
				default:
					return Replace(TypeConverter.ToString(thisObj), args[0], args[1]);
			}
		}

		private static object __STUB__search(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return Search(TypeConverter.ToString(thisObj), Undefined.Value);
				default:
					return Search(TypeConverter.ToString(thisObj), args[0]);
			}
		}

		private static object __STUB__slice(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return Slice(TypeConverter.ToString(thisObj), 0, int.MaxValue);
				case 1:
					return Slice(TypeConverter.ToString(thisObj), TypeConverter.ToInteger(args[0]), int.MaxValue);
				default:
					return Slice(TypeConverter.ToString(thisObj), TypeConverter.ToInteger(args[0]), TypeConverter.ToInteger(args[1], int.MaxValue));
			}
		}

		private static object __STUB__split(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return Split(engine, TypeConverter.ToString(thisObj), Undefined.Value, uint.MaxValue);
				case 1:
					return Split(engine, TypeConverter.ToString(thisObj), args[0], uint.MaxValue);
				default:
					return Split(engine, TypeConverter.ToString(thisObj), args[0], TypeConverter.ToNumber(args[1], uint.MaxValue));
			}
		}

		private static object __STUB__substr(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return Substr(TypeConverter.ToString(thisObj), 0, int.MaxValue);
				case 1:
					return Substr(TypeConverter.ToString(thisObj), TypeConverter.ToInteger(args[0]), int.MaxValue);
				default:
					return Substr(TypeConverter.ToString(thisObj), TypeConverter.ToInteger(args[0]), TypeConverter.ToInteger(args[1], int.MaxValue));
			}
		}

		private static object __STUB__substring(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return Substring(TypeConverter.ToString(thisObj), 0, int.MaxValue);
				case 1:
					return Substring(TypeConverter.ToString(thisObj), TypeConverter.ToInteger(args[0]), int.MaxValue);
				default:
					return Substring(TypeConverter.ToString(thisObj), TypeConverter.ToInteger(args[0]), TypeConverter.ToInteger(args[1], int.MaxValue));
			}
		}

		private static object __STUB__toLocaleLowerCase(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			return ToLocaleLowerCase(TypeConverter.ToString(thisObj));
		}

		private static object __STUB__toLocaleUpperCase(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			return ToLocaleUpperCase(TypeConverter.ToString(thisObj));
		}

		private static object __STUB__toLowerCase(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			return ToLowerCase(TypeConverter.ToString(thisObj));
		}

		private static object __STUB__toString(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is StringInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'toString' is not generic.");
			return ((StringInstance)thisObj).ToString();
		}

		private static object __STUB__toUpperCase(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			return ToUpperCase(TypeConverter.ToString(thisObj));
		}

		private static object __STUB__trim(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			return Trim(TypeConverter.ToString(thisObj));
		}

		private static object __STUB__trimLeft(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			return TrimLeft(TypeConverter.ToString(thisObj));
		}

		private static object __STUB__trimRight(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			return TrimRight(TypeConverter.ToString(thisObj));
		}

		private static object __STUB__valueOf(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is StringInstance))
				throw new JavaScriptException(engine, "TypeError", "The method 'valueOf' is not generic.");
			return ((StringInstance)thisObj).ValueOf();
		}

		private static object __STUB__anchor(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return Anchor(TypeConverter.ToString(thisObj), "undefined");
				default:
					return Anchor(TypeConverter.ToString(thisObj), TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__big(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			return Big(TypeConverter.ToString(thisObj));
		}

		private static object __STUB__blink(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			return Blink(TypeConverter.ToString(thisObj));
		}

		private static object __STUB__bold(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			return Bold(TypeConverter.ToString(thisObj));
		}

		private static object __STUB__fixed(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			return Fixed(TypeConverter.ToString(thisObj));
		}

		private static object __STUB__fontcolor(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return FontColor(TypeConverter.ToString(thisObj), "undefined");
				default:
					return FontColor(TypeConverter.ToString(thisObj), TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__fontsize(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return FontSize(TypeConverter.ToString(thisObj), "undefined");
				default:
					return FontSize(TypeConverter.ToString(thisObj), TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__italics(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			return Italics(TypeConverter.ToString(thisObj));
		}

		private static object __STUB__link(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			switch (args.Length)
			{
				case 0:
					return Link(TypeConverter.ToString(thisObj), "undefined");
				default:
					return Link(TypeConverter.ToString(thisObj), TypeConverter.ToString(args[0]));
			}
		}

		private static object __STUB__small(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			return Small(TypeConverter.ToString(thisObj));
		}

		private static object __STUB__strike(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			return Strike(TypeConverter.ToString(thisObj));
		}

		private static object __STUB__sub(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			return Sub(TypeConverter.ToString(thisObj));
		}

		private static object __STUB__sup(ScriptEngine engine, object thisObj, object[] args)
		{
			if (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)
				throw new JavaScriptException(engine, "TypeError", "Cannot convert undefined or null to object.");
			return Sup(TypeConverter.ToString(thisObj));
		}
	}

}
