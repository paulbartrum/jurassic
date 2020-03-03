using Jurassic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UnitTests
{
    public abstract class TestBase
    {
        private static AsyncLocal<ScriptEngine> scriptEngine = new AsyncLocal<ScriptEngine>();

        [TestInitialize]
        public void Init()
        {
            // Each test method gets a fresh script engine instance.
            scriptEngine.Value = null;
        }

        public ScriptEngine ScriptEngine
        {
            get { InitializeJurassic(); return scriptEngine.Value; }
        }

        public CompatibilityMode CompatibilityMode
        {
            get { InitializeJurassic(); return scriptEngine.Value.CompatibilityMode; }
            set { InitializeJurassic(); scriptEngine.Value.CompatibilityMode = value; }
        }

        public object Evaluate(string script)
        {
            InitializeJurassic();
            OnBeforeExecute();
            return scriptEngine.Value.Evaluate(script);
        }

        public void Execute(string script)
        {
            InitializeJurassic();
            OnBeforeExecute();
            scriptEngine.Value.Execute(script);
        }

        private void InitializeJurassic()
        {
            if (scriptEngine.Value == null)
                scriptEngine.Value = InitializeScriptEngine();
        }

        protected virtual ScriptEngine InitializeScriptEngine()
        {
            return new ScriptEngine();
        }

        protected virtual void OnBeforeExecute()
        {
        }

        public string EvaluateExceptionType(string script)
        {
            object result;
            try
            {
                result = Evaluate(script);
            }
            catch (JavaScriptException ex)
            {
                return ex.Name;
            }
            return string.Format("No error was thrown (result was '{0}')", result);
        }

        public string EvaluateExceptionMessage(string script)
        {
            object result;
            try
            {
                result = Evaluate(script);
            }
            catch (JavaScriptException ex)
            {
                return ex.Message;
            }
            return string.Format("No error was thrown (result was '{0}')", result);
        }

        public Jurassic.Library.PropertyAttributes EvaluateAccessibility(string objectExpression, string propertyName)
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
            var priorPriority = Thread.CurrentThread.Priority;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
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
                    stopWatch.Reset();
                    stopWatch.Start();

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
                Thread.CurrentThread.Priority = priorPriority;
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

        /// <summary>
        /// Removes spaces from the start of each line and removes extraneous line breaks from the
        /// start and end of the given text.
        /// </summary>
        /// <param name="text"> The text to operate on. </param>
        /// <param name="lineBreak"> The type of line break to normalize to. </param>
        /// <returns> The text, but with extra space removed. </returns>
        public static string NormalizeText(string text, string lineBreak = null)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            // Find the maximum number of spaces that is common to each line.
            bool startOfLine = true;
            int indentationToRemove = int.MaxValue;
            int startOfLineSpace = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\r' || text[i] == '\n')
                {
                    startOfLine = true;
                    startOfLineSpace = 0;
                }
                else if (startOfLine == true)
                {
                    if (text[i] == ' ')
                        startOfLineSpace++;
                    else
                    {
                        indentationToRemove = Math.Min(indentationToRemove, startOfLineSpace);
                        startOfLine = false;
                    }
                }
            }

            // Remove that amount of space from each line.
            // Also, normalize line breaks to Environment.NewLine.
            var result = new StringBuilder(text.Length);
            int j = 0;
            for (; j < Math.Min(indentationToRemove, text.Length); j++)
                if (text[j] != ' ')
                    break;
            for (int i = j; i < text.Length; i++)
            {
                if (text[i] == '\r' || text[i] == '\n')
                {
                    if (text[i] == '\r' && i < text.Length - 1 && text[i + 1] == '\n')
                        i++;
                    result.Append(lineBreak == null ? Environment.NewLine : lineBreak);
                    i++;
                    for (j = i; j < Math.Min(i + indentationToRemove, text.Length); j++)
                        if (text[j] != ' ')
                            break;
                    i = j - 1;
                }
                else
                    result.Append(text[i]);
            }
            return result.ToString().Trim('\r', '\n');
        }

        /// <summary>
        /// Changes the culture to run the the given action, then restores the culture.
        /// </summary>
        /// <param name="cultureName"> The culture name. </param>
        /// <param name="action"> The action to run under the modified culture. </param>
        public static T ChangeLocale<T>(string cultureName, Func<T> action)
        {
            // Save the current culture.
            var previousCulture = Thread.CurrentThread.CurrentCulture;

            // Replace it with a new culture.
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName, false);

            try
            {
                // Run the action.
                return action();
            }
            finally
            {
                // Restore the previous culture.
                Thread.CurrentThread.CurrentCulture = previousCulture;
            }
        }
    }
}
