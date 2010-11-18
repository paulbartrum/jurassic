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
        public void BitwiseAnd()
        {
            Assert.AreEqual(@"IL_0000: ldarg.1    
IL_0001: ldc.i4.5   
IL_0002: newarr     System.String
IL_0007: dup        
IL_0008: ldc.i4.0   
IL_0009: ldstr      ""f""
IL_000e: stelem.ref 
IL_000f: dup        
IL_0010: ldc.i4.1   
IL_0011: ldstr      ""this""
IL_0016: stelem.ref 
IL_0017: dup        
IL_0018: ldc.i4.2   
IL_0019: ldstr      ""arguments""
IL_001e: stelem.ref 
IL_001f: dup        
IL_0020: ldc.i4.3   
IL_0021: ldstr      ""x""
IL_0026: stelem.ref 
IL_0027: dup        
IL_0028: ldc.i4.4   
IL_0029: ldstr      ""i""
IL_002e: stelem.ref 
IL_002f: call       Jurassic.Compiler.DeclarativeScope CreateRuntimeScope(Jurassic.Compiler.Scope, System.String[])/Jurassic.Compiler.DeclarativeScope
IL_0034: starg.s    V_1
IL_0036: ldarg.2    
IL_0037: ldnull     
IL_0038: ceq        
IL_003a: ldarg.2    
IL_003b: ldsfld     Jurassic.Null Value/Jurassic.Null
IL_0040: ceq        
IL_0042: or         
IL_0043: ldarg.2    
IL_0044: ldsfld     Jurassic.Undefined Value/Jurassic.Undefined
IL_0049: ceq        
IL_004b: or         
IL_004c: brfalse    IL_005c
IL_0051: ldarg.0    
IL_0052: callvirt   Jurassic.Library.GlobalObject get_Global()/Jurassic.ScriptEngine
IL_0057: br         IL_0065
IL_005c: ldarg.2    
IL_005d: stloc.1    
IL_005e: ldarg.0    
IL_005f: ldloc.1    
IL_0060: call       Jurassic.Library.ObjectInstance ToObject(Jurassic.ScriptEngine, System.Object)/Jurassic.TypeConverter
IL_0065: starg.s    V_2
IL_0067: ldarg.3    
IL_0068: stloc.1    
IL_0069: ldarg.1    
IL_006a: castclass  Jurassic.Compiler.DeclarativeScope
IL_006f: callvirt   System.Object[] get_Values()/Jurassic.Compiler.DeclarativeScope
IL_0074: ldc.i4.0   
IL_0075: ldloc.1    
IL_0076: stelem.ref 
IL_0077: ldc.i4.0   
IL_0078: dup        
IL_0079: box        System.Int32
IL_007e: stloc.1    
IL_007f: ldarg.1    
IL_0080: castclass  Jurassic.Compiler.DeclarativeScope
IL_0085: callvirt   System.Object[] get_Values()/Jurassic.Compiler.DeclarativeScope
IL_008a: ldc.i4.3   
IL_008b: ldloc.1    
IL_008c: stelem.ref 
IL_008d: pop        
IL_008e: ldc.i4.0   
IL_008f: dup        
IL_0090: box        System.Int32
IL_0095: stloc.1    
IL_0096: ldarg.1    
IL_0097: castclass  Jurassic.Compiler.DeclarativeScope
IL_009c: callvirt   System.Object[] get_Values()/Jurassic.Compiler.DeclarativeScope
IL_00a1: ldc.i4.4   
IL_00a2: ldloc.1    
IL_00a3: stelem.ref 
IL_00a4: pop        
IL_00a5: ldarg.1    
IL_00a6: castclass  Jurassic.Compiler.DeclarativeScope
IL_00ab: callvirt   System.Object[] get_Values()/Jurassic.Compiler.DeclarativeScope
IL_00b0: ldc.i4.4   
IL_00b1: ldelem.ref 
IL_00b2: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
IL_00b7: ldc.i4     100000
IL_00bc: conv.r8    
IL_00bd: clt        
IL_00bf: brfalse    IL_01c2
IL_00c4: ldarg.1    
IL_00c5: castclass  Jurassic.Compiler.DeclarativeScope
IL_00ca: callvirt   System.Object[] get_Values()/Jurassic.Compiler.DeclarativeScope
IL_00cf: ldc.i4.3   
IL_00d0: ldelem.ref 
IL_00d1: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_00d6: ldarg.1    
IL_00d7: castclass  Jurassic.Compiler.DeclarativeScope
IL_00dc: callvirt   System.Object[] get_Values()/Jurassic.Compiler.DeclarativeScope
IL_00e1: ldc.i4.4   
IL_00e2: ldelem.ref 
IL_00e3: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_00e8: and        
IL_00e9: dup        
IL_00ea: box        System.Int32
IL_00ef: stloc.1    
IL_00f0: ldarg.1    
IL_00f1: castclass  Jurassic.Compiler.DeclarativeScope
IL_00f6: callvirt   System.Object[] get_Values()/Jurassic.Compiler.DeclarativeScope
IL_00fb: ldc.i4.3   
IL_00fc: ldloc.1    
IL_00fd: stelem.ref 
IL_00fe: pop        
IL_00ff: ldarg.1    
IL_0100: castclass  Jurassic.Compiler.DeclarativeScope
IL_0105: callvirt   System.Object[] get_Values()/Jurassic.Compiler.DeclarativeScope
IL_010a: ldc.i4.4   
IL_010b: ldelem.ref 
IL_010c: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
IL_0111: dup        
IL_0112: ldc.r8     1
IL_011b: add        
IL_011c: box        System.Double
IL_0121: stloc.1    
IL_0122: ldarg.1    
IL_0123: castclass  Jurassic.Compiler.DeclarativeScope
IL_0128: callvirt   System.Object[] get_Values()/Jurassic.Compiler.DeclarativeScope
IL_012d: ldc.i4.4   
IL_012e: ldloc.1    
IL_012f: stelem.ref 
IL_0130: pop        
IL_0131: ldarg.1    
IL_0132: castclass  Jurassic.Compiler.DeclarativeScope
IL_0137: callvirt   System.Object[] get_Values()/Jurassic.Compiler.DeclarativeScope
IL_013c: ldc.i4.4   
IL_013d: ldelem.ref 
IL_013e: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
IL_0143: ldc.i4     100000
IL_0148: conv.r8    
IL_0149: clt        
IL_014b: brfalse    IL_01c2
IL_0150: ldarg.1    
IL_0151: castclass  Jurassic.Compiler.DeclarativeScope
IL_0156: callvirt   System.Object[] get_Values()/Jurassic.Compiler.DeclarativeScope
IL_015b: ldc.i4.3   
IL_015c: ldelem.ref 
IL_015d: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_0162: ldarg.1    
IL_0163: castclass  Jurassic.Compiler.DeclarativeScope
IL_0168: callvirt   System.Object[] get_Values()/Jurassic.Compiler.DeclarativeScope
IL_016d: ldc.i4.4   
IL_016e: ldelem.ref 
IL_016f: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_0174: and        
IL_0175: dup        
IL_0176: box        System.Int32
IL_017b: stloc.1    
IL_017c: ldarg.1    
IL_017d: castclass  Jurassic.Compiler.DeclarativeScope
IL_0182: callvirt   System.Object[] get_Values()/Jurassic.Compiler.DeclarativeScope
IL_0187: ldc.i4.3   
IL_0188: ldloc.1    
IL_0189: stelem.ref 
IL_018a: pop        
IL_018b: ldarg.1    
IL_018c: castclass  Jurassic.Compiler.DeclarativeScope
IL_0191: callvirt   System.Object[] get_Values()/Jurassic.Compiler.DeclarativeScope
IL_0196: ldc.i4.4   
IL_0197: ldelem.ref 
IL_0198: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
IL_019d: dup        
IL_019e: ldc.r8     1
IL_01a7: add        
IL_01a8: box        System.Double
IL_01ad: stloc.1    
IL_01ae: ldarg.1    
IL_01af: castclass  Jurassic.Compiler.DeclarativeScope
IL_01b4: callvirt   System.Object[] get_Values()/Jurassic.Compiler.DeclarativeScope
IL_01b9: ldc.i4.4   
IL_01ba: ldloc.1    
IL_01bb: stelem.ref 
IL_01bc: pop        
IL_01bd: br         IL_0131
IL_01c2: ldloc.0    
IL_01c3: ret        
",
                GetIL(@"function f() {
                    var x = 0;
                    for (var i = 0; i < 100000; i++)
                        x = x & i
                }", "f"));
        }

        private static string GetIL(string code, string functionName)
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