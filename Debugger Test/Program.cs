using System;
using Jurassic;
using Jurassic.Extensions;

namespace Debugger_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var scriptEngine = new ScriptEngine();
            scriptEngine.EnableDebugging = true;
            scriptEngine.AddFirebugConsole();
            scriptEngine.ExecuteFile(@"..\..\prime.js");
            Console.ReadKey();
        }
    }
}
