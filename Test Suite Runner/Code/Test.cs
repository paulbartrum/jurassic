using System;
using System.Collections.Generic;
using System.IO;
using Jurassic;
using Jurassic.Library;

namespace Test_Suite_Runner
{

    /// <summary>
    /// Represents a single test in a test suite.
    /// </summary>
    public class Test
    {
        private TestSuite suite;
        private string path;

        /// <summary>
        /// Creates a new test.
        /// </summary>
        /// <param name="suite"> The test suite the test is part of. </param>
        /// <param name="path"> The file name of the test script. </param>
        public Test(TestSuite suite, string path)
        {
            if (suite == null)
                throw new ArgumentNullException("suite");
            if (path == null)
                throw new ArgumentNullException("path");
            this.suite = suite;
            this.path = path;
        }

        /// <summary>
        /// Gets the test suite the test is part of.
        /// </summary>
        public TestSuite Suite
        {
            get { return this.suite; }
        }

        /// <summary>
        /// The name of the test.
        /// </summary>
        public string Name
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(this.path); }
        }

        /// <summary>
        /// The file path of the test.
        /// </summary>
        public string Path
        {
            get { return this.path; }
        }

        /// <summary>
        /// Gets or sets a description of what the test is doing.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the failure exception.
        /// </summary>
        public Exception FailureException
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that indicates that an exception means the test succeeded.
        /// </summary>
        public bool IsNegativeTest
        {
            get;
            set;
        }
    }
}
