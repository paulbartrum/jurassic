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
            Assert.AreEqual(@"IL_0000: ldarg.3    
IL_0001: stloc.1    
IL_0002: ldc.i4.0   
IL_0003: dup        
IL_0004: box        System.Int32
IL_0009: stloc.s    V_4
IL_000b: pop        
IL_000c: ldc.i4.0   
IL_000d: dup        
IL_000e: box        System.Int32
IL_0013: stloc.s    V_5
IL_0015: pop        
IL_0016: ldloc.s    V_5
IL_0018: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
IL_001d: ldc.i4     100000
IL_0022: conv.r8    
IL_0023: clt        
IL_0025: brfalse    IL_00a7
IL_002a: ldloc.s    V_4
IL_002c: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_0031: ldloc.s    V_5
IL_0033: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_0038: and        
IL_0039: dup        
IL_003a: box        System.Int32
IL_003f: stloc.s    V_4
IL_0041: pop        
IL_0042: ldloc.s    V_5
IL_0044: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
IL_0049: dup        
IL_004a: ldc.r8     1
IL_0053: add        
IL_0054: box        System.Double
IL_0059: stloc.s    V_5
IL_005b: pop        
IL_005c: ldloc.s    V_5
IL_005e: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
IL_0063: ldc.i4     100000
IL_0068: conv.r8    
IL_0069: clt        
IL_006b: brfalse    IL_00a7
IL_0070: ldloc.s    V_4
IL_0072: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_0077: ldloc.s    V_5
IL_0079: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_007e: and        
IL_007f: dup        
IL_0080: box        System.Int32
IL_0085: stloc.s    V_4
IL_0087: pop        
IL_0088: ldloc.s    V_5
IL_008a: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
IL_008f: dup        
IL_0090: ldc.r8     1
IL_0099: add        
IL_009a: box        System.Double
IL_009f: stloc.s    V_5
IL_00a1: pop        
IL_00a2: br         IL_005c
IL_00a7: ldloc.0    
IL_00a8: ret        
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