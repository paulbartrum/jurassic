using Jurassic;
using System;

namespace Debugger_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var scriptEngine = new ScriptEngine();
            scriptEngine.EnableDebugging = true;
            scriptEngine.SetGlobalValue("console", new Jurassic.Library.FirebugConsole(scriptEngine));
            scriptEngine.ExecuteFile(@"..\..\prime.js");
            Console.ReadKey();
        }
    }
}
