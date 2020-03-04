/*
 * This file is auto-generated, do not modify directly.
 */

using System.Collections.Generic;
using Jurassic;
using Jurassic.Library;

namespace Jurassic.Extensions.FirebugConsole
{

	public partial class FirebugConsole
	{
		private static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)
		{
			return new List<PropertyNameAndValue>(16)
			{
				new PropertyNameAndValue("log", new ClrStubFunction(engine, "log", 1, __STUB__Log), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("debug", new ClrStubFunction(engine, "debug", 1, __STUB__Debug), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("info", new ClrStubFunction(engine, "info", 1, __STUB__Info), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("warn", new ClrStubFunction(engine, "warn", 1, __STUB__Warn), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("error", new ClrStubFunction(engine, "error", 1, __STUB__Error), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("assert", new ClrStubFunction(engine, "assert", 2, __STUB__Assert), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("clear", new ClrStubFunction(engine, "clear", 1, __STUB__Clear), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("group", new ClrStubFunction(engine, "group", 1, __STUB__Group), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("groupCollapsed", new ClrStubFunction(engine, "groupCollapsed", 1, __STUB__GroupCollapsed), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("groupEnd", new ClrStubFunction(engine, "groupEnd", 0, __STUB__GroupEnd), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("time", new ClrStubFunction(engine, "time", 1, __STUB__Time), PropertyAttributes.NonEnumerable),
				new PropertyNameAndValue("timeEnd", new ClrStubFunction(engine, "timeEnd", 1, __STUB__TimeEnd), PropertyAttributes.NonEnumerable),
			};
		}

		private static object __STUB__Log(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is FirebugConsole))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'log' is not generic.");
			switch (args.Length)
			{
				case 0:
					((FirebugConsole)thisObj).Log(new object[0]); return Undefined.Value;
				default:
					((FirebugConsole)thisObj).Log(args); return Undefined.Value;
			}
		}

		private static object __STUB__Debug(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is FirebugConsole))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'debug' is not generic.");
			switch (args.Length)
			{
				case 0:
					((FirebugConsole)thisObj).Debug(new object[0]); return Undefined.Value;
				default:
					((FirebugConsole)thisObj).Debug(args); return Undefined.Value;
			}
		}

		private static object __STUB__Info(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is FirebugConsole))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'info' is not generic.");
			switch (args.Length)
			{
				case 0:
					((FirebugConsole)thisObj).Info(new object[0]); return Undefined.Value;
				default:
					((FirebugConsole)thisObj).Info(args); return Undefined.Value;
			}
		}

		private static object __STUB__Warn(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is FirebugConsole))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'warn' is not generic.");
			switch (args.Length)
			{
				case 0:
					((FirebugConsole)thisObj).Warn(new object[0]); return Undefined.Value;
				default:
					((FirebugConsole)thisObj).Warn(args); return Undefined.Value;
			}
		}

		private static object __STUB__Error(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is FirebugConsole))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'error' is not generic.");
			switch (args.Length)
			{
				case 0:
					((FirebugConsole)thisObj).Error(new object[0]); return Undefined.Value;
				default:
					((FirebugConsole)thisObj).Error(args); return Undefined.Value;
			}
		}

		private static object __STUB__Assert(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is FirebugConsole))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'assert' is not generic.");
			switch (args.Length)
			{
				case 0:
					((FirebugConsole)thisObj).Assert(false, new object[0]); return Undefined.Value;
				case 1:
					((FirebugConsole)thisObj).Assert(TypeConverter.ToBoolean(args[0]), new object[0]); return Undefined.Value;
				default:
					((FirebugConsole)thisObj).Assert(TypeConverter.ToBoolean(args[0]), TypeUtilities.SliceArray(args, 1)); return Undefined.Value;
			}
		}

		private static object __STUB__Clear(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is FirebugConsole))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'clear' is not generic.");
			switch (args.Length)
			{
				case 0:
					((FirebugConsole)thisObj).Clear(new object[0]); return Undefined.Value;
				default:
					((FirebugConsole)thisObj).Clear(args); return Undefined.Value;
			}
		}

		private static object __STUB__Group(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is FirebugConsole))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'group' is not generic.");
			switch (args.Length)
			{
				case 0:
					((FirebugConsole)thisObj).Group(new object[0]); return Undefined.Value;
				default:
					((FirebugConsole)thisObj).Group(args); return Undefined.Value;
			}
		}

		private static object __STUB__GroupCollapsed(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is FirebugConsole))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'groupCollapsed' is not generic.");
			switch (args.Length)
			{
				case 0:
					((FirebugConsole)thisObj).GroupCollapsed(new object[0]); return Undefined.Value;
				default:
					((FirebugConsole)thisObj).GroupCollapsed(args); return Undefined.Value;
			}
		}

		private static object __STUB__GroupEnd(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is FirebugConsole))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'groupEnd' is not generic.");
			((FirebugConsole)thisObj).GroupEnd(); return Undefined.Value;
		}

		private static object __STUB__Time(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is FirebugConsole))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'time' is not generic.");
			switch (args.Length)
			{
				case 0:
					((FirebugConsole)thisObj).Time(""); return Undefined.Value;
				default:
					((FirebugConsole)thisObj).Time(TypeUtilities.IsUndefined(args[0]) ? "" : TypeConverter.ToString(args[0])); return Undefined.Value;
			}
		}

		private static object __STUB__TimeEnd(ScriptEngine engine, object thisObj, object[] args)
		{
			thisObj = TypeConverter.ToObject(engine, thisObj);
			if (!(thisObj is FirebugConsole))
				throw new JavaScriptException(engine, ErrorType.TypeError, "The method 'timeEnd' is not generic.");
			switch (args.Length)
			{
				case 0:
					((FirebugConsole)thisObj).TimeEnd(""); return Undefined.Value;
				default:
					((FirebugConsole)thisObj).TimeEnd(TypeUtilities.IsUndefined(args[0]) ? "" : TypeConverter.ToString(args[0])); return Undefined.Value;
			}
		}
	}

}
