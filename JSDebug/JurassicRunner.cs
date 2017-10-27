using Jurassic;
using System;

namespace JSDebug
{
    public class JurassicRunner
    {
        static void Main(string[] args)
        {
            ScriptEngine jurassic = new ScriptEngine();
//            jurassic.EnableDebugging = true;
            jurassic.EnableILAnalysis = true;
            jurassic.ExecuteFile("Script.js");
        }
    }
}

