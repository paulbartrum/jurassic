using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace Performance
{

    /// <summary>
    /// Runs the IE Test Center javascript tests (available here: http://es5conform.codeplex.com).
    /// </summary>
    [TestClass]
    public class IETestCenter
    {
        // Chapter 7

        [TestMethod]
        public void IETestCenter_Literals()
        {
            RunTests(@"chapter07\7.8",
                "7.8.4-1-s" // Bug in test - http://es5conform.codeplex.com/workitem/28578
                );
        }


        // Chapter 8

        [TestMethod]
        public void IETestCenter_Reference()
        {
            RunTests(@"chapter08\8.7");
        }


        // Chapter 10

        [TestMethod]
        public void IETestCenter_Execution()
        {
            RunTests(@"chapter10\10.4");
        }

        [TestMethod]
        public void IETestCenter_Argument()
        {
            RunTests(@"chapter10\10.6");
        }


        // Chapter 11

        [TestMethod]
        public void IETestCenter_Primary()
        {
            RunTests(@"chapter11\11.1");
        }

        [TestMethod]
        public void IETestCenter_Unary()
        {
            RunTests(@"chapter11\11.4");
        }

        [TestMethod]
        public void IETestCenter_Assignment()
        {
            RunTests(@"chapter11\11.13");
        }


        // Chapter 12

        [TestMethod]
        public void IETestCenter_Try()
        {
            RunTests(@"chapter12\12.14",
                "12.14-5"   // Bug in test - http://es5conform.codeplex.com/workitem/28580
                );
        }

        [TestMethod]
        public void IETestCenter_With()
        {
            RunTests(@"chapter12\12.10");
        }

        [TestMethod]
        public void IETestCenter_Var()
        {
            RunTests(@"chapter12\12.2");
        }


        // Chapter 13

        [TestMethod]
        public void IETestCenter_StrictMode()
        {
            RunTests(@"chapter13\13.1");
        }
        

        // Chapter 14

        [TestMethod]
        public void IETestCenter_Directive()
        {
            RunTests(@"chapter14\14.1");
        }


        // Chapter 15

        [TestMethod]
        public void IETestCenter_Array()
        {
            RunTests(@"chapter15\15.4");
        }

        [TestMethod]
        public void IETestCenter_Date()
        {
            RunTests(@"chapter15\15.9");
        }

        [TestMethod]
        public void IETestCenter_Function()
        {
            RunTests(@"chapter15\15.3");
        }

        [TestMethod]
        public void IETestCenter_Global()
        {
            RunTests(@"chapter15\15.1");
        }

        [TestMethod]
        public void IETestCenter_JSON()
        {
            RunTests(@"chapter15\15.12",
                "15.12.2-0-3",      // precondition does not return true - http://es5conform.codeplex.com/workitem/28581
                "15.12.3-0-3"       // precondition does not return true - http://es5conform.codeplex.com/workitem/28581
                );
        }

        [TestMethod]
        public void IETestCenter_Number()
        {
            RunTests(@"chapter15\15.7");
        }

        [TestMethod]
        public void IETestCenter_Object()
        {
            RunTests(@"chapter15\15.2",
                "15.2.3.3-4-188",   // assumes Function.prototype.name does not exist - http://es5conform.codeplex.com/workitem/28594
                "15.2.3.4-4-1",     // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-2",     // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-3",     // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-4",     // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-5",     // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-6",     // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-7",     // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-8",     // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-9",     // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-10",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-11",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-12",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-13",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-14",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-15",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-16",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-17",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-18",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-19",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-20",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-21",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-22",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-23",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-24",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-25",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-26",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-27",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-28",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-29",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-30",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-31",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-32",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-33",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-34",    // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                "15.2.3.4-4-35"     // assumes fixed list of properties on global object - http://es5conform.codeplex.com/workitem/28595
                );
        }

        [TestMethod]
        public void IETestCenter_RegExp()
        {
            RunTests(@"chapter15\15.10");
        }

        [TestMethod]
        public void IETestCenter_String()
        {
            RunTests(@"chapter15\15.5");
        }



        private void RunTests(string path, params string[] suppressTests)
        {
            if (Path.IsPathRooted(path) == false)
                path = Path.GetFullPath(Path.Combine(@"..\..\..\Unit Tests\IETestCenter\", path));
            foreach (string filePath in Directory.EnumerateFiles(path))
                RunTestFile(filePath, suppressTests);
            foreach (string dirPath in Directory.EnumerateDirectories(path))
                RunTests(dirPath, suppressTests);
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
                function fnExists() {
                    for (var i=0; i<arguments.length; i++) {
                        if (typeof(arguments[i]) !== ""function"") return false;
                    }
                    return true;
                }");

            // Create the fnSupportsStrict helper function.
            engine.Execute(@"function fnSupportsStrict() { return true; }");

            // Create the fnGlobalObject helper function.
            engine.Execute(@"function fnGlobalObject() { return (function () {return this}).call(null); }");

            // Create the compareArray helper function.
            engine.Execute(@"
                function compareArray(aExpected, aActual) {
                  if (aActual.length != aExpected.length) {
                    return false;
                  }

                  aExpected.sort();
                  aActual.sort();

                  var s;
                  for (var i = 0; i < aExpected.length; i++) {
                    if (aActual[i] !== aExpected[i]) {
                      return false;
                    }
                  }
  
                  return true;
                }");

            // One test uses "window" as a synonym for the global object.
            engine.SetGlobalValue("window", engine.Global);

            // Check if the test is suppressed.
            if (suppressTests != null && System.Array.IndexOf(suppressTests, Path.GetFileNameWithoutExtension(path)) >= 0)
            {
                Console.WriteLine("Suppressed test file '{0}'", path);
                return;
            }

            try
            {
                // Run the script file to register the test.
                engine.ExecuteFile(path, System.Text.Encoding.Default);
            }
            catch (Exception ex)
            {
                throw new FormatException(string.Format("Failed to run script '{0}'", path), ex);
            }

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
                if (TypeComparer.Equals(result, true) == false)
                    Assert.Fail("Test '{0}' ({1}) returned {2} (should return true)", registeredTest["id"], registeredTest["description"], result);
            }
        }
    }

}