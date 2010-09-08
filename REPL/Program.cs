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
                Console.WriteLine();
                Console.Write("> ");
                string source = Console.ReadLine();
                if (source == "exit" || source == "quit")
                    return;
                try
                {
                    var result = engine.Evaluate(source);

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
                    console.Error(ex.Message);
                }
            }
        }
    }
}
