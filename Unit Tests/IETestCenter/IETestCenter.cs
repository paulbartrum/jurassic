using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace Performance
{

    /// <summary>
    /// Runs the IE Test Center javascript tests.
    /// </summary>
    [TestClass]
    public class IETestCenter
    {
        // Chapter 10

        [TestMethod]
        public void IETestCenter_Execution()
        {
            RunTests("Execution");
        }

        [TestMethod]
        public void IETestCenter_Argument()
        {
            RunTests("Argument");
        }


        // Chapter 11

        [TestMethod]
        public void IETestCenter_Primary()
        {
            RunTests("Primary");
        }

        [TestMethod]
        public void IETestCenter_Unary()
        {
            RunTests("Unary");
        }

        [TestMethod]
        public void IETestCenter_Assignment()
        {
            RunTests("Assignment");
        }


        // Chapter 12

        [TestMethod]
        public void IETestCenter_Try()
        {
            RunTests("Try");
        }

        [TestMethod]
        public void IETestCenter_With()
        {
            RunTests("With");
        }

        [TestMethod]
        public void IETestCenter_Var()
        {
            RunTests("Var");
        }


        // Chapter 15

        [TestMethod]
        public void IETestCenter_Array()
        {
            RunTests("Array", new string[] { "15.4.4.14-1-16", "15.4.4.14-2-16" });
        }

        [TestMethod]
        public void IETestCenter_Date()
        {
            RunTests("Date");
        }

        [TestMethod]
        public void IETestCenter_Function()
        {
            RunTests("Function");
        }

        [TestMethod]
        public void IETestCenter_JSON()
        {
            RunTests("JSON");
        }

        [TestMethod]
        public void IETestCenter_Number()
        {
            RunTests("Number");
        }

        [TestMethod]
        public void IETestCenter_Object()
        {
            RunTests("Object");
        }

        [TestMethod]
        public void IETestCenter_RegExp()
        {
            RunTests("RegExp");
        }

        [TestMethod]
        public void IETestCenter_String()
        {
            RunTests("String");
        }



        private void RunTests(string folderName, string[] suppressTests = null)
        {
            foreach (string path in Directory.EnumerateFiles(Path.Combine(@"..\..\..\Unit Tests\IETestCenter\", folderName)))
                RunTestFile(path, suppressTests);
        }

        public class ES5Harness : ObjectInstance
        {
            private List<ObjectInstance> registeredTests;

            public ES5Harness(ScriptEngine engine)
                : base(engine)
            {
                this.PopulateFunctions();
                this.registeredTests = new List<ObjectInstance>();
            }

            public IEnumerable<ObjectInstance> RegisteredTests
            {
                get { return this.registeredTests; }
            }

            [JSFunction(Name = "registerTest")]
            public void RegisterTest(ObjectInstance obj)
            {
                this.registeredTests.Add(obj);
            }
        }

        private void RunTestFile(string path, string[] suppressTests)
        {
            var engine = new ScriptEngine();

            // Create a new ES5Harness object (used to register tests).
            var harness = new ES5Harness(engine);
            engine.SetGlobalValue("ES5Harness", harness);

            // Create the fnExists helper function.
            engine.Execute(@"
                function fnExists(f) {
                  if (typeof(f) === 'function') {
                    return true;
                  }
                }");

            // One test uses "window" as a synonym for the global object.
            engine.SetGlobalValue("window", engine.Global);

            // Run the script file to register the test.
            engine.ExecuteFile(path);

            foreach (var registeredTest in harness.RegisteredTests)
            {
                // Check if the test is suppressed.
                if (suppressTests != null && System.Array.IndexOf(suppressTests, registeredTest["id"]) >= 0)
                {
                    Console.WriteLine("Suppressed test '{0}' ({1})", registeredTest["id"], registeredTest["description"]);
                    continue;
                }

                Console.WriteLine("Running test '{0}' ({1})...", registeredTest["id"], registeredTest["description"]);

                // Run the precondition first, if there is one.
                object result;
                if (registeredTest.HasProperty("precondition"))
                {
                    result = registeredTest.CallMemberFunction("precondition");
                    if (TypeComparer.StrictEquals(result, true) == false)
                        Assert.Fail("Precondition for test '{0}' returned {1} (should return true)", registeredTest["id"], result);
                }

                // Run the actual test.
                result = registeredTest.CallMemberFunction("test");
                if (TypeComparer.StrictEquals(result, true) == false)
                    Assert.Fail("Test '{0}' ({1}) returned {2} (should return true)", registeredTest["id"], registeredTest["description"], result);
            }
        }
    }

}