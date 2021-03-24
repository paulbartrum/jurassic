using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace UnitTests
{

    /// <summary>
    /// Test the code-gen for short code snippets.
    /// </summary>
    [TestClass]
    public class CodeGenTests
    {
        [TestMethod]
        public void GlobalVariableSet()
        {
            Assert.AreEqual(NormalizeText(@"
                .local [0] System.Int32
                      ldc.i4     6
                      stloc      V0
                      ldarg      0
                      callvirt   Jurassic.Compiler.RuntimeScope get_ParentScope()/Jurassic.Compiler.ExecutionContext
                      ldstr      ""x""
                      ldloc      V0
                      box        System.Int32
                      ldc.i4     1
                      ldnull
                      callvirt   Void SetValue(System.String, System.Object, Int32, System.String)/Jurassic.Compiler.RuntimeScope
                      ldnull
                      ret"), GetGlobalIL(@"x = 6"));
        }
        
        [TestMethod]
        public void Let()
        {
            Assert.AreEqual(NormalizeText(@"
                .local [0] System.Object f
                .local [1] System.Object arguments
                .local [2] System.Object a
                .local [3] System.Object b
                .local [4] System.Object a
                .local [5] System.Object returnValue
                      ldnull
                      stloc      V0 (f)
                      ldnull
                      stloc      V1 (arguments)
                      ldnull
                      stloc      V2 (a)
                      ldnull
                      stloc      V3 (b)
                      ldc.i4     11
                      box        System.Int32
                      stloc      V2 (a)
                      ldsfld     Jurassic.Undefined Value/Jurassic.Undefined
                      stloc      V3 (b)
                      ldnull
                      stloc      V4 (a)
                      ldc.i4     13
                      box        System.Int32
                      stloc      V4 (a)
                      ldloc      V4 (a)
                      dup
                      brtrue     L000
                      ldc.i4     6
                      ldstr      ""Cannot access 'a' before initialization.""
                      ldc.i4     0
                      ldnull
                      ldnull
                      newobj     Void .ctor(Jurassic.Library.ErrorType, System.String, Int32, System.String, System.String)/Jurassic.JavaScriptException
                      throw
                L000: stloc      V3 (b)
                      ldloc      V3 (b)
                      dup
                      brtrue     L001
                      ldc.i4     6
                      ldstr      ""Cannot access 'b' before initialization.""
                      ldc.i4     0
                      ldnull
                      ldnull
                      newobj     Void .ctor(Jurassic.Library.ErrorType, System.String, Int32, System.String, System.String)/Jurassic.JavaScriptException
                      throw
                L001: ldloc      V2 (a)
                      dup
                      brtrue     L002
                      ldc.i4     6
                      ldstr      ""Cannot access 'a' before initialization.""
                      ldc.i4     0
                      ldnull
                      ldnull
                      newobj     Void .ctor(Jurassic.Library.ErrorType, System.String, Int32, System.String, System.String)/Jurassic.JavaScriptException
                      throw
                L002: call       System.Object Add(System.Object, System.Object)/Jurassic.TypeUtilities
                      stloc      V3 (b)
                      ldloc      V3 (b)
                      dup
                      brtrue     L003
                      ldc.i4     6
                      ldstr      ""Cannot access 'b' before initialization.""
                      ldc.i4     0
                      ldnull
                      ldnull
                      newobj     Void .ctor(Jurassic.Library.ErrorType, System.String, Int32, System.String, System.String)/Jurassic.JavaScriptException
                      throw
                L003: stloc      V5 (returnValue)
                      ldloc      V5 (returnValue)
                      ret"),
            GetFunctionIL(@"function f() {
                let a = 11, b;
                {
                    let a = 13;
                    b = a;
                }
                b += a;
                return b;
            }", "f"));
        }

        private static string GetGlobalIL(string code)
        {
            var script = CompiledScript.Compile(new StringScriptSource(code),
                new Jurassic.Compiler.CompilerOptions { EnableILAnalysis = true });
            return NormalizeText(script.DisassembledIL);
        }

        private static string GetEvalIL(string code)
        {
            var script = CompiledEval.Compile(new StringScriptSource(code),
                new Jurassic.Compiler.CompilerOptions { EnableILAnalysis = true });
            return NormalizeText(script.DisassembledIL);
        }

        private static string GetFunctionIL(string code, string functionName)
        {
            var scriptEngine = new ScriptEngine();
            scriptEngine.EnableILAnalysis = true;
            scriptEngine.Execute(code);
            var function = (Jurassic.Library.UserDefinedFunction)scriptEngine.GetGlobalValue(functionName);
            if (function == null)
                throw new ArgumentException(string.Format("The function {0} was not found.", functionName));
            return NormalizeText(function.DisassembledIL);
        }

        /// <summary>
        /// Removes spaces from the start of each line and removes extraneous line breaks from the
        /// start and end of the given text.
        /// </summary>
        /// <param name="text"> The text to operate on. </param>
        /// <param name="lineBreak"> The type of line break to normalize to. </param>
        /// <returns> The text, but with extra space removed. </returns>
        private static string NormalizeText(string text, string lineBreak = null)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            // Find the maximum number of spaces that is common to each line.
            bool startOfLine = true;
            int indentationToRemove = int.MaxValue;
            int startOfLineSpace = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\r' || text[i] == '\n')
                {
                    startOfLine = true;
                    startOfLineSpace = 0;
                }
                else if (startOfLine == true)
                {
                    if (text[i] == ' ')
                        startOfLineSpace++;
                    else
                    {
                        indentationToRemove = Math.Min(indentationToRemove, startOfLineSpace);
                        startOfLine = false;
                    }
                }
            }

            // Remove that amount of space from each line.
            // Also, normalize line breaks to Environment.NewLine.
            var result = new StringBuilder(text.Length);
            int j = 0;
            for (; j < Math.Min(indentationToRemove, text.Length); j++)
                if (text[j] != ' ')
                    break;
            for (int i = j; i < text.Length; i++)
            {
                if (text[i] == '\r' || text[i] == '\n')
                {
                    if (text[i] == '\r' && i < text.Length - 1 && text[i + 1] == '\n')
                        i++;
                    result.Append(lineBreak == null ? Environment.NewLine : lineBreak);
                    i++;
                    for (j = i; j < Math.Min(i + indentationToRemove, text.Length); j++)
                        if (text[j] != ' ')
                            break;
                    i = j - 1;
                }
                else
                    result.Append(text[i]);
            }
            return result.ToString().Trim('\r', '\n');
        }
    }

}