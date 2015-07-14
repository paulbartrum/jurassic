using System;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace UnitTests
{
    [TestClass]
    public class DynamicCodeDebugger
    {

        //private string Disassemble(string source)
        //{
        //    // Compile the expression tree.
        //    var expressionFunc = Compile(source);

        //    var reader = ClrTest.Reflection.ILReaderFactory.Create(expressionFunc.Method);
        //    var writer = new System.IO.StringWriter();
        //    var visitor = new ClrTest.Reflection.ReadableILStringVisitor(new ClrTest.Reflection.ReadableILStringToTextWriter(writer));
        //    reader.Accept(visitor);
        //    return writer.ToString();
        //}

        //[TestMethod]
        //public void Save()
        //{
        //    // create a dynamic assembly and module 
        //    AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Debug"),
        //        AssemblyBuilderAccess.Save, @"..\..\..\", false, null);
        //    ModuleBuilder module = assemblyBuilder.DefineDynamicModule("Module", "Debug.dll");

        //    // create a new type to hold our Main method
        //    TypeBuilder typeBuilder = module.DefineType("TestClass", TypeAttributes.Public | TypeAttributes.Class);

        //    // create the Main(string[] args) method
        //    MethodBuilder methodBuilder = typeBuilder.DefineMethod("TestMethod", MethodAttributes.HideBySig | MethodAttributes.Static | MethodAttributes.Public,
        //        typeof(object), new Type[] { typeof(object), typeof(object[]) });

        //    //var method = FunctionBinder.CreateSingleMethodBinder(
        //    //    new Type[] { typeof(string) }, new FunctionBinderMethod(
        //    //        ReflectionHelpers.GetInstanceMethod(typeof(RegExpInstance), "compile", typeof(string), typeof(string))));
        //    //var reader = ClrTest.Reflection.ILReaderFactory.Create(method.Method);

        //    //var ilArray = reader.ILProvider.GetByteArray();
        //    //methodBuilder.CreateMethodBody(ilArray, ilArray.Length);

        //    // generate the IL for the Main method
        //    ILGenerator ilGenerator = methodBuilder.GetILGenerator();
        //    FunctionBinder.CreateSingleMethodBinder(new Type[] { typeof(string) }, new FunctionBinderMethod(
        //            ReflectionHelpers.GetStaticMethod(typeof(ObjectConstructor), "getPrototypeOf", typeof(ObjectInstance))), ilGenerator);

        //    // bake it
        //    Type helloWorldType = typeBuilder.CreateType();

        //    // set the entry point for the application and save it
        //    assemblyBuilder.Save(@"Debug.dll");

        //    System.Diagnostics.Process.Start(@"C:\Program Files\Reflector\Reflector.exe", string.Format("\"{0}\" /select:TestClass",
        //        System.IO.Path.Combine(@"..\..\..\", @"Debug.dll")));
        //}

    }
}
