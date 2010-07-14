using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Performance
{

    public static class TestUtils
    {
        public static void Benchmark(Action codeToTest, double previousResult = 0)
        {
            // Up the thread priority.
            var priorPriority = System.Threading.Thread.CurrentThread.Priority;
            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;
            try
            {
                // Get the test name from a stack trace.
                var testName = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;

                // Measure the test overhead.
                Action emptyAction = () => { };
                var stopWatch = System.Diagnostics.Stopwatch.StartNew();
                for (int i = 0; i < 100; i++)
                    emptyAction();
                long overheadInTicks = stopWatch.ElapsedTicks / 100;

                // Make sure the code is jitted.
                codeToTest();

                // Run the test a number of times.
                long totalTimeRemaining = System.Diagnostics.Stopwatch.Frequency * 2;
                var elapsedTimes = new List<long>();
                while (totalTimeRemaining > 0)
                {
                    // Reset the stopwatch.
                    stopWatch.Restart();

                    // Run the code to test.
                    codeToTest();

                    // Record the time taken.
                    long elapsed = Math.Max(stopWatch.ElapsedTicks - overheadInTicks, 0);
                    elapsedTimes.Add(elapsed);

                    // Collect all garbage.
                    System.GC.Collect();

                    // Check if we have run for the required amount of time.
                    totalTimeRemaining -= stopWatch.ElapsedTicks;
                }

                double average = elapsedTimes.Average();
                //double variance = elapsedTimes.Select(e => Math.Pow(average - e, 2)).Average();
                //double deviation = Math.Sqrt(variance);
                double min = Math.Sqrt(elapsedTimes.Where(e => e <= average).Select(e => Math.Pow(average - e, 2)).Average());
                double max = Math.Sqrt(elapsedTimes.Where(e => e >= average).Select(e => Math.Pow(average - e, 2)).Average());

                // Convert to milliseconds.
                double ticksToMilliseconds = 1000.0 / (double)System.Diagnostics.Stopwatch.Frequency;
                average *= ticksToMilliseconds;
                //variance *= ticksToMilliseconds;
                //deviation *= ticksToMilliseconds;
                min *= ticksToMilliseconds;
                max *= ticksToMilliseconds;

                // Output the time taken.
                //Console.WriteLine("Performance test '{0}' took {1:f1} ± {2:f1} milliseconds.", testName, average, deviation * 2);
                for (int i = 0; i < elapsedTimes.Count; i++)
                    Console.WriteLine("Test #{0}: {1:f1} milliseconds.", i + 1, elapsedTimes[i] * ticksToMilliseconds);

                // Show the results in the unit test error message column.
                throw new AssertInconclusiveException(string.Format("{0:f1} operations/sec (± {1:f1}), was {2}",
                    1000.0 / average, (1000.0 / (average - min) - 1000.0 / (average + max)) / 2, previousResult));

                //if (testName != null)
                //{
                //    string outputDir = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(AssertUtils).Assembly.Location), @"..\..\..\Performance Tests\");
                //    if (System.IO.Directory.Exists(outputDir) == false)
                //        System.IO.Directory.CreateDirectory(outputDir);
                //    string outputPath = System.IO.Path.Combine(outputDir, testName + ".csv");
                //    if (System.IO.File.Exists(outputPath) == false)
                //        System.IO.File.WriteAllText(outputPath, "Time,Sample,Variance");
                //    System.IO.File.AppendAllText(outputPath, string.Format("\r\n{0:yyyy'-'MM'-'dd HH':'mm':'ss},{1:f1},{2:f1}", DateTime.Now, average, deviation));
                //}
            }
            finally
            {
                // Revert the thread priority.
                System.Threading.Thread.CurrentThread.Priority = priorPriority;
            }
        }

        public static void Benchmark(string code, double previousResult = 0)
        {
            // Parse the javascript code.
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var context = new Jurassic.Compiler.GlobalContext(new System.IO.StringReader(code), null);
            context.Parse();
            Console.WriteLine("Parse: {0:n1}ms", timer.Elapsed.TotalMilliseconds);

            // Optimize the code.
            timer.Reset();
            context.Optimize();
            Console.WriteLine("Optimization: {0:n1}ms", timer.Elapsed.TotalMilliseconds);

            // Compile the code.
            timer.Reset();
            context.GenerateCode();
            Console.WriteLine("Code generation: {0:n1}ms", timer.Elapsed.TotalMilliseconds);

            // Run the javascript code.
            Benchmark(() => context.Execute(), previousResult);
        }
    }

}