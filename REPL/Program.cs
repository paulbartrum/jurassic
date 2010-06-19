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
            // Register the firebug console object.
            var console = new FirebugConsole(2);
            Jurassic.Library.GlobalObject.Instance["console"] = console;

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
                    var result = Jurassic.Library.GlobalObject.Eval(source);

                    // Write the result.
                    var original = Console.ForegroundColor;
                    if (result == Jurassic.Undefined.Value)
                        Console.ForegroundColor = ConsoleColor.Gray;
                    else
                        Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("-> ");
                    Console.WriteLine(ToString(result));
                    Console.ForegroundColor = original;
                }
                catch (Exception ex)
                {
                    console.error(ex.Message);
                }
            }
        }

        public static string ToString(object obj)
        {
            if (obj is bool)
                return ((bool)obj) == false ? "false" : "true";
            return obj.ToString();
        }
    }
}
