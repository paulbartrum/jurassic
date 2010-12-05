using System;
using System.Collections.Generic;
using System.IO;
using Jurassic;
using Jurassic.Library;

namespace Test_Suite_Runner
{

    /// <summary>
    /// Represents the sputnik test suite (http://code.google.com/p/sputniktests/).
    /// </summary>
    public class SputnikTestSuite : TestSuite
    {
        private HashSet<string> buggyTests;
        private HashSet<string> wontFixTests;
        private Dictionary<string, string> includes;

        /// <summary>
        /// Creates a new ES5ConformTestSuite instance.
        /// </summary>
        public SputnikTestSuite()
            : base(@"sputnik\Conformance")
        {
            // Create an array of buggy tests.
            this.buggyTests = new HashSet<string>
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
            this.wontFixTests = new HashSet<string>
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

                "S11.6.2_A4_T7",        // 1 / Number.MAX_VALUE - 1 / Number.MAX_VALUE = +0  Actual: -0

                "S12.1_A1",             // Function declarations are technically not be allowed except in a top-level context.
                "S12.5_A9_T1",          // Function declarations are technically not be allowed except in a top-level context.
                "S12.5_A9_T2",          // Function declarations are technically not be allowed except in a top-level context.
                "S12.6.1_A13_T1",       // Function declarations are technically not be allowed except in a top-level context.
                "S12.6.1_A13_T2",       // Function declarations are technically not be allowed except in a top-level context.
                "S12.6.2_A13_T1",       // Function declarations are technically not be allowed except in a top-level context.
                "S12.6.2_A13_T2",       // Function declarations are technically not be allowed except in a top-level context.
                "S12.6.4_A13_T1",       // Function declarations are technically not be allowed except in a top-level context.
                "S12.6.4_A13_T2",       // Function declarations are technically not be allowed except in a top-level context.

                // Implementation_Diagnostics
                "S15.1.2.2_D1.2",       // Forbids octal values in parseInt.  This is a de-facto standard (only enabled in compatibility mode).
            };
        }

        /// <summary>
        /// Gets the name of the test suite.
        /// </summary>
        public override string Name
        {
            get { return "Sputnik"; }
        }

        /// <summary>
        /// Gets the compatibility mode the tests were designed for.
        /// </summary>
        public override CompatibilityMode CompatibilityMode
        {
            get { return CompatibilityMode.ECMAScript3; }
        }

        /// <summary>
        /// Gets the text encoding of the script files in the test suite.
        /// </summary>
        public override System.Text.Encoding ScriptEncoding
        {
            get { return System.Text.Encoding.UTF8; }
        }

        /// <summary>
        /// Determines whether to skip a test.
        /// </summary>
        /// <param name="test"> The test to check. </param>
        /// <returns> <c>true</c> if the test should be skipped; <c>false</c> otherwise. </returns>
        public override bool SkipTest(Test test)
        {
            return this.buggyTests.Contains(test.Name) || this.wontFixTests.Contains(test.Name);
        }

        /// <summary>
        /// Transforms the test script before executing it.
        /// </summary>
        /// <param name="test"> Details about the test. </param>
        /// <param name="scriptContents"> The contents of the test file. </param>
        /// <returns> The new contents of the test file. </returns>
        public override string TransformTestScript(Test test, string scriptContents)
        {
            // Load the include files (if they haven't already been loaded).
            if (includes == null)
                includes = BuildIncludes(Path.Combine(this.BaseDirectory, @"..\lib"));

            // Strip the header from the start of the script file.
            scriptContents = StripHeader(scriptContents);

            // Check if this is a negative test (i.e. it is expected to fail).
            if (scriptContents.Contains("@negative") == true)
                test.IsNegativeTest = true;

            // Substitute the special calls.
            scriptContents = scriptContents.Replace("$ERROR", "testFailed");
            scriptContents = scriptContents.Replace("$FAIL", "testFailed");
            scriptContents = scriptContents.Replace("$PRINT", "testPrint");
            scriptContents = System.Text.RegularExpressions.Regex.Replace(scriptContents, @"\$INCLUDE\(""([a-zA-Z._]+)""\);", match => includes[match.Groups[1].ToString()]);

            // Include framework.js by default.
            scriptContents = includes["framework.js"] + scriptContents;

            return scriptContents;
        }





        private static string StripHeader(string source)
        {
            while (source.StartsWith("//") == true)
                source = source.Substring(source.IndexOf('\n') + 1);
            return source;
        }

        private static Dictionary<string, string> BuildIncludes(string dir)
        {
            var includes = new Dictionary<string, string>();
            foreach (string filePath in Directory.EnumerateFiles(dir))
                includes.Add(Path.GetFileName(filePath), StripHeader(File.ReadAllText(filePath)));

            // Set up special include "environment.js" - contains time zone information.
            var environmentJS = new System.Text.StringBuilder();
            environmentJS.AppendFormat("$LocalTZ = {0};" + Environment.NewLine, TimeZoneInfo.Local.BaseUtcOffset.TotalHours);
            var rules = TimeZoneInfo.Local.GetAdjustmentRules();
            if (rules.Length > 0)
            {
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
            }
            else
            {
                // No daylight savings.
                environmentJS.Append("$DST_start_month = 0;" + Environment.NewLine);
                environmentJS.Append("$DST_start_sunday = 1;" + Environment.NewLine);
                environmentJS.Append("$DST_start_hour = 0;" + Environment.NewLine);
                environmentJS.Append("$DST_start_minutes = 0;" + Environment.NewLine);
                environmentJS.Append("$DST_end_month = 0;" + Environment.NewLine);
                environmentJS.Append("$DST_end_sunday = 1;" + Environment.NewLine);
                environmentJS.Append("$DST_end_hour = 0;" + Environment.NewLine);
                environmentJS.Append("$DST_end_minutes = 0;" + Environment.NewLine);
            }
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
    }
}
