using System;
using System.Collections.Generic;
using System.IO;
using Jurassic;
using Jurassic.Library;

namespace Sputnik
{
    class Program
    {
        private static object consoleLock = new object();
        private static int testsRun = 0;
        private static int testsSucceeded = 0;
        private static int testsFailed = 0;
        private static int testsNotRun = 0;

        static void Main(string[] args)
        {
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            // Create an array of bugged tests.
            var buggedTests = new string[]
            {
                "7.8.4-1-s",            // Bug in test - http://es5conform.codeplex.com/workitem/28578
                "12.14-5",              // Bug in test - http://es5conform.codeplex.com/workitem/28580
                "15.12.2-0-3",          // precondition does not return true - http://es5conform.codeplex.com/workitem/28581
                "15.12.3-0-3",          // precondition does not return true - http://es5conform.codeplex.com/workitem/28581
                "15.2.3.3-4-188",       // assumes Function.prototype.name does not exist - http://es5conform.codeplex.com/workitem/28594
                "11.4.1-5-1-s",         // assumes delete var produces ReferenceError - http://es5conform.codeplex.com/workitem/29084
                "11.4.1-5-2-s",         // assumes delete var produces ReferenceError - http://es5conform.codeplex.com/workitem/29084
                "11.4.1-5-3-s",         // assumes delete var produces ReferenceError - http://es5conform.codeplex.com/workitem/29084
                "15.4.4.21-9-c-ii-4-s", // asserts null should be passed to the callback function - http://es5conform.codeplex.com/workitem/29085
                "15.4.4.22-9-c-ii-4-s", // asserts null should be passed to the callback function - http://es5conform.codeplex.com/workitem/29085
                "11.13.1-4-2-s",        // gets global object incorrectly - http://es5conform.codeplex.com/workitem/29087
                "11.13.1-4-27-s",       // gets global object incorrectly - http://es5conform.codeplex.com/workitem/29087
                "11.13.1-4-3-s",        // gets global object incorrectly - http://es5conform.codeplex.com/workitem/29087
                "11.13.1-4-4-s",        // gets global object incorrectly - http://es5conform.codeplex.com/workitem/29087
                "15.4.4.19-9-3",        // references undefined variable - http://es5conform.codeplex.com/workitem/29088
                "12.2.1-1-s",           // assumes EvalError should be SyntaxError - http://es5conform.codeplex.com/workitem/29089
                "12.2.1-2-s",           // assumes EvalError should be SyntaxError - http://es5conform.codeplex.com/workitem/29089
                "12.2.1-3-s",           // assumes EvalError should be SyntaxError - http://es5conform.codeplex.com/workitem/29089
                "12.2.1-4-s",           // assumes EvalError should be SyntaxError - http://es5conform.codeplex.com/workitem/29089
                "12.2.1-5-s",           // assumes EvalError should be SyntaxError - http://es5conform.codeplex.com/workitem/29089
                "12.2.1-6-s",           // assumes EvalError should be SyntaxError - http://es5conform.codeplex.com/workitem/29089
                "12.2.1-7-s",           // assumes EvalError should be SyntaxError - http://es5conform.codeplex.com/workitem/29089
                "12.2.1-8-s",           // assumes EvalError should be SyntaxError - http://es5conform.codeplex.com/workitem/29089
                "12.2.1-9-s",           // assumes EvalError should be SyntaxError - http://es5conform.codeplex.com/workitem/29089
                "12.2.1-10-s",          // assumes EvalError should be SyntaxError - http://es5conform.codeplex.com/workitem/29089
                "13.1-3-3-s",           // missing return statement - http://es5conform.codeplex.com/workitem/29100
                "13.1-3-4-s",           // missing return statement - http://es5conform.codeplex.com/workitem/29100
                "13.1-3-5-s",           // missing return statement - http://es5conform.codeplex.com/workitem/29100
                "13.1-3-6-s",           // missing return statement - http://es5conform.codeplex.com/workitem/29100
                "13.1-3-9-s",           // missing return statement - http://es5conform.codeplex.com/workitem/29100
                "13.1-3-10-s",          // missing return statement - http://es5conform.codeplex.com/workitem/29100
                "13.1-3-11-s",          // missing return statement - http://es5conform.codeplex.com/workitem/29100
                "13.1-3-12-s",          // missing return statement - http://es5conform.codeplex.com/workitem/29100
                "15.4.4.14-9.a-1",      // placeholder test - http://es5conform.codeplex.com/workitem/29102
                "15.3.2.1-11-6-s",      // missing return statement - http://es5conform.codeplex.com/workitem/29103
                "10.6-13-b-3-s",        // incorrect property check - http://es5conform.codeplex.com/workitem/29141
                "10.6-13-c-3-s",        // incorrect property check - http://es5conform.codeplex.com/workitem/29141
                "15.4.4.17-4-9",        // asserts Array.prototype.some returns -1 - http://es5conform.codeplex.com/workitem/29143
                "15.4.4.17-8-10",       // mixes up true and false - http://es5conform.codeplex.com/workitem/29144
                "15.4.4.22-9-1",        // copy and paste error - http://es5conform.codeplex.com/workitem/29146
                "15.4.4.22-9-7",        // deleted array should still be traversed - http://es5conform.codeplex.com/workitem/26872
                "11.4.1-4.a-4-s",       // assumes this refers to global object - http://es5conform.codeplex.com/workitem/29151
                "11.13.1-1-7-s",        // assumes this is undefined - http://es5conform.codeplex.com/workitem/29152
            };

            // Create an array of "won't fix" tests.
            var wontFixTests = new string[]
            {
            };

            //ExecuteTest(@"");
            //return;

            //// Determine all the file paths to execute.
            List<string> scriptPaths = new List<string>();
            EnumerateScripts(scriptPaths, @"..\..\TestCases\");

            // Execute all the tests in parallel.
            //System.Threading.Tasks.Parallel.ForEach(scriptPaths, path => ExecuteTest(path, libSourceStr));

            foreach (string path in scriptPaths)
            {
                if (Array.IndexOf(buggedTests, Path.GetFileNameWithoutExtension(path)) < 0 &&
                    Array.IndexOf(wontFixTests, Path.GetFileNameWithoutExtension(path)) < 0)
                    ExecuteTest(path);
                else
                    testsNotRun++;
            }

            Console.WriteLine("");
            Console.WriteLine("Tests run: {0}", testsRun);
            Console.WriteLine("Tests succeeded: {0}", testsSucceeded);
            Console.WriteLine("Tests failed: {0}", testsFailed);
            Console.WriteLine("Tests not run: {0}", testsNotRun);
            Console.WriteLine("Time elapsed: {0}", stopWatch.Elapsed);
        }

        public static void EnumerateScripts(List<string> allPaths, string dir)
        {
            // Execute all the javascript files.
            foreach (string filePath in Directory.EnumerateFiles(dir, "*.js"))
                allPaths.Add(filePath);

            // Recurse.
            foreach (string dirPath in Directory.EnumerateDirectories(dir))
                EnumerateScripts(allPaths, dirPath);
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

        public static void ExecuteTest(string path)
        {
            StartTest(path);

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

            // Create the compareArray, compareValues and isSubsetOf helper functions.
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
                }

                function compareValues(v1, v2)
                {
                  if (v1 === 0 && v2 === 0)
                    return 1 / v1 === 1 / v2;
                  if (v1 !== v1 && v2 !== v2)
                    return true;
                  return v1 === v2;
                }

                function isSubsetOf(aSubset, aArray) {
                  if (aArray.length < aSubset.length) {
                    return false;
                  }

                  var sortedSubset = [].concat(aSubset).sort();
                  var sortedArray = [].concat(aArray).sort();

                  nextSubsetMember:
                  for (var i = 0, j = 0; i < sortedSubset.length; i++) {
                    var v = sortedSubset[i];
                    while (j < sortedArray.length) {
                      if (compareValues(v, sortedArray[j++])) {
                        continue nextSubsetMember;
                      }
                    }

                    return false;
                  }

                  return true;
                }");

            // One test uses "window" as a synonym for the global object.
            engine.SetGlobalValue("window", engine.Global);

            try
            {
                // Execute the test file.
                engine.ExecuteFile(path, System.Text.Encoding.Default);

                // Run each test that was registered inside the file.
                foreach (var registeredTest in harness.RegisteredTests)
                {
                    // Run the precondition first, if there is one.
                    object result;
                    if (registeredTest.HasProperty("precondition"))
                    {
                        result = registeredTest.CallMemberFunction("precondition");
                        if (TypeComparer.StrictEquals(result, true) == false)
                        {
                            TestFailed(path, string.Format("Precondition for test '{0}' returned {1} (should return true)", registeredTest["id"], result));
                            return;
                        }
                    }

                    // Run the actual test.
                    result = registeredTest.CallMemberFunction("test");
                    if (TypeComparer.Equals(result, true) == false)
                    {
                        TestFailed(path, string.Format("Test '{0}' ({1}) returned {2} (should return true)", registeredTest["id"], registeredTest["description"], result));
                        return;
                    }
                }

            }
            catch (JavaScriptException ex)
            {
                TestFailed(path, ex.Message);
                return;
            }
            catch (Exception ex)
            {
                TestFailed(path, ex.Message);
                throw;
            }

            TestSucceeded(path);
        }

        public static void StartTest(string path)
        {
            //lock (consoleLock)
            //{
            //    if (Console.CursorLeft != 0)
            //        Console.WriteLine();
            //    Console.Write("Executing {0}... ", Path.GetFileName(path));
            //}
        }

        public static void TestSucceeded(string path)
        {
            lock (consoleLock)
            {
                testsRun++;
                testsSucceeded++;
                //var originalRow = Console.CursorTop;
                //var originalCol = Console.CursorLeft;
                //Console.CursorTop = row;
                //Console.CursorLeft = col;
                //Console.WriteLine("SUCCEEDED", Path.GetFileName(path));
                //Console.CursorTop = originalRow;
                //Console.CursorLeft = originalCol;
            }
        }

        public static void TestFailed(string path, string reason)
        {
            lock (consoleLock)
            {
                testsRun++;
                testsFailed++;
                //var originalRow = Console.CursorTop;
                //var originalCol = Console.CursorLeft;
                //Console.CursorTop = row;
                //Console.CursorLeft = col;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0}: FAILED", Path.GetFileName(path));
                //Console.CursorTop = originalRow;
                //Console.CursorLeft = originalCol;
                //Console.WriteLine();
                Console.WriteLine("{0}", reason);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    }
}
