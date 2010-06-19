using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnitTests
{
    public enum ScriptLanguage
    {
        VBScript,
        JScript
    }

    /// <summary>
    /// Represents an engine that can execute script.
    /// </summary>
    public class ActiveScriptEngine : IActiveScriptSite, IDisposable
    {
        ScriptLanguage language;
        IActiveScript engine;
        IActiveScriptParse parser;
        string lastError;

        /// <summary>
        /// Creates a scripting engine host given the name of a script engine.
        /// </summary>
        /// <param name="engineName"> The name of the script engine. </param>
        /// <returns> A ScriptEngine instance. </returns>
        public static ActiveScriptEngine FromName(string engineName)
        {
            switch (engineName)
            {
                case "JScript":
                    return new ActiveScriptEngine(new JScriptEngine(), ScriptLanguage.JScript);
                case "VBScript":
                    return new ActiveScriptEngine(new VBScriptEngine(), ScriptLanguage.VBScript);
            }
            throw new NotImplementedException(string.Format("The scripting engine '{0}' is not supported.", engineName));
        }

        /// <summary>
        /// Creates a scripting engine host given a scripting language.
        /// </summary>
        /// <param name="engineName"> The language to host. </param>
        /// <returns> A ScriptEngine instance. </returns>
        public static ActiveScriptEngine FromLanguage(ScriptLanguage language)
        {
            switch (language)
            {
                case ScriptLanguage.JScript:
                    return new ActiveScriptEngine(new JScriptEngine(), ScriptLanguage.JScript);
                case ScriptLanguage.VBScript:
                    return new ActiveScriptEngine(new VBScriptEngine(), ScriptLanguage.VBScript);
            }
            throw new NotImplementedException(string.Format("The scripting language '{0}' is not supported.", language));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveScriptEngine"/> class.
        /// </summary>
        /// <param name="scriptEngine"> The script engine. </param>
        /// <param name="language"> The scripting language. </param>
        private ActiveScriptEngine(object scriptEngine, ScriptLanguage language)
        {
            if (scriptEngine == null)
                throw new ArgumentNullException("scriptEngine");
            this.language = language;
            this.engine = (IActiveScript)scriptEngine;
            this.engine.SetScriptSite(this);
            this.parser = (IActiveScriptParse)this.engine;
            this.parser.InitNew();

            // Set properties.
            var activeScriptProperty = (IActiveScriptProperty)scriptEngine;

            // Indicate that we are not combining multiple script engines.
            object value = true;
            activeScriptProperty.SetProperty(SCRIPTPROP.ABBREVIATE_GLOBALNAME_RESOLUTION, IntPtr.Zero, ref value);

            // Upgrade the version of the script engine to 5.8 (IE 8).
            value = SCRIPTLANGUAGEVERSION.V5_8;
            activeScriptProperty.SetProperty(SCRIPTPROP.INVOKEVERSIONING, IntPtr.Zero, ref value);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.engine != null)
                this.engine.Close();
            if (this.parser != null)
                Marshal.ReleaseComObject(this.parser);
            this.parser = null;
            if (this.engine != null)
                Marshal.ReleaseComObject(this.engine);
            this.engine = null;
        }

        /// <summary>
        /// Parses the specified script code.
        /// </summary>
        /// <param name="code"> The code to parse. </param>
        public void Parse(string code)
        {
            Parse(code, SCRIPTITEM.ISVISIBLE);
        }

        /// <summary>
        /// Evaluates an expression.
        /// </summary>
        /// <param name="expression"> The expression to evaluate. </param>
        /// <returns> The result of the expression. </returns>
        public object Evaluate(string expression)
        {
            return Parse(expression, SCRIPTITEM.ISEXPRESSION);
        }

        /// <summary>
        /// Calls the function with the given name.
        /// </summary>
        /// <param name="functionName"> The name of the function to call. </param>
        /// <param name="parameters"> The parameters to pass to the function.</param>
        /// <returns> The value returned by the function, or <c>null</c> if the function did not
        /// return a value. </returns>
        public object CallFunction(string functionName, params object[] parameters)
        {
            var expression = new System.Text.StringBuilder();
            expression.Append(functionName);
            expression.Append("(");
            bool firstParameter = true;
            foreach (object parameter in parameters)
            {
                if (firstParameter == false)
                    expression.Append(", ");
                expression.Append(FormatValue(parameter));
                firstParameter = false;
            }
            expression.Append(")");
            return Evaluate(expression.ToString());
        }

        /// <summary>
        /// Converts a value to the equivalent script literal.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <returns> The script literal that is equivalent to the given value. </returns>
        public string FormatValue(object value)
        {
            if (value is string)
            {
                if (language == ScriptLanguage.JScript)
                    return string.Format("\"{0}\"", value.ToString().Replace("\"", "\\\""));
                else
                    return string.Format("\"{0}\"", value.ToString().Replace("\"", "\"\""));
            }
            else if (value is int || value is float || value is double)
            {
                return value.ToString();
            }
            else if (value is bool)
            {
                return (bool)value ? "true" : "false";
            }
            else if (value is DateTime)
            {
                DateTime date = (DateTime)value;
                if (language == ScriptLanguage.JScript)
                    return string.Format("new Date({0})", (date - new DateTime(1970, 1, 1)).TotalMilliseconds);
                else
                    return string.Format("#{0}/{1}/{2}#", date.Month, date.Day, date.Year);
            }
            else
                throw new NotImplementedException(string.Format("Unsupported type '{0}'.", value.GetType().FullName));
        }

        /// <summary>
        /// Parses the specified script code.
        /// </summary>
        /// <param name="code"> The code to parse. </param>
        /// <param name="flags"> The parsing flags. </param>
        /// <returns> The result of the expression, if ISEXPRESSION was passed. </returns>
        private object Parse(string code, SCRIPTITEM flags)
        {
            EXCEPINFO exceptionInfo;
            object result = null;
            int errorCode = this.parser.ParseScriptText(code, null, IntPtr.Zero, null, 0, 0, flags, out result, out exceptionInfo);
            if (errorCode == unchecked((int)0x80020009))    // DISP_E_EXCEPTION
                throw new COMException(string.Format("{0} - {1}", exceptionInfo.bstrSource, exceptionInfo.bstrDescription));
            if (errorCode == unchecked((int)0x80020101))    // OLESCRIPT_E_SYNTAX
                throw new System.Runtime.InteropServices.COMException("Syntax error");
            if (errorCode != 0)
                Marshal.ThrowExceptionForHR(errorCode);
            return result;
        }

        #region IActiveScriptSite Members

        private const int E_NOTIMPL = unchecked((int)0x80004001);

        /// <summary>
        /// Retrieves the locale identifier that the host uses for displaying user-interface
        /// elements.
        /// </summary>
        /// <param name="id"> The locale identifier for user-interface elements displayed by the
        /// scripting engine. </param>
        int IActiveScriptSite.GetLCID(out uint id)
        {
            // Use the system-defined locale.
            id = 0;
            return E_NOTIMPL;
        }

        /// <summary>
        /// Obtains information about an item that was added to an engine through a call to the
        /// IActiveScript::AddNamedItem method.
        /// </summary>
        /// <param name="pstrName"> The name associated with the item, as specified in the
        /// IActiveScript::AddNamedItem method. </param>
        /// <param name="dwReturnMask"> A bit mask specifying what information about the item
        /// should be returned. </param>
        /// <param name="item"> The IUnknown interface associated with the given item. </param>
        /// <param name="ppti"> A pointer to the ITypeInfo interface associated with the item. </param>
        int IActiveScriptSite.GetItemInfo([In, MarshalAs(UnmanagedType.BStr)] string pstrName, [In, MarshalAs(UnmanagedType.U4)] uint dwReturnMask, [Out, MarshalAs(UnmanagedType.IUnknown)] out object item, out IntPtr ppti)
        {
            item = null;
            ppti = IntPtr.Zero;
            return E_NOTIMPL;
        }

        /// <summary>
        /// Retrieves a host-defined string that uniquely identifies the current document version
        /// from the host's point of view.
        /// </summary>
        /// <param name="v"> The host-defined document version string. </param>
        int IActiveScriptSite.GetDocVersionString(out string v)
        {
            // Causes the scripting engine to assume that the script is in sync with the document.
            v = null;
            return E_NOTIMPL;
        }

        /// <summary>
        /// Called when the script has completed execution.
        /// </summary>
        /// <param name="result"> The script result, or <c>null</c> if the script produced no result.
        /// </param>
        /// <param name="info"> The exception information generated when the script terminated, or
        /// <c>null</c> if no exception was generated. </param>
        void IActiveScriptSite.OnScriptTerminate(ref object result, ref EXCEPINFO info)
        {
        }

        /// <summary>
        /// Informs the host that the scripting engine has changed states.
        /// </summary>
        /// <param name="state"> The new script state. </param>
        void IActiveScriptSite.OnStateChange(SCRIPTSTATE state)
        {
        }

        /// <summary>
        /// Informs the host that an execution error occurred while the engine was running the
        /// script.
        /// </summary>
        /// <param name="err"> Information about the execution error. </param>
        void IActiveScriptSite.OnScriptError(IActiveScriptError err)
        {
            EXCEPINFO exceptionInfo;
            err.GetExceptionInfo(out exceptionInfo);
            lastError = string.Format("{0} - {1}", exceptionInfo.bstrSource, exceptionInfo.bstrDescription);
        }

        /// <summary>
        /// Informs the host that the scripting engine has begun executing the script code.
        /// </summary>
        void IActiveScriptSite.OnEnterScript()
        {
        }

        /// <summary>
        /// Informs the host that the scripting engine has returned from executing script code.
        /// </summary>
        void IActiveScriptSite.OnLeaveScript()
        {
        }

        #endregion
    }
}
