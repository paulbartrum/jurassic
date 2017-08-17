using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Library
{

    public class ClassWithCallCounters
    {
        private readonly int _iterationsLimit;
        public int InstanceCounter = 0;
        public static int StaticCounter = 0;

        public ClassWithCallCounters(int iterationsLimit=100)
        {
            _iterationsLimit = iterationsLimit;
        }
        public void InstanceMethod()
        {
            InstanceCounter++;

            if (InstanceCounter > _iterationsLimit)
                throw new InvalidOperationException($"Reached a maximum of {_iterationsLimit} iterations");


        }

        public static void StaticMetod()
        {
            StaticCounter++;
        }
    }

    [TestClass]
    public class EmitUserCodeOnLoopIterationTests
    {
    

        [TestMethod]
        public void OnLoopCallWithInstanceMethod()
        {
            var engine = new ScriptEngine();
            var classWithCallCounters = new ClassWithCallCounters();
            engine.OnLoopIterationCall = classWithCallCounters.InstanceMethod;
            var loopScript = @"
function Test(){ 

for (var i=0; i< 20; i++){

}
}";
            engine.Evaluate(loopScript);
            engine.CallGlobalFunction("Test");
            Assert.AreEqual(19, classWithCallCounters.InstanceCounter);
        }

        [TestMethod]
        public void OnLoopTerationAndTrivialContinueStatement()
        {
            var engine = new ScriptEngine();
            var classWithCallCounters = new ClassWithCallCounters();
            engine.OnLoopIterationCall = classWithCallCounters.InstanceMethod;
            engine.EnableDebugging = true;
            var loopScript = @"
function Test(){ 
for ( var i=0; i< 10; i++){
continue;
}
}";
            engine.Evaluate(loopScript);
            engine.CallGlobalFunction("Test");
            Assert.AreEqual(19, classWithCallCounters.InstanceCounter);
        }

        [TestMethod]
        public void OnLoopTerationAndLabeledContinueStatement()
        {
            var engine = new ScriptEngine();
            var classWithCallCounters = new ClassWithCallCounters();
            engine.OnLoopIterationCall = classWithCallCounters.InstanceMethod;
            engine.EnableDebugging = true;
            var loopScript = @"
function Test(){ 
label1:
for ( var i=0; i< 10; i++){
continue label1;
}
}";
            engine.Evaluate(loopScript);
            engine.CallGlobalFunction("Test");
            Assert.AreEqual(19, classWithCallCounters.InstanceCounter);
        }

        [TestMethod]
        public void OnNestedLoop()
        {
            var engine = new ScriptEngine();
            var classWithCallCounters = new ClassWithCallCounters();
            engine.OnLoopIterationCall = classWithCallCounters.InstanceMethod;
            engine.EnableDebugging = true;
            var loopScript = @"
function Test(){ 

for ( var i=0; i< 10; i++){
for (var j=0; j< 10; j++){
}

}
}";
            engine.Evaluate(loopScript);

            engine.CallGlobalFunction("Test");

            Assert.AreEqual(99, classWithCallCounters.InstanceCounter);
          
        }

        [TestMethod]
        public void OnLoopTerationAndNestedLabeledContinueStatement()
        {
            var engine = new ScriptEngine();
            var classWithCallCounters = new ClassWithCallCounters();
            engine.OnLoopIterationCall = classWithCallCounters.InstanceMethod;
            engine.EnableDebugging = true;
            var loopScript = @"
function Test(){ 
label1:
for ( var i=0; i< 10; i++){
label2:
for (var j=0; j< 10; j++){
continue label1;
}

}
}";
            engine.Evaluate(loopScript);

            engine.CallGlobalFunction("Test");

            Assert.AreEqual(19, classWithCallCounters.InstanceCounter);
        }

        [TestMethod]
        public void ThrowRightExceptionOnALotOfIterations()
        {
            var engine = new ScriptEngine();
            var classWithCallCounters = new ClassWithCallCounters(5);
            engine.OnLoopIterationCall = classWithCallCounters.InstanceMethod;
            engine.EnableDebugging = true;
            var loopScript = @"
function Test(){ 

for ( var i=0; i< 10; i++){

}
}";
            engine.Evaluate(loopScript);

            Exception e = null;
            try
            {
                engine.CallGlobalFunction("Test");
            }
            catch (Exception ex)
            {
                e = ex;
            }

            Assert.IsInstanceOfType(e, typeof(InvalidOperationException));
        }
    }
}
