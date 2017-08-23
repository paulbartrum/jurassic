using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Jurassic.TestSuite
{
    class Program
    {
        public class CompatTableEntry
        {
            public string group { get; set; }
            public string name { get; set; }
            public string detail { get; set; }
            public string script { get; set; }
            public bool Success { get; set; }
            public WorkerProcessResponse Response { get; set; }
        }

        static void Main(string[] args)
        {
            string inPipeHandle, outPipeHandle;
            if (PipeServer<WorkerProcessRequest, WorkerProcessResponse>.IsChildProcess(out inPipeHandle, out outPipeHandle))
            {
                // Child process.
                WorkerProcess.Start(inPipeHandle, outPipeHandle);
                return;
            }

            // Server process.
            using (var pipeServer = new PipeServer<WorkerProcessRequest, WorkerProcessResponse>())
            {
                var entries = JsonConvert.DeserializeObject<CompatTableEntry[]>(File.ReadAllText(@"Kangax\compat-table.json"));
                foreach (var testCase in entries)
                {
                    try
                    {
                        testCase.Response = pipeServer.Send(new WorkerProcessRequest { Script = $"(function () {{ {testCase.script} }})();" });
                        if (testCase.Response.JsonResult == "true")
                            testCase.Success = true;
                        if (testCase.Success == false && Array.IndexOf(
                            new string[]
                            {
                                "default function parameters",
                                "for..of loops",
                                "template literals",
                                "typed arrays",
                                "WeakMap",
                                "Set",
                                "WeakMap",
                                "WeakSet",
                                "Object static methods",
                                @"function ""name"" property",
                                "Array static methods",
                                "Array.prototype methods",
                            }, testCase.name) >= 0)
                        {
                            Console.WriteLine($"{testCase.name} -- {testCase.detail}, result: {testCase.Response.JsonResult ?? $"{testCase.Response.ErrorType}: {testCase.Response.ErrorMessage}"}");
                        }
                    }
                    catch (IOException)
                    {
                        Console.WriteLine("*** Worker process crashed ***");
                    }
                }

                Console.WriteLine();
                Console.WriteLine();
                foreach (var group in entries.GroupBy(e => e.group))
                {
                    Console.WriteLine($"**{group.Key}**|");
                    foreach (var test in group.GroupBy(e => e.name))
                    {
                        int successCount = test.Count(t => t.Success);
                        int totalCount = test.Count();

                        string status = ":x:";
                        if (successCount == 0)
                            status = ":x:";
                        else if (successCount == totalCount)
                            status = $":white_check_mark: {successCount}/{totalCount}";
                        else
                            status = $"{successCount}/{totalCount}";
                        Console.WriteLine($"&nbsp;&nbsp;{test.Key.Replace("_", "\\_")}|{status}");
                    }
                }
            }

            //// Find the Test Suite Files directory.
            //var testSuiteFilesDir = FindTestSuiteFilesDir();
            //if (testSuiteFilesDir == null)
            //{
            //    Console.WriteLine("Missing 'Test Suite Files' directory.");
            //    Environment.Exit(-1);
            //}

            //var timer = System.Diagnostics.Stopwatch.StartNew();
            //using (var testSuite = new TestSuite(testSuiteFilesDir))
            //{
            //    //testSuite.RunInSandbox = true;
            //    //testSuite.IncludedTests.Add("12.7-1");
            //    //testSuite.IncludedTests.Add("15.2.3.14-5-13");

            //    testSuite.TestFinished += OnTestFinished;
            //    testSuite.Start();

            //    Console.WriteLine();
            //    Console.WriteLine("Finished in {0} minutes, {1} seconds!", (int)timer.Elapsed.TotalMinutes, timer.Elapsed.Seconds);
            //    Console.WriteLine("Succeeded: {0} ({1:P1})", testSuite.SuccessfulTestCount, testSuite.SuccessfulPercentage);
            //    Console.WriteLine("Failed: {0} ({1:P1})", testSuite.FailedTestCount, testSuite.FailedPercentage);
            //    Console.WriteLine("Skipped: {0} ({1:P1})", testSuite.SkippedTestCount, testSuite.SkippedPercentage);
            //    Console.WriteLine("Total: {0}", testSuite.ExecutedTestCount);
            //}
        }

    //    private static string FindTestSuiteFilesDir()
    //    {
    //        var dir = AppDomain.CurrentDomain.BaseDirectory;
    //        do
    //        {
    //            if (Directory.Exists(Path.Combine(dir, "Test Suite Files")))
    //                return Path.Combine(dir, "Test Suite Files");
    //            dir = Path.GetDirectoryName(dir);
    //        } while (dir != null);
    //        return null;
    //    }

    //    private static object lockObject = new object();
    //    private static bool newlineRequired = false;
    //    private static int executedTests = 0;

    //    private static void OnTestFinished(object sender, TestEventArgs e)
    //    {
    //        if (e.Status == TestRunStatus.Success && (e.Test.Suite.ExecutedTestCount % 20) != 0)
    //            return;
    //        lock (lockObject)
    //        {
    //            if (e.Status == TestRunStatus.Failed)
    //            {
    //                if (newlineRequired)
    //                    Console.WriteLine();
    //                var mode = e.StrictMode ? " (strict)" : "";
    //                Console.WriteLine("{0}{1}: {2}", e.Test.Name, mode, e.FailureException.Message);
    //                newlineRequired = false;
    //                //Console.WriteLine(e.Test.Script);
    //            }

    //            if (e.Status != TestRunStatus.Skipped)
    //            {
    //                executedTests++;
    //                if ((executedTests % 20) == 0)
    //                {
    //                    if ((executedTests % 100) == 0)
    //                    {
    //                        if (newlineRequired)
    //                            Console.WriteLine();
    //                        Console.WriteLine("Executed {0} tests", e.Test.Suite.ExecutedTestCount);
    //                        newlineRequired = false;
    //                    }
    //                    else
    //                    {
    //                        Console.Write(".");
    //                        newlineRequired = true;
    //                    }
    //                }
    //            }
    //        }
    //    }
    }
}
