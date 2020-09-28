using Jurassic;
using Jurassic.Library;
using System;
using System.Text;

namespace REPL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Jurassic version {typeof(ScriptEngine).Assembly.GetName().Version}");
            Console.WriteLine();

            var scriptEngine = new ScriptEngine();
            scriptEngine.SetGlobalValue("console", new FirebugConsole(scriptEngine));

            var script = new StringBuilder();
            while (true)
            {
                Console.ResetColor();
                Console.Write(script.Length == 0 ? "> " : "  ");
                
                script.AppendLine(Console.ReadLine());

                try
                {
                    if (script.ToString() == "exit")
                        return;
                    var result = scriptEngine.Evaluate(script.ToString());

                    if (result == Undefined.Value)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("undefined");
                    }
                    else if (result == Null.Value)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
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
                    script.Clear();
                }
                catch (JavaScriptException ex)
                {
                    if (!ex.Message.StartsWith("SyntaxError: Unexpected end of input"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        if (ex.ErrorObject is ErrorInstance error && error.Stack != null)
                            Console.WriteLine(error.Stack);
                        else
                            Console.WriteLine($"Uncaught {ex.Message}");
                        script.Clear();
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex);
                    script.Clear();
                }
            }
        }

        private static string ReadInput()
        {
            var buffer = new StringBuilder();
            while (true)
            {
                var keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.Enter)
                    Console.WriteLine();
                if (keyInfo.Key == ConsoleKey.Enter && (keyInfo.Modifiers & ConsoleModifiers.Shift) == 0)
                    break;
                buffer.Append(keyInfo.KeyChar);
            }
            return buffer.ToString();
        }
    }
}
