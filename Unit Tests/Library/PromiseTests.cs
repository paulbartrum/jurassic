using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Jurassic.Library.PromiseInstance;
using Jurassic.Library;
using Jurassic;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System;

namespace UnitTests
{
    /// <summary>
    /// Test the global Promise object.
    /// </summary>
    [TestClass]
    public class PromiseTests : TestBase
    {
        private struct NotifyCompletionWithNoResult : INotifyCompletion
        {
            public void OnCompleted(Action continuation) => continuation();
        }

        private static PromiseInstance EvaluatePromise(string script)
        {
            var result = Evaluate(script);
            Assert.IsInstanceOfType(result, typeof(PromiseInstance));
            return (PromiseInstance)result;
        }

        [TestMethod]
        public void Constructor()
        {
            // Call
            Assert.AreEqual("TypeError", EvaluateExceptionType("Promise()"));

            // Construct
            Assert.AreEqual("TypeError", EvaluateExceptionType("new Promise()"));
            Assert.AreEqual("[object Promise]", Evaluate("new Promise(function(resolve, reject) { }).toString()"));
            Assert.AreEqual("s", Evaluate("var f; var x = new Promise(function(resolve, reject) { f = resolve; }); f.name"));
            Assert.AreEqual("t", Evaluate("var f; var x = new Promise(function(resolve, reject) { f = reject; }); f.name"));

            // toString and valueOf.
            Assert.AreEqual("function Promise() { [native code] }", Evaluate("Promise.toString()"));
            Assert.AreEqual("Promise", Evaluate("Promise.prototype[Symbol.toStringTag]"));
            Assert.AreEqual(true, Evaluate("Promise.valueOf() === Promise"));

            // species
            Assert.AreEqual(true, Evaluate("Promise[Symbol.species] === Promise"));

            // length
            Assert.AreEqual(1, Evaluate("Promise.length"));
        }

        [TestMethod]
        public void Resolve()
        {
            var promise = EvaluatePromise("new Promise(function(resolve, reject) { resolve(1) })");
            Assert.AreEqual(PromiseState.Fulfilled, promise.State);
            Assert.AreEqual(1, (int)promise.Result);

            promise = EvaluatePromise("new Promise(function(resolve, reject) { resolve() })");
            Assert.AreEqual(PromiseState.Fulfilled, promise.State);
            Assert.AreEqual(Undefined.Value, promise.Result);

            promise = EvaluatePromise("Promise.resolve(1)");
            Assert.AreEqual(PromiseState.Fulfilled, promise.State);
            Assert.AreEqual(1, (int)promise.Result);

            promise = EvaluatePromise("Promise.resolve()");
            Assert.AreEqual(PromiseState.Fulfilled, promise.State);
            Assert.AreEqual(Undefined.Value, promise.Result);
        }

        [TestMethod]
        public void Reject()
        {
            var promise = EvaluatePromise("new Promise(function(resolve, reject) { reject(1) })");
            Assert.AreEqual(PromiseState.Rejected, promise.State);
            Assert.AreEqual(1, (int)promise.Result);

            promise = EvaluatePromise("new Promise(function(resolve, reject) { reject() })");
            Assert.AreEqual(PromiseState.Rejected, promise.State);
            Assert.AreEqual(Undefined.Value, promise.Result);

            promise = EvaluatePromise("Promise.reject(1)");
            Assert.AreEqual(PromiseState.Rejected, promise.State);
            Assert.AreEqual(1, (int)promise.Result);

            promise = EvaluatePromise("Promise.reject()");
            Assert.AreEqual(PromiseState.Rejected, promise.State);
            Assert.AreEqual(Undefined.Value, promise.Result);
        }

        [TestMethod]
        public void Then()
        {
            Execute(@"
                (function(){
                    p = new Promise(function(resolve, reject) { }).then(function(r) { testingContext.push(r) }, function(r) { testingContext.push(r) }); 
                })()");
            Assert.AreEqual(0, (int)testingContext.Length);

            Execute(@"
                (function(){ 
                    p = new Promise(function(resolve, reject) { resolve(2) }).then(function(r) { testingContext.push(r) }, function(r) { throw r });
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(2, (int)testingContext[0]);

            Execute(@"
                (function(){ 
                    p = new Promise(function(resolve, reject) { reject(3) }).then(function(r) { throw r }, function(r) { testingContext.push(r) });
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(3, (int)testingContext[0]);
        }

        [TestMethod]
        public void Catch()
        {
            Execute(@"
                (function(){ 
                    p = new Promise(function(resolve, reject) { }).then(function(r) { throw r }).catch(function(r) { testingContext.push(r) });
                })()");
            Assert.AreEqual(0, (int)testingContext.Length);

            Execute(@"
                (function(){ 
                    p = new Promise(function(resolve, reject) { resolve(2) }).catch(function(r) { testingContext.push(r) });
                })()");
            Assert.AreEqual(0, (int)testingContext.Length);

            Execute(@"
                (function(){ 
                    p = new Promise(function(resolve, reject) { reject(2) }).then(function(r) { throw r }).catch(function(r) { testingContext.push(r) });
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(2, (int)testingContext[0]);
        }

        [TestMethod]
        public void Finally()
        {
            Execute(@"
                (function(){ 
                    p = new Promise(function(resolve, reject) { }).finally(function() { testingContext.push('Complete') });
                })()");
            Assert.AreEqual(0, (int)testingContext.Length);

            Execute(@"
                (function(){ 
                    p = new Promise(function(resolve, reject) { resolve(2) }).finally(function() { testingContext.push('Complete') });
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);

            Execute(@"
                (function(){ 
                    p = new Promise(function(resolve, reject) { reject(2) }).finally(function() { testingContext.push('Complete') });
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
        }

        [TestMethod]
        public void Race()
        {
            // No action
            Execute(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var promise = Promise.race([promise1, promise2, promise3]).finally(function() { testingContext.push('Complete') });
                })()");
            Assert.AreEqual(0, (int)testingContext.Length);

            // Resolve
            Execute(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var promise = Promise.race([promise1, promise2, promise3]).then(function(r) { testingContext.push(r) });
                    complete2[1](2);
                })()");
            Assert.AreEqual(0, (int)testingContext.Length);

            Execute(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var promise = Promise.race([promise1, promise2, promise3]).then(function(r) { testingContext.push(r) });
                    complete2[0](2);
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(2, (int)testingContext[0]);

            Execute(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var promise = Promise.race([promise1, promise2, promise3]).then(function(r) { testingContext.push(r) });
                    complete3[0](3);
                    complete2[0](2);
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(3, (int)testingContext[0]);

            Execute(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    complete3[0](3);
                    var promise = Promise.race([promise1, promise2, promise3]).then(function(r) { testingContext.push(r) });
                    complete2[0](2);
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(3, (int)testingContext[0]);

            Execute(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var promise = Promise.race([promise1, promise2, 3]).then(function(r) { testingContext.push(r) });
                    complete2[0](2);
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(3, (int)testingContext[0]);

            // Reject
            Execute(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var promise = Promise.race([promise1, promise2, promise3]).catch(function(r) { testingContext.push(r) });
                    complete2[0](2);
                })()");
            Assert.AreEqual(0, (int)testingContext.Length);

            Execute(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var promise = Promise.race([promise1, promise2, promise3]).catch(function(r) { testingContext.push(r) });
                    complete2[1](2);
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(2, (int)testingContext[0]);

            Execute(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var promise = Promise.race([promise1, promise2, promise3]).catch(function(r) { testingContext.push(r) });
                    complete3[1](3);
                    complete2[1](2);
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(3, (int)testingContext[0]);

            Execute(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    complete3[1](3);
                    var promise = Promise.race([promise1, promise2, promise3]).catch(function(r) { testingContext.push(r) });
                    complete2[1](2);
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(3, (int)testingContext[0]);

            Execute(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var promise = Promise.race([promise1, promise2, 3]).then(function(r) { testingContext.push(r) });
                    complete2[1](2);
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(3, (int)testingContext[0]);
        }

        [TestMethod]
        public void All()
        {
            ArrayInstance arrayInstance;

            // No action
            Execute(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var promise = Promise.all([promise1, promise2, promise3]).finally(function() { throw 'Called' });
                })()");
            Assert.AreEqual(0, (int)testingContext.Length);

            // Resolve
            Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var promise = Promise.all([promise1, promise2, promise3]).then(function(r) { throw r }, function(r) { throw r });
                    complete1[0](2);
                })()");
            Assert.AreEqual(0, (int)testingContext.Length);

            Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var promise = Promise.all([promise1, promise2, promise3]).then(function(r) { throw r }, function(r) { throw r });
                    complete1[0](2);
                    complete2[0](3);
                })()");
            Assert.AreEqual(0, (int)testingContext.Length);

            Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var promise = Promise.all([promise1, promise2, promise3]).then(function(r) { testingContext.push(r) }, function(r) { throw r });
                    complete3[0](4);
                    complete1[0](2);
                    complete2[0](3);
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            arrayInstance = testingContext[0] as ArrayInstance;
            Assert.IsNotNull(arrayInstance);
            Assert.AreEqual(3, (int)arrayInstance.Length);
            Assert.AreEqual(2, (int)arrayInstance[0]);
            Assert.AreEqual(3, (int)arrayInstance[1]);
            Assert.AreEqual(4, (int)arrayInstance[2]);

            Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var promise = Promise.all([promise1, 3, 4]).then(function(r) { testingContext.push(r) }, function(r) { throw r });
                    complete1[0](2);
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            arrayInstance = testingContext[0] as ArrayInstance;
            Assert.AreEqual(3, (int)arrayInstance.Length);
            Assert.AreEqual(2, (int)arrayInstance[0]);
            Assert.AreEqual(3, (int)arrayInstance[1]);
            Assert.AreEqual(4, (int)arrayInstance[2]);

            Evaluate(@"
                (function()
                {
                    var promise = Promise.all([2, 3, 4]).then(function(r) { testingContext.push(r) }, function(r) { throw r });
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            arrayInstance = testingContext[0] as ArrayInstance;
            Assert.AreEqual(3, (int)arrayInstance.Length);
            Assert.AreEqual(2, (int)arrayInstance[0]);
            Assert.AreEqual(3, (int)arrayInstance[1]);
            Assert.AreEqual(4, (int)arrayInstance[2]);

            // Reject
            Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var promise = Promise.all([promise1, promise2, promise3]).then(function(r) { throw r }, function(r) { testingContext.push(r) });
                    complete1[1](2);
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(2, (int)testingContext[0]);

            Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var promise = Promise.all([promise1, promise2, promise3]).then(function(r) { throw r }, function(r) { testingContext.push(r) });
                    complete2[0](3);
                    complete1[1](2);
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(2, (int)testingContext[0]);

            Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var promise = Promise.all([promise1, 3, promise3]).then(function(r) { throw r }, function(r) { testingContext.push(r) });
                    complete1[1](2);
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(2, (int)testingContext[0]);
        }

        [TestMethod, Timeout(1000)]
        public async Task CreateTask()
        {
            var promise = EvaluatePromise("new Promise(function(){})");
            Assert.IsFalse(promise.CreateTask().IsCompleted);

            promise = EvaluatePromise("Promise.resolve(1);");
            Assert.AreEqual(1, (int)await promise.CreateTask());

            promise = EvaluatePromise("Promise.reject(1);");
            try
            {
                await promise.CreateTask();
                Assert.Fail("Exception was expected");
            }
            catch (JavaScriptException e)
            {
                Assert.AreEqual(1, (int)e.ErrorObject);
            }
        }

        [TestMethod, Timeout(1000)]
        public async Task CreateTask_Delayed()
        {
            var promise = EvaluatePromise("new Promise(function(){})");
            Assert.IsFalse(promise.CreateTask().IsCompleted);

            promise = EvaluatePromise("r = null, new Promise(function(resolve, reject){ r = resolve })");
            var task = promise.CreateTask();
            Assert.IsFalse(task.IsCompleted);
            Evaluate("r(1)");
            Assert.AreEqual(1, (int)await task);

            promise = EvaluatePromise("r = null, new Promise(function(resolve, reject){ r = reject })");
            task = promise.CreateTask();
            Assert.IsFalse(task.IsCompleted);
            try
            {
                Evaluate("r(1)");
                await task;
                Assert.Fail("Exception was expected");
            }
            catch (JavaScriptException e)
            {
                Assert.AreEqual(1, (int)e.ErrorObject);
            }
        }

        [TestMethod]
        public void FromTask()
        {
            InitializeJurassic();

            // Result
            var tcs = new TaskCompletionSource<int>();
            var promise = TypeConverter.ToObject(jurassicScriptEngine, tcs.Task.GetAwaiter());
            jurassicScriptEngine.SetGlobalValue("promise", promise);

            Execute("promise.then(function (r) { testingContext.push(r) }, function (r) { throw r })");
            Assert.AreEqual(0, (int)testingContext.Length);

            tcs.SetResult(100);
            Assert.AreEqual(0, (int)testingContext.Length);

            jurassicScriptEngine.EventLoop.ExecutePendingCallbacks();
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(100, (int)testingContext[0]);

            // Exception
            tcs = new TaskCompletionSource<int>();
            promise = TypeConverter.ToObject(jurassicScriptEngine, tcs.Task.GetAwaiter());
            jurassicScriptEngine.SetGlobalValue("promise", promise);

            Execute("promise.then(function (r) { throw r }, function (r) { testingContext.push(r) })");
            Assert.AreEqual(0, (int)testingContext.Length);

            tcs.SetException(new JavaScriptException(100, 0, null));
            Assert.AreEqual(0, (int)testingContext.Length);

            jurassicScriptEngine.EventLoop.ExecutePendingCallbacks();
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(100, (int)testingContext[0]);
        }

        [TestMethod]
        public void FromTask_Strong()
        {
            InitializeJurassic();

            // Result
            var tcs = new TaskCompletionSource<int>();
            var promise = TypeConverter.ToPromise(jurassicScriptEngine, tcs.Task.GetAwaiter());
            jurassicScriptEngine.SetGlobalValue("promise", promise);

            Execute("promise.then(function (r) { testingContext.push(r) }, function (r) { throw r })");
            Assert.AreEqual(0, (int)testingContext.Length);

            tcs.SetResult(100);
            Assert.AreEqual(0, (int)testingContext.Length);

            jurassicScriptEngine.EventLoop.ExecutePendingCallbacks();
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(100, (int)testingContext[0]);

            // Exception
            tcs = new TaskCompletionSource<int>();
            promise = TypeConverter.ToPromise(jurassicScriptEngine, tcs.Task.GetAwaiter());
            jurassicScriptEngine.SetGlobalValue("promise", promise);

            Execute("promise.then(function (r) { throw r }, function (r) { testingContext.push(r) })");
            Assert.AreEqual(0, (int)testingContext.Length);

            tcs.SetException(new JavaScriptException(100, 0, null));
            Assert.AreEqual(0, (int)testingContext.Length);

            jurassicScriptEngine.EventLoop.ExecutePendingCallbacks();
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(100, (int)testingContext[0]);
        }

        [TestMethod]
        public void FromTask_ExceptionResult()
        {
            InitializeJurassic();

            var tcs = new TaskCompletionSource<int>();

            async Task DoNothingAsync()
            {
                await tcs.Task;
            }

            // Result
            var promise = TypeConverter.ToPromise(jurassicScriptEngine, DoNothingAsync().GetAwaiter());
            jurassicScriptEngine.SetGlobalValue("promise", promise);

            Execute("promise.then(function (r) { testingContext.push(r) }, function (r) { throw r })");
            Assert.AreEqual(0, (int)testingContext.Length);

            tcs.SetResult(100);
            Assert.AreEqual(0, (int)testingContext.Length);

            jurassicScriptEngine.EventLoop.ExecutePendingCallbacks();
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(Undefined.Value, testingContext[0]);

            // Exception
            tcs = new TaskCompletionSource<int>();
            promise = TypeConverter.ToPromise(jurassicScriptEngine, DoNothingAsync().GetAwaiter());
            jurassicScriptEngine.SetGlobalValue("promise", promise);

            Execute("promise.then(function (r) { throw r }, function (r) { testingContext.push(r) })");
            Assert.AreEqual(0, (int)testingContext.Length);

            tcs.SetException(new JavaScriptException(100, 0, null));
            Assert.AreEqual(0, (int)testingContext.Length);

            jurassicScriptEngine.EventLoop.ExecutePendingCallbacks();
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(100, testingContext[0]);
        }

        [TestMethod]
        public void FromTask_NoResult()
        {
            InitializeJurassic();

            // Result
            var promise = TypeConverter.ToPromise(jurassicScriptEngine, new NotifyCompletionWithNoResult());
            jurassicScriptEngine.SetGlobalValue("promise", promise);

            Execute("promise.then(function (r) { testingContext.push(r) }, function (r) { throw r })");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(Undefined.Value, testingContext[0]);
        }
    }
}
