using System;
using System.Collections.Generic;
using System.IO;
using Jurassic;

namespace Benchmarker
{
    class Program
    {
        private class Series
        {
            private List<double> samples = new List<double>();

            public void AddSample(double value)
            {
                this.samples.Add(value);
            }

            public double Mean
            {
                get
                {
                    double total = 0;
                    foreach (var sample in this.samples)
                        total += sample;
                    return total / this.samples.Count;
                }
            }

            public double StandardDeviation
            {
                get
                {
                    double mean = this.Mean;
                    double result = 0;
                    foreach (var sample in this.samples)
                        result += (sample - mean) * (sample - mean);
                    result /= this.samples.Count - 1;
                    result = Math.Sqrt(result);
                    return result;
                }
            }
        }

        static void Main(string[] args)
        {
            // Up the thread priority so nothing gets in the way of the benchmarking.
            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.AboveNormal;

            // Sunspider.
            var files = Directory.GetFiles(@"..\..\Files\sunspider-0.9.1", "*.js");
            var results = new Series[files.Length];
            for (int j = 0; j < 5; j++)
            {
                Console.WriteLine("Beginning phase #{0}...", j + 1);

                for (int i = 0; i < files.Length; i++)
                {
                    // Get the script path.
                    var path = files[i];

                    // Indicate we are running the test.
                    Console.Write("Running {0}... ", Path.GetFileNameWithoutExtension(path));

                    // Initialize the script engine.
                    var engine = new ScriptEngine();

                    // Load the javascript source into a string (so no I/O during benchmarking).
                    var script = File.ReadAllText(path);

                    // Start timing.
                    var timer = System.Diagnostics.Stopwatch.StartNew();

                    // Execute the script.
                    engine.Execute(new StringScriptSource(script, path));

                    // Stop timing.
                    double elapsed = timer.Elapsed.TotalMilliseconds;

                    // Add the result to the prior result (but throw away the first result).
                    if (results[i] == null)
                        results[i] = new Series();
                    else
                        results[i].AddSample(elapsed);

                    // Output the result to the screen.
                    Console.WriteLine("{0:n1}ms", elapsed);
                }

                Console.WriteLine();
            }

            // Print the results.
            Console.WriteLine("Summary");
            Console.WriteLine();
            double total = 0;
            for (int i = 0; i < files.Length; i++)
            {
                total += results[i].Mean;
                Console.WriteLine("{0} - {1:n1}ms \u00B1 {2:n1}ms",
                    Path.GetFileNameWithoutExtension(files[i]),
                    results[i].Mean, results[i].StandardDeviation);
            }
            Console.WriteLine("Total elapsed time: {0:n1}ms", total);
        }
    }
}
