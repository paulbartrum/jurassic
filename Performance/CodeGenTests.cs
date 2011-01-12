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

        [TestMethod]
        public void NSieveBits()
        {
            Assert.AreEqual(@"IL_0000: ldarg.3    
IL_0001: stloc.0    
IL_0002: ldarg.s    V_4
IL_0004: ldlen      
IL_0005: ldc.i4.0   
IL_0006: ble        IL_001f
IL_000b: ldarg.s    V_4
IL_000d: ldc.i4.0   
IL_000e: ldelem.ref 
IL_000f: stloc.3    
IL_0010: ldarg.s    V_4
IL_0012: ldlen      
IL_0013: ldc.i4.1   
IL_0014: ble        IL_001f
IL_0019: ldarg.s    V_4
IL_001b: ldc.i4.1   
IL_001c: ldelem.ref 
IL_001d: stloc.s    V_4
IL_001f: ldc.i4.0   
IL_0020: dup        
IL_0021: box        System.Int32
IL_0026: stloc.s    V_6
IL_0028: pop        
IL_0029: ldc.i4     10000
IL_002e: ldloc.s    V_4
IL_0030: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_0035: ldc.i4.s   31
IL_0037: and        
IL_0038: shl        
IL_0039: dup        
IL_003a: box        System.Int32
IL_003f: stloc.s    V_7
IL_0041: pop        
IL_0042: ldloc.s    V_7
IL_0044: ldc.i4.s   31
IL_0046: box        System.Int32
IL_004b: call       System.Object Add(System.Object, System.Object)/Jurassic.TypeUtilities
IL_0050: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_0055: ldc.i4.5   
IL_0056: ldc.i4.s   31
IL_0058: and        
IL_0059: shr        
IL_005a: dup        
IL_005b: box        System.Int32
IL_0060: stloc.s    V_8
IL_0062: pop        
IL_0063: ldc.i4.0   
IL_0064: dup        
IL_0065: box        System.Int32
IL_006a: stloc.s    V_5
IL_006c: pop        
IL_006d: ldloc.s    V_5
IL_006f: ldloc.s    V_8
IL_0071: call       Boolean LessThan(System.Object, System.Object)/Jurassic.TypeComparer
IL_0076: brfalse    IL_012f
IL_007b: ldc.r8     4294967295
IL_0084: dup        
IL_0085: box        System.Double
IL_008a: stloc.s    V_10
IL_008c: ldloc.3    
IL_008d: stloc.s    V_11
IL_008f: ldarg.0    
IL_0090: ldloc.s    V_11
IL_0092: call       Jurassic.Library.ObjectInstance ToObject(Jurassic.ScriptEngine, System.Object)/Jurassic.TypeConverter
IL_0097: ldloc.s    V_5
IL_0099: call       System.String ToString(System.Object)/Jurassic.TypeConverter
IL_009e: ldloc.s    V_10
IL_00a0: ldc.i4.0   
IL_00a1: callvirt   Void SetPropertyValue(System.String, System.Object, Boolean)/Jurassic.Library.ObjectInstance
IL_00a6: pop        
IL_00a7: ldloc.s    V_5
IL_00a9: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
IL_00ae: dup        
IL_00af: ldc.r8     1
IL_00b8: add        
IL_00b9: box        System.Double
IL_00be: stloc.s    V_5
IL_00c0: pop        
IL_00c1: ldloc.s    V_5
IL_00c3: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
IL_00c8: stloc.s    V_12
IL_00ca: ldloc.s    V_12
IL_00cc: ldloc.s    V_8
IL_00ce: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
IL_00d3: clt        
IL_00d5: brfalse    IL_0120
IL_00da: ldc.r8     4294967295
IL_00e3: dup        
IL_00e4: box        System.Double
IL_00e9: stloc.s    V_11
IL_00eb: ldloc.3    
IL_00ec: stloc.s    V_10
IL_00ee: ldarg.0    
IL_00ef: ldloc.s    V_10
IL_00f1: call       Jurassic.Library.ObjectInstance ToObject(Jurassic.ScriptEngine, System.Object)/Jurassic.TypeConverter
IL_00f6: ldloc.s    V_12
IL_00f8: box        System.Double
IL_00fd: call       System.String ToString(System.Object)/Jurassic.TypeConverter
IL_0102: ldloc.s    V_11
IL_0104: ldc.i4.0   
IL_0105: callvirt   Void SetPropertyValue(System.String, System.Object, Boolean)/Jurassic.Library.ObjectInstance
IL_010a: pop        
IL_010b: ldloc.s    V_12
IL_010d: dup        
IL_010e: ldc.r8     1
IL_0117: add        
IL_0118: stloc.s    V_12
IL_011a: pop        
IL_011b: br         IL_00ca
IL_0120: leave      IL_012f
IL_0125: ldloc.s    V_12
IL_0127: box        System.Double
IL_012c: stloc.s    V_5
IL_012e: endfinally 
IL_012f: ldc.i4.2   
IL_0130: dup        
IL_0131: box        System.Int32
IL_0136: stloc.s    V_5
IL_0138: pop        
IL_0139: ldloc.s    V_5
IL_013b: ldloc.s    V_7
IL_013d: call       Boolean LessThan(System.Object, System.Object)/Jurassic.TypeComparer
IL_0142: brfalse    IL_04a3
IL_0147: ldloc.3    
IL_0148: stloc.s    V_10
IL_014a: ldarg.0    
IL_014b: ldloc.s    V_10
IL_014d: call       Jurassic.Library.ObjectInstance ToObject(Jurassic.ScriptEngine, System.Object)/Jurassic.TypeConverter
IL_0152: ldloc.s    V_5
IL_0154: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_0159: ldc.i4.5   
IL_015a: ldc.i4.s   31
IL_015c: and        
IL_015d: shr        
IL_015e: box        System.Int32
IL_0163: call       System.String ToString(System.Object)/Jurassic.TypeConverter
IL_0168: callvirt   System.Object GetPropertyValue(System.String)/Jurassic.Library.ObjectInstance
IL_016d: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_0172: ldc.i4.1   
IL_0173: ldloc.s    V_5
IL_0175: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_017a: ldc.i4.s   31
IL_017c: and        
IL_017d: ldc.i4.s   31
IL_017f: and        
IL_0180: shl        
IL_0181: and        
IL_0182: ldc.i4.0   
IL_0183: cgt.un     
IL_0185: brfalse    IL_02c8
IL_018a: ldloc.s    V_5
IL_018c: ldloc.s    V_5
IL_018e: call       System.Object Add(System.Object, System.Object)/Jurassic.TypeUtilities
IL_0193: dup        
IL_0194: stloc.s    V_9
IL_0196: pop        
IL_0197: ldloc.s    V_9
IL_0199: ldloc.s    V_7
IL_019b: call       Boolean LessThan(System.Object, System.Object)/Jurassic.TypeComparer
IL_01a0: brfalse    IL_02ae
IL_01a5: ldloc.3    
IL_01a6: stloc.s    V_11
IL_01a8: ldarg.0    
IL_01a9: ldloc.s    V_11
IL_01ab: call       Jurassic.Library.ObjectInstance ToObject(Jurassic.ScriptEngine, System.Object)/Jurassic.TypeConverter
IL_01b0: ldloc.s    V_9
IL_01b2: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_01b7: ldc.i4.5   
IL_01b8: ldc.i4.s   31
IL_01ba: and        
IL_01bb: shr        
IL_01bc: box        System.Int32
IL_01c1: call       System.String ToString(System.Object)/Jurassic.TypeConverter
IL_01c6: callvirt   System.Object GetPropertyValue(System.String)/Jurassic.Library.ObjectInstance
IL_01cb: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_01d0: ldc.i4.1   
IL_01d1: ldloc.s    V_9
IL_01d3: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_01d8: ldc.i4.s   31
IL_01da: and        
IL_01db: ldc.i4.s   31
IL_01dd: and        
IL_01de: shl        
IL_01df: not        
IL_01e0: and        
IL_01e1: dup        
IL_01e2: box        System.Int32
IL_01e7: stloc.s    V_10
IL_01e9: ldloc.3    
IL_01ea: stloc.s    V_11
IL_01ec: ldarg.0    
IL_01ed: ldloc.s    V_11
IL_01ef: call       Jurassic.Library.ObjectInstance ToObject(Jurassic.ScriptEngine, System.Object)/Jurassic.TypeConverter
IL_01f4: ldloc.s    V_9
IL_01f6: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_01fb: ldc.i4.5   
IL_01fc: ldc.i4.s   31
IL_01fe: and        
IL_01ff: shr        
IL_0200: box        System.Int32
IL_0205: call       System.String ToString(System.Object)/Jurassic.TypeConverter
IL_020a: ldloc.s    V_10
IL_020c: ldc.i4.0   
IL_020d: callvirt   Void SetPropertyValue(System.String, System.Object, Boolean)/Jurassic.Library.ObjectInstance
IL_0212: pop        
IL_0213: ldloc.s    V_9
IL_0215: ldloc.s    V_5
IL_0217: call       System.Object Add(System.Object, System.Object)/Jurassic.TypeUtilities
IL_021c: dup        
IL_021d: stloc.s    V_9
IL_021f: pop        
IL_0220: ldloc.s    V_9
IL_0222: ldloc.s    V_7
IL_0224: call       Boolean LessThan(System.Object, System.Object)/Jurassic.TypeComparer
IL_0229: brfalse    IL_02ae
IL_022e: ldloc.3    
IL_022f: stloc.s    V_11
IL_0231: ldarg.0    
IL_0232: ldloc.s    V_11
IL_0234: call       Jurassic.Library.ObjectInstance ToObject(Jurassic.ScriptEngine, System.Object)/Jurassic.TypeConverter
IL_0239: ldloc.s    V_9
IL_023b: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_0240: ldc.i4.5   
IL_0241: ldc.i4.s   31
IL_0243: and        
IL_0244: shr        
IL_0245: box        System.Int32
IL_024a: call       System.String ToString(System.Object)/Jurassic.TypeConverter
IL_024f: callvirt   System.Object GetPropertyValue(System.String)/Jurassic.Library.ObjectInstance
IL_0254: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_0259: ldc.i4.1   
IL_025a: ldloc.s    V_9
IL_025c: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_0261: ldc.i4.s   31
IL_0263: and        
IL_0264: ldc.i4.s   31
IL_0266: and        
IL_0267: shl        
IL_0268: not        
IL_0269: and        
IL_026a: dup        
IL_026b: box        System.Int32
IL_0270: stloc.s    V_10
IL_0272: ldloc.3    
IL_0273: stloc.s    V_11
IL_0275: ldarg.0    
IL_0276: ldloc.s    V_11
IL_0278: call       Jurassic.Library.ObjectInstance ToObject(Jurassic.ScriptEngine, System.Object)/Jurassic.TypeConverter
IL_027d: ldloc.s    V_9
IL_027f: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_0284: ldc.i4.5   
IL_0285: ldc.i4.s   31
IL_0287: and        
IL_0288: shr        
IL_0289: box        System.Int32
IL_028e: call       System.String ToString(System.Object)/Jurassic.TypeConverter
IL_0293: ldloc.s    V_10
IL_0295: ldc.i4.0   
IL_0296: callvirt   Void SetPropertyValue(System.String, System.Object, Boolean)/Jurassic.Library.ObjectInstance
IL_029b: pop        
IL_029c: ldloc.s    V_9
IL_029e: ldloc.s    V_5
IL_02a0: call       System.Object Add(System.Object, System.Object)/Jurassic.TypeUtilities
IL_02a5: dup        
IL_02a6: stloc.s    V_9
IL_02a8: pop        
IL_02a9: br         IL_0220
IL_02ae: ldloc.s    V_6
IL_02b0: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
IL_02b5: dup        
IL_02b6: ldc.r8     1
IL_02bf: add        
IL_02c0: box        System.Double
IL_02c5: stloc.s    V_6
IL_02c7: pop        
IL_02c8: ldloc.s    V_5
IL_02ca: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
IL_02cf: dup        
IL_02d0: ldc.r8     1
IL_02d9: add        
IL_02da: box        System.Double
IL_02df: stloc.s    V_5
IL_02e1: pop        
IL_02e2: ldloc.s    V_5
IL_02e4: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
IL_02e9: stloc.s    V_13
IL_02eb: ldloc.s    V_13
IL_02ed: ldloc.s    V_7
IL_02ef: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
IL_02f4: clt        
IL_02f6: brfalse    IL_0494
IL_02fb: ldloc.3    
IL_02fc: stloc.s    V_11
IL_02fe: ldarg.0    
IL_02ff: ldloc.s    V_11
IL_0301: call       Jurassic.Library.ObjectInstance ToObject(Jurassic.ScriptEngine, System.Object)/Jurassic.TypeConverter
IL_0306: ldloc.s    V_13
IL_0308: conv.u4    
IL_0309: ldc.i4.5   
IL_030a: ldc.i4.s   31
IL_030c: and        
IL_030d: shr        
IL_030e: box        System.Int32
IL_0313: call       System.String ToString(System.Object)/Jurassic.TypeConverter
IL_0318: callvirt   System.Object GetPropertyValue(System.String)/Jurassic.Library.ObjectInstance
IL_031d: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_0322: ldc.i4.1   
IL_0323: ldloc.s    V_13
IL_0325: conv.u4    
IL_0326: ldc.i4.s   31
IL_0328: and        
IL_0329: ldc.i4.s   31
IL_032b: and        
IL_032c: shl        
IL_032d: and        
IL_032e: ldc.i4.0   
IL_032f: cgt.un     
IL_0331: brfalse    IL_047f
IL_0336: ldloc.s    V_13
IL_0338: ldloc.s    V_13
IL_033a: add        
IL_033b: dup        
IL_033c: box        System.Double
IL_0341: stloc.s    V_9
IL_0343: pop        
IL_0344: ldloc.s    V_9
IL_0346: ldloc.s    V_7
IL_0348: call       Boolean LessThan(System.Object, System.Object)/Jurassic.TypeComparer
IL_034d: brfalse    IL_0465
IL_0352: ldloc.3    
IL_0353: stloc.s    V_10
IL_0355: ldarg.0    
IL_0356: ldloc.s    V_10
IL_0358: call       Jurassic.Library.ObjectInstance ToObject(Jurassic.ScriptEngine, System.Object)/Jurassic.TypeConverter
IL_035d: ldloc.s    V_9
IL_035f: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_0364: ldc.i4.5   
IL_0365: ldc.i4.s   31
IL_0367: and        
IL_0368: shr        
IL_0369: box        System.Int32
IL_036e: call       System.String ToString(System.Object)/Jurassic.TypeConverter
IL_0373: callvirt   System.Object GetPropertyValue(System.String)/Jurassic.Library.ObjectInstance
IL_0378: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_037d: ldc.i4.1   
IL_037e: ldloc.s    V_9
IL_0380: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_0385: ldc.i4.s   31
IL_0387: and        
IL_0388: ldc.i4.s   31
IL_038a: and        
IL_038b: shl        
IL_038c: not        
IL_038d: and        
IL_038e: dup        
IL_038f: box        System.Int32
IL_0394: stloc.s    V_11
IL_0396: ldloc.3    
IL_0397: stloc.s    V_10
IL_0399: ldarg.0    
IL_039a: ldloc.s    V_10
IL_039c: call       Jurassic.Library.ObjectInstance ToObject(Jurassic.ScriptEngine, System.Object)/Jurassic.TypeConverter
IL_03a1: ldloc.s    V_9
IL_03a3: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_03a8: ldc.i4.5   
IL_03a9: ldc.i4.s   31
IL_03ab: and        
IL_03ac: shr        
IL_03ad: box        System.Int32
IL_03b2: call       System.String ToString(System.Object)/Jurassic.TypeConverter
IL_03b7: ldloc.s    V_11
IL_03b9: ldc.i4.0   
IL_03ba: callvirt   Void SetPropertyValue(System.String, System.Object, Boolean)/Jurassic.Library.ObjectInstance
IL_03bf: pop        
IL_03c0: ldloc.s    V_9
IL_03c2: ldloc.s    V_13
IL_03c4: box        System.Double
IL_03c9: call       System.Object Add(System.Object, System.Object)/Jurassic.TypeUtilities
IL_03ce: dup        
IL_03cf: stloc.s    V_9
IL_03d1: pop        
IL_03d2: ldloc.s    V_9
IL_03d4: ldloc.s    V_7
IL_03d6: call       Boolean LessThan(System.Object, System.Object)/Jurassic.TypeComparer
IL_03db: brfalse    IL_0465
IL_03e0: ldloc.3    
IL_03e1: stloc.s    V_10
IL_03e3: ldarg.0    
IL_03e4: ldloc.s    V_10
IL_03e6: call       Jurassic.Library.ObjectInstance ToObject(Jurassic.ScriptEngine, System.Object)/Jurassic.TypeConverter
IL_03eb: ldloc.s    V_9
IL_03ed: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_03f2: ldc.i4.5   
IL_03f3: ldc.i4.s   31
IL_03f5: and        
IL_03f6: shr        
IL_03f7: box        System.Int32
IL_03fc: call       System.String ToString(System.Object)/Jurassic.TypeConverter
IL_0401: callvirt   System.Object GetPropertyValue(System.String)/Jurassic.Library.ObjectInstance
IL_0406: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_040b: ldc.i4.1   
IL_040c: ldloc.s    V_9
IL_040e: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_0413: ldc.i4.s   31
IL_0415: and        
IL_0416: ldc.i4.s   31
IL_0418: and        
IL_0419: shl        
IL_041a: not        
IL_041b: and        
IL_041c: dup        
IL_041d: box        System.Int32
IL_0422: stloc.s    V_11
IL_0424: ldloc.3    
IL_0425: stloc.s    V_10
IL_0427: ldarg.0    
IL_0428: ldloc.s    V_10
IL_042a: call       Jurassic.Library.ObjectInstance ToObject(Jurassic.ScriptEngine, System.Object)/Jurassic.TypeConverter
IL_042f: ldloc.s    V_9
IL_0431: call       Int32 ToInt32(System.Object)/Jurassic.TypeConverter
IL_0436: ldc.i4.5   
IL_0437: ldc.i4.s   31
IL_0439: and        
IL_043a: shr        
IL_043b: box        System.Int32
IL_0440: call       System.String ToString(System.Object)/Jurassic.TypeConverter
IL_0445: ldloc.s    V_11
IL_0447: ldc.i4.0   
IL_0448: callvirt   Void SetPropertyValue(System.String, System.Object, Boolean)/Jurassic.Library.ObjectInstance
IL_044d: pop        
IL_044e: ldloc.s    V_9
IL_0450: ldloc.s    V_13
IL_0452: box        System.Double
IL_0457: call       System.Object Add(System.Object, System.Object)/Jurassic.TypeUtilities
IL_045c: dup        
IL_045d: stloc.s    V_9
IL_045f: pop        
IL_0460: br         IL_03d2
IL_0465: ldloc.s    V_6
IL_0467: call       Double ToNumber(System.Object)/Jurassic.TypeConverter
IL_046c: dup        
IL_046d: ldc.r8     1
IL_0476: add        
IL_0477: box        System.Double
IL_047c: stloc.s    V_6
IL_047e: pop        
IL_047f: ldloc.s    V_13
IL_0481: dup        
IL_0482: ldc.r8     1
IL_048b: add        
IL_048c: stloc.s    V_13
IL_048e: pop        
IL_048f: br         IL_02eb
IL_0494: leave      IL_04a3
IL_0499: ldloc.s    V_13
IL_049b: box        System.Double
IL_04a0: stloc.s    V_5
IL_04a2: endfinally 
IL_04a3: ldnull     
IL_04a4: ret        
",
                GetFunctionIL(@"
                    function primes(isPrime, n) {
                      var i, count = 0, m = 10000<<n, size = m+31>>5;

                      for (i=0; i<size; i++) isPrime[i] = 0xffffffff;

                      for (i=2; i<m; i++)
                        if (isPrime[i>>5] & 1<<(i&31)) {
                          for (var j=i+i; j<m; j+=i)
                            isPrime[j>>5] &= ~(1<<(j&31));
                          count++;
                        }
                    }", "primes"));
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