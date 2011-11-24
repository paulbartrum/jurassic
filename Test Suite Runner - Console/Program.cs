using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test_Suite_Runner_WP7
{
    class Program
    {
        static void Main(string[] args)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var engine = new Jurassic.ScriptEngine();
            Console.WriteLine("Start-up time: {0}ms", timer.ElapsedMilliseconds);
            using (var testSuite = new TestSuite(@"..\..\"))
            {
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
                }
                else
                {
                    if ((e.Test.Suite.ExecutedTestCount % 20) == 0)
                    {
                        if ((e.Test.Suite.ExecutedTestCount % 100) == 0)
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
