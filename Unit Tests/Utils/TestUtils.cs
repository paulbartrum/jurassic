using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    public enum JSEngine
    {
        Jurassic,
        JScript,
    }

    public static class TestUtils
    {
        [ThreadStatic]
        private static ActiveScriptEngine jscriptEngine;

        public static JSEngine Engine
        {
            get { return JSEngine.Jurassic; }
        }

        public static object Evaluate(string script)
        {
            object result;
            if (Engine == JSEngine.JScript)
            {
                if (jscriptEngine == null)
                    jscriptEngine = ActiveScriptEngine.FromLanguage(ScriptLanguage.JScript);
                result = jscriptEngine.Evaluate(script);
                if (result == DBNull.Value)
                    result = Jurassic.Null.Value;
                else if (result == null)
                    result = Jurassic.Undefined.Value;
            }
            else
            {
                result = Jurassic.Library.GlobalObject.Eval(script);
            }
            if (result is double)
            {
                var numericResult = (double)result;
                if ((double)((int)numericResult) == numericResult)
                    return (int)numericResult;
            }
            return result;
        }

        public static object EvaluateExceptionType(string script)
        {
            try
            {
                Evaluate("try { " + script + "; globalErrorName = 'No error was thrown' } catch(e) { globalErrorName = e.name; }");
            }
            catch (Jurassic.JavaScriptException ex)
            {
                return ex.Name;
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                if (ex.Message == "Syntax error")
                    return "SyntaxError";
                throw;
            }
            return Evaluate("globalErrorName");
        }

        public static Jurassic.Library.PropertyAttributes EvaluateAccessibility(string objectExpression, string propertyName)
        {
            // Surround the script with parentheses to prevent problems with precedence.
            objectExpression = string.Concat("(", objectExpression, ")");
            propertyName = string.Concat(@"""", propertyName, @"""");

            // Check if the property exists.
            if ((bool)Evaluate(string.Format("{0}.hasOwnProperty({1})", objectExpression, propertyName)) == false)
                throw new InvalidOperationException(string.Format("Property '{0}' does not exist or is inherited.", propertyName));

            var result = (Jurassic.Library.PropertyAttributes)0;

            // Check if the property is enumerable.
            if ((bool)Evaluate(string.Format("Object.getOwnPropertyDescriptor({0}, {1}).enumerable", objectExpression, propertyName)) == true)
                result |= Jurassic.Library.PropertyAttributes.Enumerable;

            // Check if the property is writable.
            if ((bool)Evaluate(string.Format("Object.getOwnPropertyDescriptor({0}, {1}).writable", objectExpression, propertyName)) == true)
                result |= Jurassic.Library.PropertyAttributes.Writable;

            // Check if the property is configurable.
            if ((bool)Evaluate(string.Format("Object.getOwnPropertyDescriptor({0}, {1}).configurable", objectExpression, propertyName)) == true)
                result |= Jurassic.Library.PropertyAttributes.Configurable;

            return result;
        }

        public static void Benchmark(Action codeToTest)
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
                throw new AssertInconclusiveException(string.Format("{0}: {1:f1} operations/sec (± {2:f1})", testName, 1000.0 / average, (1000.0 / (average - min) - 1000.0 / (average + max)) / 2));

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

        public static void ExpectException<TException>(Action action) where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException)
            {
                return;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected exception {0} but caught {1}.\r\n{2}", typeof(TException).FullName, ex.GetType().FullName, ex.ToString());
            }
            Assert.Fail("Expected exception {0} but no failure was observed.", typeof(TException).FullName);
        }
    }
}
