using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;

namespace Performance
{

#if DEBUG

    /// <summary>
    /// Test the code-gen for short code snippets.
    /// </summary>
    [TestClass]
    public class CodeGenTests
    {
        [TestMethod]
        public void EmptyFunction()
        {
            Assert.AreEqual(
@"IL_0000: ldarg.3    
IL_0001: stloc.0    
IL_0002: ldnull     
IL_0003: ret        
",
                GetFunctionIL(@"function f() {
                }", "f"));
        }

        [TestMethod]
        public void SimpleReturn()
        {
            Assert.AreEqual(
@"IL_0000: ldarg.3    
IL_0001: stloc.0    
IL_0002: ldc.i4.5   
IL_0003: box        System.Int32
IL_0008: stloc.3    
IL_0009: ldloc.3    
IL_000a: ret        
",
                GetFunctionIL(@"function f() {
                    return 5;
                }", "f"));
        }

        [TestMethod]
        public void ForLoop()
        {
            Assert.AreEqual(
@"IL_0000: ldarg.3    
IL_0001: stloc.0    
IL_0002: ldc.i4.0   
IL_0003: dup        
IL_0004: box        System.Int32
IL_0009: stloc.3    
IL_000a: pop        
IL_000b: ldloc.3    
IL_000c: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
IL_0011: ldc.i4.s   10
IL_0013: conv.r8    
IL_0014: clt        
IL_0016: brfalse    IL_0061
IL_001b: ldloc.3    
IL_001c: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
IL_0021: dup        
IL_0022: ldc.r8     1
IL_002b: add        
IL_002c: box        System.Double
IL_0031: stloc.3    
IL_0032: pop        
IL_0033: ldloc.3    
IL_0034: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_0039: stloc.s    V_4
IL_003b: ldloc.s    V_4
IL_003d: ldc.i4.s   10
IL_003f: clt        
IL_0041: brfalse    IL_0053
IL_0046: ldloc.s    V_4
IL_0048: dup        
IL_0049: ldc.i4.1   
IL_004a: add        
IL_004b: stloc.s    V_4
IL_004d: pop        
IL_004e: br         IL_003b
IL_0053: leave      IL_0061
IL_0058: ldloc.s    V_4
IL_005a: box        System.Int32
IL_005f: stloc.3    
IL_0060: endfinally 
IL_0061: ldnull     
IL_0062: ret        
",
                GetFunctionIL(@"function f() {
                    for (var i = 0; i < 10; i ++)
                        ;
                }", "f"));
        }

        [TestMethod]
        public void GetGlobalVariable()
        {
            Assert.AreEqual(@"IL_0000: ldarg.3    
IL_0001: stloc.0    
IL_0002: ldarg.1    
IL_0003: castclass  Jurassic.Compiler.ObjectScope
IL_0008: callvirt   Jurassic.Library.ObjectInstance get_ScopeObject()/Jurassic.Compiler.ObjectScope
IL_000d: stloc.s    V_5
IL_000f: ldloc.3    
IL_0010: ldloc.s    V_5
IL_0012: callvirt   System.Object get_InlineCacheKey()/Jurassic.Library.ObjectInstance
IL_0017: beq        IL_0031
IL_001c: ldloc.s    V_5
IL_001e: ldstr      ""x""
IL_0023: ldloca.s   V_4
IL_0025: ldloca.s   V_3
IL_0027: callvirt   System.Object InlineGetPropertyValue(System.String, Int32 ByRef, System.Object ByRef)/Jurassic.Library.ObjectInstance
IL_002c: br         IL_003b
IL_0031: ldloc.s    V_5
IL_0033: callvirt   System.Object[] get_InlinePropertyValues()/Jurassic.Library.ObjectInstance
IL_0038: ldloc.s    V_4
IL_003a: ldelem.ref 
IL_003b: dup        
IL_003c: brtrue     IL_0052
IL_0041: ldarg.0    
IL_0042: ldstr      ""ReferenceError""
IL_0047: ldstr      ""x is not defined""
IL_004c: newobj     Void .ctor(Jurassic.ScriptEngine, System.String, System.String)/Jurassic.JavaScriptException
IL_0051: throw      
IL_0052: stloc.s    V_6
IL_0054: ldloc.s    V_6
IL_0056: ret        
",
                GetFunctionIL(@"function f() {
                    return x;
                }", "f"));
        }



        private static string GetFunctionIL(string code, string functionName)
        {
            var scriptEngine = new ScriptEngine();
            scriptEngine.Execute(code);
            var function = (Jurassic.Library.UserDefinedFunction)scriptEngine.GetGlobalValue(functionName);
            if (function == null)
                throw new ArgumentException(string.Format("The function {0} was not found.", functionName));
            return function.DisassembledIL;
        }
    }

#endif

}