using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Compiler;

namespace Performance
{

    /// <summary>
    /// Benchmarks the lexer.
    /// </summary>
    [TestClass]
    public class LexerBenchmarks
    {
        [TestMethod]
        public void LexerPerformance()
        {
            var sourceFiles = new Dictionary<string, string>();
            foreach (string path in System.IO.Directory.EnumerateFiles(@"D:\Documents\Visual Studio 2010\Projects\Jurassic\Main\Performance\Files\sunspider-0.9.1"))
                sourceFiles.Add(path, System.IO.File.ReadAllText(path));
            foreach (string path in System.IO.Directory.EnumerateFiles(@"D:\Documents\Visual Studio 2010\Projects\Jurassic\Main\Performance\Files\v8-v4"))
                sourceFiles.Add(path, System.IO.File.ReadAllText(path));

            // 26.4 ops/sec +- 2.5
            TestUtils.Benchmark(() =>
            {
                foreach (var sourceFile in sourceFiles)
                {
                    var lexer = new Lexer(new System.IO.StringReader(sourceFile.Value), sourceFile.Key);
                    try
                    {
                        while (true)
                        {
                            var token = lexer.NextToken();
                            if (token == null)
                                break;
                        }
                    }
                    catch (JavaScriptException ex)
                    {
                        throw new Exception(string.Format("Failed in {0}, line {1}", ex.SourcePath, ex.LineNumber), ex);
                    }
                }
            });
        }
    }
}