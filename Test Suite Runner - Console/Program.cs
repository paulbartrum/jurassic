using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic.TestSuite;

namespace Jurassic.TestSuite
{
    class Program
    {
        static void Main(string[] args)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var engine = new Jurassic.ScriptEngine();
            Console.WriteLine("Start-up time: {0}ms", timer.ElapsedMilliseconds);
            using (var testSuite = new TestSuite(@"..\..\..\Test Suite Files\"))
            {
                //testSuite.RunInSandbox = true;
                //testSuite.IncludedTests.Add("12.7-1");
                //testSuite.IncludedTests.Add("15.2.3.14-5-13");

                testSuite.TestFinished += OnTestFinished;
                testSuite.Start();

                Console.WriteLine();
                Console.WriteLine("Finished in {0} minutes, {1} seconds!", (int)timer.Elapsed.TotalMinutes, timer.Elapsed.Seconds);
                Console.WriteLine("Succeeded: {0} ({1:P1})", testSuite.SuccessfulTestCount, testSuite.SuccessfulPercentage);
                Console.WriteLine("Failed: {0} ({1:P1})", testSuite.FailedTestCount, testSuite.FailedPercentage);
                Console.WriteLine("Skipped: {0} ({1:P1})", testSuite.SkippedTestCount, testSuite.SkippedPercentage);
                Console.WriteLine("Total: {0}", testSuite.ExecutedTestCount);
            }
        }

        private static object lockObject = new object();
        private static bool newlineRequired = false;
        private static int executedTests = 0;

        private static void OnTestFinished(object sender, TestEventArgs e)
        {
            if (e.Status == TestRunStatus.Success && (e.Test.Suite.ExecutedTestCount % 20) != 0)
                return;
            lock (lockObject)
            {
                if (e.Status == TestRunStatus.Failed)
                {
                    if (newlineRequired)
                        Console.WriteLine();
                    var mode = e.StrictMode ? " (strict)" : "";
                    Console.WriteLine("{0}{1}: {2}", e.Test.Name, mode, e.FailureException.Message);
                    newlineRequired = false;
                    //Console.WriteLine(e.Test.Script);
                }

                if (e.Status != TestRunStatus.Skipped)
                {
                    executedTests++;
                    if ((executedTests % 20) == 0)
                    {
                        if ((executedTests % 100) == 0)
                        {
                            if (newlineRequired)
                                Console.WriteLine();
                            Console.WriteLine("Executed {0} tests", e.Test.Suite.ExecutedTestCount);
                            newlineRequired = false;
                        }
                        else
                        {
                            Console.Write(".");
                            newlineRequired = true;
                        }
                    }
                }
            }
        }
    }
}
