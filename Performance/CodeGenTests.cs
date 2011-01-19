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
            Assert.AreEqual(NormalizeILWhitespace(@"
                      ldarg      3
                      stloc      V0 (f)
                L000: ldnull
                      ret
                "),
                GetFunctionIL(@"function f() {
                }", "f"));
        }

        [TestMethod]
        public void SimpleReturn()
        {
            Assert.AreEqual(NormalizeILWhitespace(@"
                      ldarg      3
                      stloc      V0 (f)
                L000: ldc.i4     5
                      box        System.Int32
                      stloc      V3 (returnValue)
                      ldloc      V3 (returnValue)
                      ret
                "),
                GetFunctionIL(@"function f() {
                    return 5;
                }", "f"));
        }

        [TestMethod]
        public void FunctionCall0()
        {
            Assert.AreEqual(NormalizeILWhitespace(@"
                      ldarg      3
                      stloc      V0 (f)
                L000: ldarg      4
                      ldlen
                      ldc.i4     0
                      ble        L001
                      ldarg      4
                      ldc.i4     0
                      ldelem     System.Object
                      stloc      V3 (a)
                L001: ldloc      V3 (a)
                L002: ininst     Jurassic.Library.FunctionInstance
                      dup
                      brtrue     L003
                      pop
                      ldarg      0
                      ldstr      ""TypeError""
                      ldstr      ""'a' is not a function""
                      newobj     Void .ctor(Jurassic.ScriptEngine, System.String, System.String)/Jurassic.JavaScriptException
                      throw
                L003: ldsfld     Jurassic.Undefined Value/Jurassic.Undefined
                      ldc.i4     0
                      newarr     System.Object
                      callvirt   System.Object CallLateBound(System.Object, System.Object[])/Jurassic.Library.FunctionInstance
                      pop
                      ldnull
                      ret
                "),
                GetFunctionIL(@"function f(a) {
                    a();
                }", "f"));
        }

        [TestMethod]
        public void FunctionCall1()
        {
            Assert.AreEqual(NormalizeILWhitespace(@"
                      ldarg      3
                      stloc      V0 (f)
                L000: ldarg      4
                      ldlen
                      ldc.i4     0
                      ble        L001
                      ldarg      4
                      ldc.i4     0
                      ldelem     System.Object
                      stloc      V3 (a)
                L001: ldloc      V3 (a)
                L002: ininst     Jurassic.Library.FunctionInstance
                      dup
                      brtrue     L003
                      pop
                      ldarg      0
                      ldstr      ""TypeError""
                      ldstr      ""'a' is not a function""
                      newobj     Void .ctor(Jurassic.ScriptEngine, System.String, System.String)/Jurassic.JavaScriptException
                      throw
                L003: ldsfld     Jurassic.Undefined Value/Jurassic.Undefined
                      ldc.i4     1
                      newarr     System.Object
                      dup
                      ldc.i4     0
                      ldc.i4     5
                      box        System.Int32
                      stelem     System.Object
                      callvirt   System.Object CallLateBound(System.Object, System.Object[])/Jurassic.Library.FunctionInstance
                      pop
                      ldnull
                      ret
                "),
                GetFunctionIL(@"function f(a) {
                    a(5);
                }", "f"));
        }

        [TestMethod]
        public void ForLoop()
        {
            Assert.AreEqual(NormalizeILWhitespace(@"
                      ldarg      3
                      stloc      V0 (f)
                L000: ldc.i4     0
                      dup
                      box        System.Int32
                      stloc      V3 (i)
                L001: pop
                      ldloc      V3 (i)
                L002: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
                      ldc.i4     10
                      conv.u4
                      clt
                      brfalse    L013
                      ldloc      V3 (i)
                L003: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
                      dup
                      ldc.r8     1
                      add
                      box        System.Double
                      stloc      V3 (i)
                L004: pop
                      ldloc      V3 (i)
                L005: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
                      stloc      V4
                L006: ldloc      V4
                L007: ldc.i4     10
                      clt
                      brfalse    L011
                L008: ldloc      V4
                L009: dup
                      ldc.i4     1
                      add
                      stloc      V4
                L010: pop
                      br             
                L011: ldloc      V4
                L012: box        System.Int32
                      stloc      V3 (i)
                L013: ldnull
                      ret
                "),
                GetFunctionIL(@"function f() {
                    for (var i = 0; i < 10; i ++)
                        ;
                }", "f"));
        }

        [TestMethod]
        public void GetGlobalVariable()
        {
            Assert.AreEqual(NormalizeILWhitespace(@"
                      ldarg      3
                      stloc      V0 (f)
                L000: ldarg      1
                      castclass  Jurassic.Compiler.ObjectScope
                      callvirt   Jurassic.Library.ObjectInstance get_ScopeObject()/Jurassic.Compiler.ObjectScope
                      stloc      V5
                      ldloc      V3
                      ldloc      V5
                      callvirt   System.Object get_InlineCacheKey()/Jurassic.Library.ObjectInstance
                      beq        L001
                      ldloc      V5
                      ldstr      ""x""
                      ldloca     V4
                      ldloca     V3
                      callvirt   System.Object InlineGetPropertyValue(System.String, Int32 ByRef, System.Object ByRef)/Jurassic.Library.ObjectInstance
                      br         L002
                L001: ldloc      V5
                      callvirt   System.Object[] get_InlinePropertyValues()/Jurassic.Library.ObjectInstance
                      ldloc      V4
                      ldelem     System.Object
                L002: dup
                      brtrue     L003
                      ldarg      0
                      ldstr      ""ReferenceError""
                      ldstr      ""x is not defined""
                      newobj     Void .ctor(Jurassic.ScriptEngine, System.String, System.String)/Jurassic.JavaScriptException
                      throw
                L003: stloc      V6 (returnValue)
                      ldloc      V6 (returnValue)
                      ret"),
                GetFunctionIL(@"function f() {
                    return x;
                }", "f"));
        }

        private static string NormalizeILWhitespace(string text)
        {
            // Remove excess spaces and carriage returns at the start or end.
            return text.Replace("                ", "").Trim('\r', '\n');
        }

        private static string GetFunctionIL(string code, string functionName)
        {
            var scriptEngine = new ScriptEngine();
            scriptEngine.EnableILAnalysis = true;
            scriptEngine.Execute(code);
            var function = (Jurassic.Library.UserDefinedFunction)scriptEngine.GetGlobalValue(functionName);
            if (function == null)
                throw new ArgumentException(string.Format("The function {0} was not found.", functionName));
            return NormalizeILWhitespace(function.DisassembledIL);
        }
    }

}