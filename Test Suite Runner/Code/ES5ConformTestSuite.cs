using System;
using System.Collections.Generic;
using System.IO;
using Jurassic;
using Jurassic.Library;

namespace Test_Suite_Runner
{

    /// <summary>
    /// Represents the es5conform test suite (http://es5conform.codeplex.com).
    /// </summary>
    public class ES5ConformTestSuite : TestSuite
    {
        private HashSet<string> buggyTests;
        private HashSet<string> wontFixTests;

        /// <summary>
        /// Creates a new ES5ConformTestSuite instance.
        /// </summary>
        public ES5ConformTestSuite()
            : base("es5conform")
        {
            // Create an array of buggy tests.
            this.buggyTests = new HashSet<string>
            {
                //"7.8.4-1-s",            // Bug in test - http://es5conform.codeplex.com/workitem/28578
                //"10.6-13-b-3-s",        // incorrect property check - http://es5conform.codeplex.com/workitem/29141
                //"10.6-13-c-3-s",        // incorrect property check - http://es5conform.codeplex.com/workitem/29141
                "11.4.1-4.a-4-s",       // assumes this refers to global object - http://es5conform.codeplex.com/workitem/29151
                "11.4.1-5-1-s",         // assumes delete var produces ReferenceError - http://es5conform.codeplex.com/workitem/29084
                "11.4.1-5-2-s",         // assumes delete var produces ReferenceError - http://es5conform.codeplex.com/workitem/29084
                "11.4.1-5-3-s",         // assumes delete var produces ReferenceError - http://es5conform.codeplex.com/workitem/29084
                "11.13.1-1-7-s",        // assumes this is undefined - http://es5conform.codeplex.com/workitem/29152
                "11.13.1-4-2-s",        // gets global object incorrectly - http://es5conform.codeplex.com/workitem/29087
                "11.13.1-4-27-s",       // gets global object incorrectly - http://es5conform.codeplex.com/workitem/29087
                "11.13.1-4-3-s",        // gets global object incorrectly - http://es5conform.codeplex.com/workitem/29087
                "11.13.1-4-4-s",        // gets global object incorrectly - http://es5conform.codeplex.com/workitem/29087
                //"12.2.1-1-s",           // assumes EvalError should be SyntaxError - http://es5conform.codeplex.com/workitem/29089
                //"12.2.1-2-s",           // assumes EvalError should be SyntaxError - http://es5conform.codeplex.com/workitem/29089
                //"12.2.1-3-s",           // assumes EvalError should be SyntaxError - http://es5conform.codeplex.com/workitem/29089
                //"12.2.1-4-s",           // assumes EvalError should be SyntaxError - http://es5conform.codeplex.com/workitem/29089
                //"12.2.1-5-s",           // assumes EvalError should be SyntaxError - http://es5conform.codeplex.com/workitem/29089
                //"12.2.1-6-s",           // assumes EvalError should be SyntaxError - http://es5conform.codeplex.com/workitem/29089
                //"12.2.1-7-s",           // assumes EvalError should be SyntaxError - http://es5conform.codeplex.com/workitem/29089
                //"12.2.1-8-s",           // assumes EvalError should be SyntaxError - http://es5conform.codeplex.com/workitem/29089
                //"12.2.1-9-s",           // assumes EvalError should be SyntaxError - http://es5conform.codeplex.com/workitem/29089
                //"12.2.1-10-s",          // assumes EvalError should be SyntaxError - http://es5conform.codeplex.com/workitem/29089
                //"12.14-5",              // Bug in test - http://es5conform.codeplex.com/workitem/28580
                //"13.1-3-3-s",           // missing return statement - http://es5conform.codeplex.com/workitem/29100
                //"13.1-3-4-s",           // missing return statement - http://es5conform.codeplex.com/workitem/29100
                //"13.1-3-5-s",           // missing return statement - http://es5conform.codeplex.com/workitem/29100
                //"13.1-3-6-s",           // missing return statement - http://es5conform.codeplex.com/workitem/29100
                //"13.1-3-9-s",           // missing return statement - http://es5conform.codeplex.com/workitem/29100
                //"13.1-3-10-s",          // missing return statement - http://es5conform.codeplex.com/workitem/29100
                //"13.1-3-11-s",          // missing return statement - http://es5conform.codeplex.com/workitem/29100
                //"13.1-3-12-s",          // missing return statement - http://es5conform.codeplex.com/workitem/29100
                "15.2.3.3-4-188",       // assumes Function.prototype.name does not exist - http://es5conform.codeplex.com/workitem/28594
                //"15.3.2.1-11-6-s",      // missing return statement - http://es5conform.codeplex.com/workitem/29103
                "15.4.4.14-9.a-1",      // placeholder test - http://es5conform.codeplex.com/workitem/29102
                //"15.4.4.17-4-9",        // asserts Array.prototype.some returns -1 - http://es5conform.codeplex.com/workitem/29143
                //"15.4.4.17-8-10",       // mixes up true and false - http://es5conform.codeplex.com/workitem/29144
                //"15.4.4.19-9-3",        // references undefined variable - http://es5conform.codeplex.com/workitem/29088
                //"15.4.4.21-9-c-ii-4-s", // asserts null should be passed to the callback function - http://es5conform.codeplex.com/workitem/29085
                //"15.4.4.22-9-c-ii-4-s", // asserts null should be passed to the callback function - http://es5conform.codeplex.com/workitem/29085
                //"15.4.4.22-9-1",        // copy and paste error - http://es5conform.codeplex.com/workitem/29146
                //"15.4.4.22-9-7",        // deleted array should still be traversed - http://es5conform.codeplex.com/workitem/26872
                //"15.12.2-0-3",          // precondition does not return true - http://es5conform.codeplex.com/workitem/28581
                //"15.12.3-0-3",          // precondition does not return true - http://es5conform.codeplex.com/workitem/28581
            };

            // Create an array of "won't fix" tests.
            this.wontFixTests = new HashSet<string>
            {
                "15.4.4.14-9-9",        // Array sizes of >= 2^31 are not currently supported.
                "15.4.4.15-8-9",        // Array sizes of >= 2^31 are not currently supported.
            };
        }

        /// <summary>
        /// Gets the name of the test suite.
        /// </summary>
        public override string Name
        {
            get { return "ECMAScript 5 Conformance Suite"; }
        }

        /// <summary>
        /// Gets the text encoding of the script files in the test suite.
        /// </summary>
        public override System.Text.Encoding ScriptEncoding
        {
            get { return System.Text.Encoding.Default; }
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
            return @"// Create the ES5Harness.registerTest function.
                var _registeredTests = [];
                ES5Harness = {};
                ES5Harness.registerTest = function(test) { _registeredTests.push(test) };

                ES5Harness.runTests = function()
                {
                    for (var i = 0; i < _registeredTests.length; i ++)
                    {
                        if (_registeredTests[i].precondition)
                        {
                            if (_registeredTests[i].precondition() !== true)
                                throw new Error('Precondition for test ' + _registeredTests[i].id + ' failed');
                        }
                        if (_registeredTests[i].test() != true)
                            throw new Error('Test ' + _registeredTests[i].id + ' failed');
                    }
                }

                // Create the fnExists helper function.
                function fnExists() {
                    for (var i=0; i<arguments.length; i++) {
                        if (typeof(arguments[i]) !== ""function"") return false;
                    }
                    return true;
                }

                // Create the fnSupportsStrict helper function.
                function fnSupportsStrict() { return true; }

                // Create the fnGlobalObject helper function.
                function fnGlobalObject() { return (function () {return this}).call(null); }

                // Create the compareArray, compareValues and isSubsetOf helper functions.
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
                }

                // One test uses 'window' as a synonym for the global object.
                window = this" +
                Environment.NewLine +
                scriptContents +
                Environment.NewLine +
                "ES5Harness.runTests();";
        }
    }
}
