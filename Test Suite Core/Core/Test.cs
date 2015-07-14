using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Jurassic;
using Jurassic.Library;

namespace Jurassic.TestSuite
{
    /// <summary>
    /// Represents a single test in the test suite.
    /// </summary>
    public class Test
    {
        /// <summary>
        /// Used to indicate whether the test can be run in strict mode.
        /// </summary>
        private enum StrictMode
        {
            /// <summary>
            /// Unknown whether the test should be run in strict mode or non-strict mode.
            /// </summary>
            Unspecified,

            /// <summary>
            /// The test can only be run in non-strict mode.
            /// </summary>
            NonStrictOnly,

            /// <summary>
            /// The test can only be run in strict mode.
            /// </summary>
            StrictOnly,
        }

        private StrictMode strictMode;

        /// <summary>
        /// Creates a new test.
        /// </summary>
        /// <param name="suite"> The test suite the test is part of. </param>
        /// <param name="path"> The path of the test script within the container file. </param>
        /// <param name="fileContents"> The contents of the file that defines the test. </param>
        public Test(TestSuite suite, string path, string fileContents)
        {
            if (suite == null)
                throw new ArgumentNullException("suite");
            if (path == null)
                throw new ArgumentNullException("path");
            if (fileContents == null)
                throw new ArgumentNullException("fileContents");
            this.Suite = suite;
            this.Path = path;

            // Extract the test metadata.
            var reader = new StringReader(fileContents);
            string line;
            
            // Skip past any initial comments.
            while (true)
            {
                line = reader.ReadLine();
                if (line == null || Regex.IsMatch(line, @"^\s*(//.*)?$") == false)
                    break;
            }

            // The comment metadata is optional.
            if (Regex.IsMatch(line, @"^\s*/\*.*$"))
            {
                while (true)
                {

                    line = reader.ReadLine();
                    if (line == null || Regex.IsMatch(line, @"\*/\s*$"))
                        break;

                    // Check if the line contains a property.
                    var propertyMatch = Regex.Match(line, @"^\s*\*?\s*@(\w+)\s*(.*)$");
                    if (propertyMatch.Success)
                    {
                        var propertyName = propertyMatch.Groups[1].Value;
                        var propertyText = propertyMatch.Groups[2].Value;
                        switch (propertyName)
                        {
                            case "description":
                                this.Description = propertyText;
                                break;
                            case "noStrict":
                                if (this.strictMode == StrictMode.Unspecified)
                                    this.strictMode = StrictMode.NonStrictOnly;
                                else
                                    throw new InvalidOperationException(string.Format("Test {0} marked as 'noStrict' and 'onlyStrict'.", this.Path));
                                break;
                            case "onlyStrict":
                                if (this.strictMode == StrictMode.Unspecified)
                                    this.strictMode = StrictMode.StrictOnly;
                                else
                                    throw new InvalidOperationException(string.Format("Test {0} marked as 'noStrict' and 'onlyStrict'.", this.Path));
                                break;
                            case "negative":
                                this.IsNegativeTest = true;
                                if (string.IsNullOrEmpty(propertyText.Trim()) == false)
                                    this.NegativeErrorPattern = propertyText.Trim();
                                break;
                        }
                    }
                }
            }

            // The rest of the file is the script itself.
            this.Script = reader.ReadToEnd();
        }

        /// <summary>
        /// Gets the test suite the test is part of.
        /// </summary>
        public TestSuite Suite { get; private set; }

        /// <summary>
        /// The name of the test.
        /// </summary>
        public string Name
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(this.Path); }
        }

        /// <summary>
        /// The file path of the test.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets a description of what the test is doing.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a value that indicates whether the test can run in non-strict mode.
        /// </summary>
        public bool RunInNonStrictMode
        {
            get { return this.strictMode != StrictMode.StrictOnly; }
        }

        /// <summary>
        /// Gets a value that indicates whether the test can run in strict mode.
        /// </summary>
        public bool RunInStrictMode
        {
            get
            {
                // If neither @noStrict no @onlyStrict were specified, run only in non-strict mode
                // because too many tests fail otherwise (this behavior is the same as that of the
                // official console runner).
                return this.strictMode == StrictMode.StrictOnly;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether an exception means the test succeeded.
        /// </summary>
        public bool IsNegativeTest { get; private set; }

        /// <summary>
        /// If non-null, a regular expression that matches the name of the error that must be
        /// thrown for the test to succeed.  The name of the error is retrieved using the name
        /// property of the exception object.
        /// </summary>
        public string NegativeErrorPattern { get; private set; }

        /// <summary>
        /// The script to run to evaluate the test.
        /// </summary>
        public string Script { get; private set; }

        /// <summary>
        /// Gets or sets the failure exception.
        /// </summary>
        public Exception FailureException { get; set; }
    }
}
