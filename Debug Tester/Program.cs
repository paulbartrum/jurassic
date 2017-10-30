using System;
using System.Collections.Generic;
using Jurassic;
using Jurassic.Debugging;

namespace Debug_Tester
{
    public class JSConsole
    {
        public void log(string message)
        {
            Console.WriteLine(message);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ScriptEngine jurassic = new ScriptEngine();
            jurassic.EnableExposedClrTypes = true;
            jurassic.SymbolFactory = DebugSymbolHelper.DebugSymbolFactory;
            //jurassic.EnableILAnalysis = true;
            jurassic.SetGlobalValue("console", new JSConsole());

            jurassic.ExecuteFile("Script.js");
            Console.ReadKey();
        }
    }
}
