using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic.Library;

namespace Performance
{

    /// <summary>
    /// Test the debugging experience.
    /// </summary>
    [TestClass]
    public class StepTest
    {
        [TestMethod]
        public void Debug()
        {
            int x, y = 10;
            var path = Path.GetFullPath(@"..\..\..\Performance\Files\step-test.js");
            var context = new Jurassic.Compiler.GlobalContext(new System.IO.StreamReader(path), path);
            context.Execute();
        }
    }

}