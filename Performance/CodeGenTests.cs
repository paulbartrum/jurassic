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
IL_0016: brfalse    IL_0062
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
IL_0053: leave      IL_0062
IL_0058: ldloc.s    V_4
IL_005a: box        System.Int32
IL_005f: stloc.s    V_5
IL_0061: endfinally 
IL_0062: ldnull     
IL_0063: ret        
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
IL_000d: ldstr      ""x""
IL_0012: callvirt   System.Object GetPropertyValue(System.String)/Jurassic.Library.ObjectInstance
IL_0017: dup        
IL_0018: brtrue     IL_002e
IL_001d: ldarg.0    
IL_001e: ldstr      ""ReferenceError""
IL_0023: ldstr      ""x is not defined""
IL_0028: newobj     Void .ctor(Jurassic.ScriptEngine, System.String, System.String)/Jurassic.JavaScriptException
IL_002d: throw      
IL_002e: stloc.3    
IL_002f: ldloc.3    
IL_0030: ret        
",
                GetFunctionIL(@"function f() {
                    return x;
                }", "f"));
        }

        [TestMethod]
        public void GetGlobalVariableInForLoop()
        {
            Assert.AreEqual(@"IL_0000: ldarg.3    
IL_0001: stloc.0    
IL_0002: ldc.i4.0   
IL_0003: dup        
IL_0004: box        System.Int32
IL_0009: stloc.3    
IL_000a: pop        
IL_000b: ldc.i4.0   
IL_000c: dup        
IL_000d: box        System.Int32
IL_0012: stloc.s    V_4
IL_0014: pop        
IL_0015: ldloc.s    V_4
IL_0017: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
IL_001c: ldc.i4.s   10
IL_001e: conv.r8    
IL_001f: clt        
IL_0021: brfalse    IL_00ce
IL_0026: ldarg.1    
IL_0027: castclass  Jurassic.Compiler.ObjectScope
IL_002c: callvirt   Jurassic.Library.ObjectInstance get_ScopeObject()/Jurassic.Compiler.ObjectScope
IL_0031: ldstr      ""y""
IL_0036: callvirt   System.Object GetPropertyValue(System.String)/Jurassic.Library.ObjectInstance
IL_003b: dup        
IL_003c: brtrue     IL_0052
IL_0041: ldarg.0    
IL_0042: ldstr      ""ReferenceError""
IL_0047: ldstr      ""y is not defined""
IL_004c: newobj     Void .ctor(Jurassic.ScriptEngine, System.String, System.String)/Jurassic.JavaScriptException
IL_0051: throw      
IL_0052: dup        
IL_0053: stloc.3    
IL_0054: pop        
IL_0055: ldloc.s    V_4
IL_0057: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
IL_005c: dup        
IL_005d: ldc.r8     1
IL_0066: add        
IL_0067: box        System.Double
IL_006c: stloc.s    V_4
IL_006e: pop        
IL_006f: ldloc.s    V_4
IL_0071: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_0076: stloc.s    V_5
IL_0078: ldloc.s    V_5
IL_007a: ldc.i4.s   10
IL_007c: clt        
IL_007e: brfalse    IL_00bf
IL_0083: ldarg.1    
IL_0084: castclass  Jurassic.Compiler.ObjectScope
IL_0089: callvirt   Jurassic.Library.ObjectInstance get_ScopeObject()/Jurassic.Compiler.ObjectScope
IL_008e: ldstr      ""y""
IL_0093: callvirt   System.Object GetPropertyValue(System.String)/Jurassic.Library.ObjectInstance
IL_0098: dup        
IL_0099: brtrue     IL_00af
IL_009e: ldarg.0    
IL_009f: ldstr      ""ReferenceError""
IL_00a4: ldstr      ""y is not defined""
IL_00a9: newobj     Void .ctor(Jurassic.ScriptEngine, System.String, System.String)/Jurassic.JavaScriptException
IL_00ae: throw      
IL_00af: dup        
IL_00b0: stloc.3    
IL_00b1: pop        
IL_00b2: ldloc.s    V_5
IL_00b4: dup        
IL_00b5: ldc.i4.1   
IL_00b6: add        
IL_00b7: stloc.s    V_5
IL_00b9: pop        
IL_00ba: br         IL_0078
IL_00bf: leave      IL_00ce
IL_00c4: ldloc.s    V_5
IL_00c6: box        System.Int32
IL_00cb: stloc.s    V_6
IL_00cd: endfinally 
IL_00ce: ldnull     
IL_00cf: ret        
",
                GetFunctionIL(@"function f() {
                    var x = 0;
                    for (var i = 0; i < 10; i ++)
                        x = y;
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