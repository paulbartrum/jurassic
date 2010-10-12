using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace REPL
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = new Jurassic.ScriptEngine();

            // Register the firebug console object.
            var console = new Jurassic.Library.FirebugConsole(engine);
            console.CurrentIndentation = 2;
            engine.Global["console"] = console;

            Console.WriteLine("JavaScript console (type 'quit' to exit)");
            Console.WriteLine("Most Firebug console commands are available");
            Console.WriteLine("(see http://getfirebug.com/wiki/index.php/Console_API)");

            while (true)
            {
                var source = new StringBuilder();
                var continuation = false;
                do
                {
                    if (continuation == false)
                    {
                        Console.WriteLine();
                        Console.Write("> ");
                        string line = Console.ReadLine();
                        if (line == "exit" || line == "quit")
                            return;
                        source.Append(line);
                    }
                    else
                    {
                        // This is a continuation of a previous command.
                        Console.Write("  ");
                        source.Append(Environment.NewLine);
                        source.Append(Console.ReadLine());
                    }

                    // Check for an empty command.
                    if (source.Length == 0)
                        break;

                    try
                    {
                        continuation = false;
                        var result = engine.Evaluate(source.ToString());

                        // Write the result.
                        var original = Console.ForegroundColor;
                        if (result == Jurassic.Undefined.Value)
                            Console.ForegroundColor = ConsoleColor.Gray;
                        else
                            Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("-> ");
                        Console.WriteLine(Jurassic.TypeConverter.ToString(result));
                        Console.ForegroundColor = original;
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("end of input"))
                            continuation = true;
                        else
                            console.Error(ex.Message);
                    }
                } while (continuation == true);
            }
        }
    }
}
