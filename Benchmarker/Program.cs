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

        // Init took 61.3242ms
        // Init took 102.1342ms
        // 
        // 3d-cube - 331.0ms ± 2.4ms
        // 3d-morph - 168.8ms ± 5.9ms
        // 3d-raytrace - 343.2ms ± 10.9ms
        // access-binary-trees - 213.3ms ± 1.2ms
        // access-fannkuch - 391.3ms ± 1.2ms
        // access-nbody - 177.3ms ± 2.1ms
        // access-nsieve - 336.9ms ± 19.2ms
        // bitops-3bit-bits-in-byte - 24.4ms ± 0.9ms
        // bitops-bits-in-byte - 22.9ms ± 0.2ms
        // bitops-bitwise-and - 40.5ms ± 0.4ms
        // bitops-nsieve-bits - 222.6ms ± 2.5ms
        // controlflow-recursive - 39.4ms ± 1.4ms
        // crypto-aes - 310.7ms ± 2.2ms
        // crypto-md5 - 202.9ms ± 1.1ms
        // crypto-sha1 - 113.4ms ± 1.6ms
        // date-format-tofte - 2,508.9ms ± 14.3ms
        // date-format-xparb - 143.6ms ± 1.6ms
        // math-cordic - 122.5ms ± 1.5ms
        // math-partial-sums - 65.9ms ± 0.2ms
        // math-spectral-norm - 104.9ms ± 0.9ms
        // regexp-dna - 465.3ms ± 6.0ms
        // string-base64 - 260.1ms ± 0.8ms
        // string-fasta - 176.8ms ± 1.1ms
        // string-tagcloud - 416.7ms ± 2.3ms
        // string-unpack-code - 555.2ms ± 22.6ms
        // string-validate-input - 163.0ms ± 6.9ms
        // Total elapsed time: 7,921.4ms

        static void Main(string[] args)
        {
            // Up the thread priority so nothing gets in the way of the benchmarking.
            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.AboveNormal;

            // 61.3242ms
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            var initEngine = new ScriptEngine();
            Console.WriteLine($"Init took {stopWatch.Elapsed.TotalMilliseconds}ms");

            // 144.1342ms.
            stopWatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {
                initEngine = new ScriptEngine();
            }
            Console.WriteLine($"Init x 1000 took {stopWatch.Elapsed.TotalMilliseconds}ms");

            // Sunspider.
            var files = Directory.GetFiles(@"..\..\..\Files\sunspider-0.9.1", "*.js");
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

        //private static void SaveAndVerifyDLL(string code, string path = null)
        //{
        //    path = SaveDLL(code, path);

        //    // Copy Jurassic.dll to the same path.
        //    var jurassicDllPath = Path.Combine(Path.GetDirectoryName(path), "Jurassic.dll");
        //    File.Copy("Jurassic.dll", jurassicDllPath, overwrite: true);

        //    try
        //    {
        //        VerifyDLL(path);
        //    }
        //    finally
        //    {
        //        File.Delete(path);
        //        File.Delete(jurassicDllPath);
        //    }

        //    //System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\x64\ildasm.exe", dllPath);
        //}

        //private static string GetFunctionIL(string functionName, string code)
        //{
        //    var scriptEngine = new ScriptEngine();
        //    scriptEngine.EnableILAnalysis = true;
        //    scriptEngine.Execute(code);
        //    var function = (Jurassic.Library.UserDefinedFunction)scriptEngine.GetGlobalValue(functionName);
        //    if (function == null)
        //        throw new ArgumentException(string.Format("The function {0} was not found.", functionName));
        //    return function.DisassembledIL;
        //}

        //private static string SaveDLL(string code, string path = null)
        //{
        //    // Make sure the path is valid.
        //    if (string.IsNullOrEmpty(path))
        //        path = Path.GetTempFileName();
        //    else if (Path.GetDirectoryName(path) == "")
        //        path = Path.Combine(Path.GetTempPath(), path);

        //    // Create a dynamic assembly and module.
        //    AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(Path.GetFileNameWithoutExtension(path)),
        //        AssemblyBuilderAccess.RunAndSave, Path.GetDirectoryName(path), true, null);
        //    ModuleBuilder module = assemblyBuilder.DefineDynamicModule("Module", Path.GetFileName(path));

        //    // Generate the code.
        //    var scriptEngine = new ScriptEngine();
        //    scriptEngine.EnableDebugging = true;
        //    scriptEngine.EnableILAnalysis = true;
        //    scriptEngine.ReflectionEmitInfo = new ScriptEngine.ReflectionEmitModuleInfo() { AssemblyBuilder = assemblyBuilder, ModuleBuilder = module };
        //    scriptEngine.Execute(code);

        //    // Save it.
        //    assemblyBuilder.Save(Path.GetFileName(path));
        //    return path;
        //}

        //private static void VerifyDLL(string path)
        //{
        //    var p = new System.Diagnostics.Process();
        //    p.StartInfo.UseShellExecute = false;
        //    p.StartInfo.RedirectStandardOutput = true;
        //    p.StartInfo.FileName = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\x64\peverify.exe";
        //    p.StartInfo.Arguments = path;
        //    p.Start();
        //    Console.WriteLine(p.StandardOutput.ReadToEnd());
        //    p.WaitForExit();
        //}
    }
}
