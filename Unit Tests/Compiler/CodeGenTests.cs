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
        public void EmptyFunction()
        {
            Assert.AreEqual(NormalizeText(@"
                      ldnull
                      ret
                "),
                GetFunctionIL(@"function f() {
                }", "f"));
        }

        [TestMethod]
        public void SimpleReturn()
        {
            Assert.AreEqual(NormalizeText(@"
                .local [0] System.Object returnValue
                      ldc.i4     5
                      box        System.Int32
                      stloc      V0 (returnValue)
                      ldloc      V0 (returnValue)
                      ret
                "),
                GetFunctionIL(@"function f() {
                    return 5;
                }", "f"));
        }

        [TestMethod]
        [Ignore("Needs investigation")]
        public void FunctionCall0()
        {
            Assert.AreEqual(NormalizeText(@"
                .local [0] System.Object a
                      ldarg      4
                      ldlen
                      ldc.i4     0
                      ble        L000
                      ldarg      4
                      ldc.i4     0
                      ldelem     System.Object
                      stloc      V0 (a)
                L000: ldloc      V0 (a)
                L001: ininst     Jurassic.Library.FunctionInstance
                      dup
                      brtrue     L002
                      pop
                      ldarg      0
                      ldstr      ""TypeError""
                      ldstr      ""'a' is not a function""
                      ldc.i4     0
                      ldnull
                      ldnull
                      newobj     Void .ctor(Jurassic.ScriptEngine, System.String, System.String, Int32, System.String, System.String)/Jurassic.JavaScriptException
                      throw
                L002: ldnull
                      ldstr      ""f""
                      ldc.i4     2
                      ldsfld     Jurassic.Undefined Value/Jurassic.Undefined
                      ldc.i4     0
                      newarr     System.Object
                      callvirt   System.Object CallWithStackTrace(System.String, System.String, Int32, System.Object, System.Object[])/Jurassic.Library.FunctionInstance
                      pop
                      ldnull
                      ret"),
                GetFunctionIL(@"function f(a) {
                    a();
                }", "f"));
        }

        [TestMethod]
        [Ignore("Needs investigation")]
        public void FunctionCall1()
        {
            Assert.AreEqual(NormalizeText(@"
                .local [0] System.Object a
                      ldarg      4
                      ldlen
                      ldc.i4     0
                      ble        L000
                      ldarg      4
                      ldc.i4     0
                      ldelem     System.Object
                      stloc      V0 (a)
                L000: ldloc      V0 (a)
                L001: ininst     Jurassic.Library.FunctionInstance
                      dup
                      brtrue     L002
                      pop
                      ldarg      0
                      ldstr      ""TypeError""
                      ldstr      ""'a' is not a function""
                      ldc.i4     0
                      ldnull
                      ldnull
                      newobj     Void .ctor(Jurassic.ScriptEngine, System.String, System.String, Int32, System.String, System.String)/Jurassic.JavaScriptException
                      throw
                L002: ldnull
                      ldstr      ""f""
                      ldc.i4     2
                      ldsfld     Jurassic.Undefined Value/Jurassic.Undefined
                      ldc.i4     1
                      newarr     System.Object
                      dup
                      ldc.i4     0
                      ldc.i4     5
                      box        System.Int32
                      stelem     System.Object
                      callvirt   System.Object CallWithStackTrace(System.String, System.String, Int32, System.Object, System.Object[])/Jurassic.Library.FunctionInstance
                      pop
                      ldnull
                      ret"),
                GetFunctionIL(@"function f(a) {
                    a(5);
                }", "f"));
        }

        [TestMethod]
        [Ignore("Needs investigation")]
        public void ForLoop()
        {
            Assert.AreEqual(NormalizeText(@"
                .local [0] System.Object i
                .local [1] System.Int32
                      ldc.i4     0
                      dup
                      box        System.Int32
                      stloc      V0 (i)
                L000: pop
                      ldloc      V0 (i)
                L001: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
                      ldc.i4     10
                      conv.u4
                      clt
                      brfalse    L013
                L002: ldloc      V0 (i)
                L003: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
                      dup
                      ldc.r8     1
                      add
                      box        System.Double
                      stloc      V0 (i)
                L004: pop
                      ldloc      V0 (i)
                L005: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
                      stloc      V1
                .try
                {
                    L006: ldloc      V1
                    L007: ldc.i4     10
                          clt
                          brfalse    L011
                    L008: ldloc      V1
                    L009: dup
                          ldc.i4     1
                          add
                          stloc      V1
                    L010: pop
                          br             
                }
                .finally
                {
                    L011: ldloc      V1
                    L012: box        System.Int32
                          stloc      V0 (i)
                }
                L013: ldnull
                      ret
                "),
                GetFunctionIL(@"function f() {
                    for (var i = 0; i < 10; i ++)
                        ;
                }", "f"));
        }

        [TestMethod]
        [Ignore("Needs investigation")]
        public void GetGlobalVariable()
        {
            Assert.AreEqual(NormalizeText(@"
                .local [0] System.Object
                .local [1] System.Int32
                .local [2] Jurassic.Library.ObjectInstance
                .local [3] System.Object returnValue
                      ldarg      1
                      castclass  Jurassic.Compiler.ObjectScope
                      callvirt   Jurassic.Library.ObjectInstance get_ScopeObject()/Jurassic.Compiler.ObjectScope
                      stloc      V2
                      ldloc      V0
                      ldloc      V2
                      callvirt   System.Object get_InlineCacheKey()/Jurassic.Library.ObjectInstance
                      beq        L000
                      ldloc      V2
                      ldstr      ""x""
                      ldloca     V1
                      ldloca     V0
                      callvirt   System.Object InlineGetPropertyValue(System.String, Int32 ByRef, System.Object ByRef)/Jurassic.Library.ObjectInstance
                      br         L001
                L000: ldloc      V2
                      callvirt   System.Object[] get_InlinePropertyValues()/Jurassic.Library.ObjectInstance
                      ldloc      V1
                      ldelem     System.Object
                L001: dup
                      brtrue     L002
                      ldarg      0
                      ldstr      ""ReferenceError""
                      ldstr      ""x is not defined""
                      ldc.i4     2
                      ldnull
                      ldstr      ""f""
                      newobj     Void .ctor(Jurassic.ScriptEngine, System.String, System.String, Int32, System.String, System.String)/Jurassic.JavaScriptException
                      throw
                L002: stloc      V3 (returnValue)
                      ldloc      V3 (returnValue)
                      ret"),
                GetFunctionIL(@"function f() {
                    return x;
                }", "f"));
        }

        [TestMethod]
        [Ignore("Needs investigation")]
        public void ReturnInsideFor()
        {
            Assert.AreEqual(NormalizeText(@"
                .local [0] System.Object i
                .local [1] System.Object returnValue
                .local [2] System.Int32
                      ldc.i4     0
                      dup
                      box        System.Int32
                      stloc      V0 (i)
                L000: pop
                      ldloc      V0 (i)
                L001: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
                      ldc.i4     10
                      conv.u4
                      clt
                      brfalse    L013
                      ldc.i4     1
                      box        System.Int32
                      stloc      V1 (returnValue)
                      br         L013
                L002: ldloc      V0 (i)
                L003: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
                      dup
                      ldc.r8     1
                      add
                      box        System.Double
                      stloc      V0 (i)
                L004: pop
                      ldloc      V0 (i)
                L005: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
                      stloc      V2
                .try
                {
                    L006: ldloc      V2
                    L007: ldc.i4     10
                          clt
                          brfalse    L011
                          ldc.i4     1
                          box        System.Int32
                          stloc      V1 (returnValue)
                          leave      L013
                    L008: ldloc      V2
                    L009: dup
                          ldc.i4     1
                          add
                          stloc      V2
                    L010: pop
                          br             
                }
                .finally
                {
                    L011: ldloc      V2
                    L012: box        System.Int32
                          stloc      V0 (i)
                }
                L013: ldloc      V1 (returnValue)
                      ret
                "),
                GetFunctionIL(@"function f() {
                    for (var i = 0; i < 10; i ++)
                        return 1;
                }", "f"));
        }

        [TestMethod]
        public void Let()
        {
            Assert.AreEqual(NormalizeText(@"
                      ldnull
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