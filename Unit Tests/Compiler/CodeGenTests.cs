using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace Performance
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
                .local [1] System.Int32
                      ldc.i4     6
                      dup
                      stloc      V0
                      stloc      V1
                      ldarg      0
                      callvirt   Jurassic.Compiler.RuntimeScope get_ParentScope()/Jurassic.Compiler.ExecutionContext
                      ldstr      ""x""
                      ldloc      V1
                      box        System.Int32
                      callvirt   Void SetValue(System.String, System.Object)/Jurassic.Compiler.RuntimeScope
                      ldloc      V0
                      pop
                      ldnull
                      ret"), GetGlobalIL(@"x = 6"));
        }
        
        [TestMethod]
        public void Let()
        {
            Assert.AreEqual(NormalizeText(@"
                .local [0] Jurassic.Compiler.RuntimeScope scope
                .local [1] System.Int32
                .local [2] System.Int32
                .local [3] Jurassic.Compiler.RuntimeScope scope
                .local [4] System.Object
                .local [5] System.Object
                .local [6] System.Object returnValue
                      ldarg      0
                      ldnull
                      ldnull
                      ldc.i4     3
                      newarr     System.String
                      dup
                      ldc.i4     0
                      ldstr      ""arguments""
                      stelem     System.String
                      dup
                      ldc.i4     1
                      ldstr      ""a""
                      stelem     System.String
                      dup
                      ldc.i4     2
                      ldstr      ""b""
                      stelem     System.String
                      ldc.i4     1
                      newarr     System.String
                      dup
                      ldc.i4     0
                      ldstr      ""f""
                      stelem     System.String
                      callvirt   Jurassic.Compiler.RuntimeScope CreateRuntimeScope(Jurassic.Compiler.RuntimeScope, System.String[], System.String[], System.String[])/Jurassic.Compiler.ExecutionContext
                      stloc      V0 (scope)
                      ldc.i4     11
                      dup
                      stloc      V1
                      stloc      V2
                      ldloc      V0 (scope)
                      ldstr      ""a""
                      ldloc      V2
                      box        System.Int32
                      callvirt   Void SetValue(System.String, System.Object)/Jurassic.Compiler.RuntimeScope
                      ldloc      V1
                      pop
                      ldarg      0
                      ldloc      V0 (scope)
                      ldnull
                      ldc.i4     1
                      newarr     System.String
                      dup
                      ldc.i4     0
                      ldstr      ""a""
                      stelem     System.String
                      ldnull
                      callvirt   Jurassic.Compiler.RuntimeScope CreateRuntimeScope(Jurassic.Compiler.RuntimeScope, System.String[], System.String[], System.String[])/Jurassic.Compiler.ExecutionContext
                      stloc      V3 (scope)
                      ldc.i4     13
                      dup
                      stloc      V2
                      stloc      V1
                      ldloc      V3 (scope)
                      ldstr      ""a""
                      ldloc      V1
                      box        System.Int32
                      callvirt   Void SetValue(System.String, System.Object)/Jurassic.Compiler.RuntimeScope
                      ldloc      V2
                      pop
                      ldloc      V3 (scope)
                      ldstr      ""a""
                      callvirt   System.Object GetValue(System.String)/Jurassic.Compiler.RuntimeScope
                      dup
                      stloc      V4
                      stloc      V5
                      ldloc      V3 (scope)
                      ldstr      ""b""
                      ldloc      V5
                      callvirt   Void SetValue(System.String, System.Object)/Jurassic.Compiler.RuntimeScope
                      ldloc      V4
                      pop
                      ldloc      V0 (scope)
                      ldstr      ""b""
                      callvirt   System.Object GetValue(System.String)/Jurassic.Compiler.RuntimeScope
                      ldloc      V0 (scope)
                      ldstr      ""a""
                      callvirt   System.Object GetValue(System.String)/Jurassic.Compiler.RuntimeScope
                      call       System.Object Add(System.Object, System.Object)/Jurassic.TypeUtilities
                      dup
                      stloc      V5
                      stloc      V4
                      ldloc      V0 (scope)
                      ldstr      ""b""
                      ldloc      V4
                      callvirt   Void SetValue(System.String, System.Object)/Jurassic.Compiler.RuntimeScope
                      ldloc      V5
                      pop
                      ldloc      V0 (scope)
                      ldstr      ""b""
                      callvirt   System.Object GetValue(System.String)/Jurassic.Compiler.RuntimeScope
                      stloc      V6 (returnValue)
                      ldloc      V6 (returnValue)
                      ret
                "),
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