using Jurassic;
using Jurassic.Library;
using System;

namespace REPL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Jurassic version {typeof(ScriptEngine).Assembly.GetName().Version}");
            Console.WriteLine();

            var scriptEngine = new ScriptEngine();

            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (line == "exit")
                    return;
                try
                {
                    var result = scriptEngine.Evaluate(line);

                    var previousForegroundColor = Console.ForegroundColor;
                    if (result == Undefined.Value)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine("undefined");
                    }
                    else if (result == Null.Value)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine("null");
                    }
                    else if (result is bool)
                        Console.WriteLine(result);
                    else if (result is double)
                        Console.WriteLine(result);
                    else if (result is string)
                        Console.WriteLine($"\"{result}\"");
                    else if (result is int)
                        Console.WriteLine(result);
                    else if (result is ObjectInstance obj)
                        Console.WriteLine(obj.ToString());
                    Console.ForegroundColor = previousForegroundColor;
                }
                catch (JavaScriptException ex) when (ex.ErrorObject is ErrorInstance error)
                {
                    var previousForegroundColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    if (error.Stack != null)
                        Console.WriteLine(error.Stack);
                    else
                        Console.WriteLine($"Uncaught {ex.Message}");
                    Console.ForegroundColor = previousForegroundColor;
                }
                catch (Exception ex)
                {
                    var previousForegroundColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex);
                    Console.ForegroundColor = previousForegroundColor;
                }
            }
        }
    }
}
