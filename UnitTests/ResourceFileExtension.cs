using Jurassic;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace UnitTests
{
    public static class ResourceFileExtension
    {
        public static void ExecuteResource(this ScriptEngine engine, string resourceFileName)
        {
            string resName = typeof(ResourceFileExtension).Namespace + "." + resourceFileName.Replace("/", ".");
            var resStream = typeof(ResourceFileExtension).GetTypeInfo().Assembly.GetManifestResourceStream( resName);
            if (resStream == null)
                throw new FileNotFoundException($"Resource not found : {resourceFileName} => {resName}");
            StringScriptSource src;
            using (resStream)
            using (StreamReader sr = new StreamReader(resStream))
            {
                src = new StringScriptSource(sr.ReadToEnd(), resourceFileName);
            }
            engine.Execute(src);
        }
    }
}
