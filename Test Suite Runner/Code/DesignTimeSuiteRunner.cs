using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jurassic;
using Jurassic.Library;
using System.Threading.Tasks;

namespace Test_Suite_Runner
{
    public class DesignTimeSuiteRunner : SuiteRunner
    {
        public DesignTimeSuiteRunner()
        {
            this.Suites.Add(new ES5ConformTestSuite());
            this.Suites.Add(new SputnikTestSuite());
        }
    }
}
