using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Jurassic;

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

            // Read the contents of all the javascript files in the lib folder.
            var includes = BuildIncludes();

            // Create an array of bugged tests.
            var buggedTests = new string[]
            {
                "S7.8.4_A6.4_T1",       // Asserts "\X" should throw, which is wrong.
                "S7.8.4_A6.4_T2",       // Asserts "\X" should throw, which is wrong.
                "S7.8.4_A7.4_T1",       // Asserts "\U" should throw, which is wrong.
                "S7.8.4_A7.4_T2",       // Asserts "\U" should throw, which is wrong.
                "S7.9_A9_T3",           // "do { } while (false) true" is not valid, even though all the browsers think it is.
                "S7.9_A9_T4",           // "do { } while (false) true" is not valid, even though all the browsers think it is.
                "S12.6.4_A14_T1",       // Assumes that function f() {}.prototype is enumerable (it isn't).
                "S15.8.2.16_A7",        // Accuracy of Math functions is not guaranteed.
                "S15.8.2.18_A7",        // Accuracy of Math functions is not guaranteed.

                // Implementation_Diagnostics
                "S8.4_D2.2",            // Asserts "test"[-1] should throw but it should return undefined.
                "S11.4.3_D1.2",         // Asserts that typeof RegExp should return "object" (it should be "function").
                "S12.6.4_D1",           // Newly added properties are not guarenteed to be included in enumeration.
                "S13.2_D1.2",           // Implementations are not required to join identical function instances.
                "S13_D1_T1",            // Appears to assume that function declarations do not get moved to the top of the scope.
                "S14_D7",               // Function declarations do not and should not respect the current scope.
                "S15.5.4.11_D1.1_T1",   // Asserts that String.prototype.replace() should fail if two arguments aren't supplied.
                "S15.5.4.11_D1.1_T2",   // Asserts that String.prototype.replace() should fail if two arguments aren't supplied.
                "S15.5.4.11_D1.1_T3",   // Asserts that String.prototype.replace() should fail if two arguments aren't supplied.
                "S15.5.4.11_D1.1_T4",   // Asserts that String.prototype.replace() should fail if two arguments aren't supplied.
                "S15.7.4.5_A1.2_D02",   // Asserts that toFixed(20.1) should fail, but it shouldn't.
                "S15.7.4.5_D1.2_T01",   // Asserts that toFixed(20.1) should fail, but it shouldn't.
            };

            // Create an array of "won't fix" tests.
            var wontFixTests = new string[]
            {
                "S7.8.4_A4.3_T1",       // Forbids octal escape sequence in strings (only enabled in compatibility mode).
                "S7.8.4_A4.3_T2",       // Forbids octal escape sequence in strings (only enabled in compatibility mode).
                "S7.8.5_A1.4_T2",       // Regular expression engine needs work.
                "S7.8.5_A2.4_T2",       // Regular expression engine needs work.
                "S9.4_A3_T1",           // A rewrite of DateInstance is required to fix this one.
                "S9.9_A1",              // Asserts that for (var x in undefined) throws a TypeError (not implemented by browsers, changed in ECMAScript 5).
                "S9.9_A2",              // Asserts that for (var x in undefined) throws a TypeError (not implemented by browsers, changed in ECMAScript 5).
                "S11.1.5_A4.1",         // Asserts that keywords are not allowed in object literals (they are in ECMAScript 5).
                "S11.1.5_A4.2",         // Asserts that keywords are not allowed in object literals (they are in ECMAScript 5).
                "S11.8.2_A2.3_T1",      // Asserts relational operator should evaluate right-to-left (spec bug fixed in ECMAScript 5).
                "S11.8.3_A2.3_T1",      // Asserts relational operator should evaluate right-to-left (spec bug fixed in ECMAScript 5).
                "S15.3.4.2_A1_T1",      // Assumes (function() { }).toString() can be compiled using eval().
                "S15.3.4.3_A6_T4",      // Asserts that apply throws a TypeError if the second argument is not an array.  This was changed in ECMAScript 5.
                "S15.4.4.2_A2_T1",      // Array.prototype.toString() is generic in ECMAScript 5.
                "S15.4.4.3_A2_T1",      // Array.prototype.toLocaleString() is generic in ECMAScript 5.
                "S15.4.4.10_A3_T3",     // Arrays > 2^31 are not supported yet.
                "S15.4.4.12_A3_T3",     // Arrays > 2^31 are not supported yet.
                "S15.4.4.7_A4_T2",      // Arrays > 2^31 are not supported yet.
                "S15.4.4.7_A4_T3",      // Arrays > 2^31 are not supported yet.
                "S15.10.2.10_A2.1_T3",  // Regular expression engine needs work.
                "S15.10.2.10_A5.1_T1",  // Regular expression engine needs work.
                "S15.10.2.11_A1_T2",    // Regular expression engine needs work.
                "S15.10.2.11_A1_T3",    // Regular expression engine needs work.
                "S15.10.2.11_A1_T5",    // Regular expression engine needs work.
                "S15.10.2.11_A1_T7",    // Regular expression engine needs work.
                "S15.10.2.12_A1_T1",    // Regular expression engine needs work.
                "S15.10.2.12_A1_T2",    // Regular expression engine needs work.
                "S15.10.2.12_A2_T1",    // Regular expression engine needs work.
                "S15.10.2.12_A2_T2",    // Regular expression engine needs work.
                "S15.10.2.13_A1_T1",    // Regular expression engine needs work.
                "S15.10.2.13_A1_T2",    // Regular expression engine needs work.
                "S15.10.2.13_A1_T17",   // Regular expression engine needs work.
                "S15.10.2.13_A2_T1",    // Regular expression engine needs work.
                "S15.10.2.13_A2_T2",    // Regular expression engine needs work.
                "S15.10.2.13_A2_T8",    // Regular expression engine needs work.
                "S15.10.2.5_A1_T4",     // Regular expression engine needs work.
                "S15.10.4.1_A8_T2",     // Regular expression engine needs work.
                "S15.10.6.2_A1_T6",     // Regular expression engine needs work.
                "S15.10.6.2_A5_T3",     // Regular expression engine needs work.
                "S15.10.6_A2",          // Asserts that Object.prototype.toString.call(/abc/) === '[object Object]'.  This was changed in ECMAScript 5.
                "S15.11.1.1_A1_T1",     // Assumes that Error().message doesn't exist (spec bug fixed in ECMAScript 5).
                "S15.11.2.1_A1_T1",     // Assumes that Error().message doesn't exist (spec bug fixed in ECMAScript 5).

                // Implementation_Diagnostics
                "S15.1.2.2_D1.2",       // Forbids octal values in parseInt.  This is a de-facto standard (only enabled in compatibility mode).
            };

            // Set the DeserializationEnvironment so any JavaScriptExceptions can be serialized
            // accross the AppDomain boundary.
            ScriptEngine.DeserializationEnvironment = new ScriptEngine();

            //ExecuteTest(@"..\..\tests\Conformance\15_Native_ECMA_Script_Objects\15.1_The_Global_Object\15.1.3_URI_Handling_Function_Properties\15.1.3.1_decodeURI\S15.1.3.1_A1.10_T1.js", includes);
            //return;

            //// Determine all the file paths to execute.
            List<string> scriptPaths = new List<string>();
            EnumerateScripts(scriptPaths, @"..\..\tests\Conformance");
            //EnumerateScripts(scriptPaths, @"..\..\tests\Conformance\15_Native_ECMA_Script_Objects\15.4_Array_Objects");

            // Execute all the tests in parallel.
            //System.Threading.Tasks.Parallel.ForEach(scriptPaths, path => ExecuteTest(path, libSourceStr));

            foreach (string path in scriptPaths)
            {
                if (Array.IndexOf(buggedTests, Path.GetFileNameWithoutExtension(path)) < 0 &&
                    Array.IndexOf(wontFixTests, Path.GetFileNameWithoutExtension(path)) < 0)
                    ExecuteTest(path, includes);
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

        private static Dictionary<string, string> BuildIncludes()
        {
            var includes = new Dictionary<string, string>();
            foreach (string filePath in Directory.EnumerateFiles(@"..\..\lib"))
                includes.Add(Path.GetFileName(filePath), StripHeader(File.ReadAllText(filePath)));

            // Set up special include "environment.js" - contains time zone information.
            var environmentJS = new System.Text.StringBuilder();
            environmentJS.AppendFormat("$LocalTZ = {0};" + Environment.NewLine, TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalHours);
            var rules = TimeZoneInfo.Local.GetAdjustmentRules();
            TimeZoneInfo.TransitionTime dstStart = rules[rules.Length - 1].DaylightTransitionStart;
            TimeZoneInfo.TransitionTime dstEnd = rules[rules.Length - 1].DaylightTransitionEnd;
            environmentJS.AppendFormat("$DST_start_month = {0};" + Environment.NewLine, dstStart.Month - 1);
            environmentJS.AppendFormat("$DST_start_sunday = {0};" + Environment.NewLine, CalculateSunday(dstStart));
            environmentJS.AppendFormat("$DST_start_hour = {0};" + Environment.NewLine, dstStart.TimeOfDay.AddSeconds(-1).Hour + 1);
            environmentJS.AppendFormat("$DST_start_minutes = {0};" + Environment.NewLine, (dstStart.TimeOfDay.AddSeconds(-1).Minute + 1) % 60);
            environmentJS.AppendFormat("$DST_end_month = {0};" + Environment.NewLine, dstEnd.Month - 1);
            environmentJS.AppendFormat("$DST_end_sunday = {0};" + Environment.NewLine, CalculateSunday(dstEnd));
            environmentJS.AppendFormat("$DST_end_hour = {0};" + Environment.NewLine, dstEnd.TimeOfDay.AddSeconds(-1).Hour + 1);
            environmentJS.AppendFormat("$DST_end_minutes = {0};" + Environment.NewLine, (dstEnd.TimeOfDay.AddSeconds(-1).Minute + 1) % 60);
            includes["environment.js"] = environmentJS.ToString();

            return includes;
        }

        // Returns the number of the sunday, or "last" if it is the last sunday of the month.
        private static string CalculateSunday(TimeZoneInfo.TransitionTime transition)
        {
            if (transition.IsFixedDateRule == true)
                throw new NotSupportedException("Harness does not like fixed DST date rules.");
            if (transition.DayOfWeek != DayOfWeek.Sunday)
                throw new NotSupportedException("Harness does not like non-sunday DST date rules.");
            if (transition.Week == 5)
                return "'last'";
            return (transition.Week - 1).ToString();
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

        private static AppDomain appDomain;

        private class ScriptEngineWrapper : MarshalByRefObject
        {
            public ScriptEngineWrapper()
            {
                this.Engine = new ScriptEngine();
                this.Engine.CompatibilityMode = CompatibilityMode.ECMAScript3;
                //this.Engine.EnableDebugging = true;
            }

            public ScriptEngine Engine
            {
                get;
                private set;
            }

            public void Execute(string code, string path)
            {
                if (this.Engine.EnableDebugging == true)
                {
                    path = Path.GetTempFileName();
                    File.WriteAllText(path, code);
                }

                try
                {

                    this.Engine.Execute(new StringScriptSource(code, path));

                }
                finally
                {
                    if (this.Engine.EnableDebugging == true)
                        System.IO.File.Delete(path);
                }
            }
        }

        public static void ExecuteTest(string path, Dictionary<string, string> includes)
        {
            StartTest(path);

            if (appDomain == null)
            {
                // Create an AppDomain with limited permissions.
                var e = new System.Security.Policy.Evidence();
                e.AddHostEvidence(new System.Security.Policy.Zone(System.Security.SecurityZone.Internet));
                appDomain = AppDomain.CreateDomain(
                    "Jurassic script domain",
                    null,
                    new AppDomainSetup() { ApplicationBase = AppDomain.CurrentDomain.BaseDirectory },
                    System.Security.SecurityManager.GetStandardSandbox(e));
            }

            // Create a ScriptEngine instance inside the AppDomain.
            var engineHandle = Activator.CreateInstanceFrom(appDomain, typeof(ScriptEngineWrapper).Assembly.CodeBase, typeof(ScriptEngineWrapper).FullName);
            var engine = (ScriptEngineWrapper)engineHandle.Unwrap();
            if (System.Runtime.Remoting.RemotingServices.IsTransparentProxy(engine) == false)
                throw new InvalidOperationException("Script engine not operating within the sandbox.");

            // Read the contents of the javascript file.
            var content = StripHeader(File.ReadAllText(path));

            // Check if this is a negative test (i.e. it is expected to fail).
            bool isNegativeTest = false;
            if (content.Contains("@negative") == true)
                isNegativeTest = true;

            // Substitute the special calls.
            content = content.Replace("$ERROR", "testFailed");
            content = content.Replace("$FAIL", "testFailed");
            content = content.Replace("$PRINT", "testPrint");
            content = Regex.Replace(content, @"\$INCLUDE\(""([a-zA-Z._]+)""\);", match => includes[match.Groups[1].ToString()]);

            // Include framework.js by default.
            content = includes["framework.js"] + content;

            try
            {
                // Execute the test file.
                engine.Execute(content, path);

                if (isNegativeTest == true)
                    TestFailed(path, "Test was expected to fail");
                else
                    TestSucceeded(path);
            }
            catch (JavaScriptException ex)
            {
                if (isNegativeTest == true)
                    TestSucceeded(path);
                else
                    TestFailed(path, ex.Message);
            }
            catch (Exception ex)
            {
                TestFailed(path, ex.Message);
                throw;
            }
        }

        private static string StripHeader(string source)
        {
            while (source.StartsWith("//") == true)
                source = source.Substring(source.IndexOf('\n') + 1);
            return source;
        }

        public static void StartTest(string path)
        {
            //lock (consoleLock)
            //{
            //    if (Console.CursorLeft != 0)
            //        Console.WriteLine();
                Console.WriteLine("Executing {0}... ", Path.GetFileName(path));
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
