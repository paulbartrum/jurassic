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
            Assert.AreEqual(TestUtils.NormalizeText(@"
                      ldnull
                      ret
                "),
                GetFunctionIL(@"function f() {
                }", "f"));
        }

        [TestMethod]
        public void SimpleReturn()
        {
            Assert.AreEqual(TestUtils.NormalizeText(@"
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
        public void FunctionCall0()
        {
            Assert.AreEqual(TestUtils.NormalizeText(@"
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
                      newobj     Void .ctor(Jurassic.ScriptEngine, System.String, System.String)/Jurassic.JavaScriptException
                      throw
                L002: ldsfld     Jurassic.Undefined Value/Jurassic.Undefined
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
            Assert.AreEqual(TestUtils.NormalizeText(@"
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
                      newobj     Void .ctor(Jurassic.ScriptEngine, System.String, System.String)/Jurassic.JavaScriptException
                      throw
                L002: ldsfld     Jurassic.Undefined Value/Jurassic.Undefined
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
            Assert.AreEqual(TestUtils.NormalizeText(@"
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
                      brfalse    L012
                      ldloc      V0 (i)
                L002: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
                      dup
                      ldc.r8     1
                      add
                      box        System.Double
                      stloc      V0 (i)
                L003: pop
                      ldloc      V0 (i)
                L004: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
                      stloc      V1
                .try
                {
                    L005: ldloc      V1
                    L006: ldc.i4     10
                          clt
                          brfalse    L010
                    L007: ldloc      V1
                    L008: dup
                          ldc.i4     1
                          add
                          stloc      V1
                    L009: pop
                          br             
                }
                .finally
                {
                    L010: ldloc      V1
                    L011: box        System.Int32
                          stloc      V0 (i)
                }
                L012: ldnull
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
            Assert.AreEqual(TestUtils.NormalizeText(@"
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
                      newobj     Void .ctor(Jurassic.ScriptEngine, System.String, System.String)/Jurassic.JavaScriptException
                      throw
                L002: stloc      V3 (returnValue)
                      ldloc      V3 (returnValue)
                      ret
                "),
                GetFunctionIL(@"function f() {
                    return x;
                }", "f"));
        }

        [TestMethod]
        public void ReturnInsideFor()
        {
            Assert.AreEqual(TestUtils.NormalizeText(@"
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
                      brfalse    L012
                      ldc.i4     1
                      box        System.Int32
                      stloc      V1 (returnValue)
                      br         L012
                      ldloc      V0 (i)
                L002: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
                      dup
                      ldc.r8     1
                      add
                      box        System.Double
                      stloc      V0 (i)
                L003: pop
                      ldloc      V0 (i)
                L004: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
                      stloc      V2
                .try
                {
                    L005: ldloc      V2
                    L006: ldc.i4     10
                          clt
                          brfalse    L010
                          ldc.i4     1
                          box        System.Int32
                          stloc      V1 (returnValue)
                          br         L012
                    L007: ldloc      V2
                    L008: dup
                          ldc.i4     1
                          add
                          stloc      V2
                    L009: pop
                          br             
                }
                .finally
                {
                    L010: ldloc      V2
                    L011: box        System.Int32
                          stloc      V0 (i)
                }
                L012: ldloc      V1 (returnValue)
                      ret
                "),
                GetFunctionIL(@"function f() {
                    for (var i = 0; i < 10; i ++)
                        return 1;
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
            return TestUtils.NormalizeText(function.DisassembledIL);
        }
    }

}