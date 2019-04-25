using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Jurassic.Library.PromiseInstance;
using Jurassic.Library;
using Jurassic;
using System.Threading.Tasks;

namespace UnitTests
{
    /// <summary>
    /// Test the global Promise object.
    /// </summary>
    [TestClass]
    public class PromiseTests : TestBase
    {
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
            object result = Evaluate("(function(){ var a = 1; p = new Promise(function(resolve, reject) { }).then(function(r) { a = r }, function(r) { a = r }); return a })()");
            Assert.AreEqual(1, (int)result);

            result = Evaluate("(function(){ var a = 1; p = new Promise(function(resolve, reject) { resolve(2) }).then(function(r) { a = r }, function(r) { a = r }); return a })()");
            Assert.AreEqual(2, (int)result);

            result = Evaluate("(function(){ var a = 1; p = new Promise(function(resolve, reject) { reject(3) }).then(function(r) { a = r }, function(r) { a = r }); return a })()");
            Assert.AreEqual(3, (int)result);

            result = Evaluate("(function(){ var a = 1; p = new Promise(function(resolve, reject) { }).then(function(r) { a = r }); return a })()");
            Assert.AreEqual(1, (int)result);

            result = Evaluate("(function(){ var a = 1; p = new Promise(function(resolve, reject) { resolve(2) }).then(function(r) { a = r }); return a })()");
            Assert.AreEqual(2, (int)result);

            result = Evaluate("(function(){ var a = 1; p = new Promise(function(resolve, reject) { reject(3) }).then(function(r) { a = r }); return a })()");
            Assert.AreEqual(1, (int)result);
        }

        [TestMethod]
        public void Catch()
        {
            object result = Evaluate("(function(){ var a = 1; p = new Promise(function(resolve, reject) { }).catch(function(r) { a = r }); return a })()");
            Assert.AreEqual(1, (int)result);

            result = Evaluate("(function(){ var a = 1; p = new Promise(function(resolve, reject) { resolve(2) }).catch(function(r) { a = r }); return a })()");
            Assert.AreEqual(1, (int)result);

            result = Evaluate("(function(){ var a = 1; p = new Promise(function(resolve, reject) { reject(3) }).catch(function(r) { a = r }); return a })()");
            Assert.AreEqual(3, (int)result);
        }

        [TestMethod]
        public void Finally()
        {
            object result = Evaluate("(function(){ var a = 1; p = new Promise(function(resolve, reject) { }).finally(function(r) { a = r }); return a })()");
            Assert.AreEqual(1, (int)result);

            result = Evaluate("(function(){ var a = 1; p = new Promise(function(resolve, reject) { resolve(2) }).finally(function(r) { a = r }); return a })()");
            Assert.AreEqual(2, (int)result);

            result = Evaluate("(function(){ var a = 1; p = new Promise(function(resolve, reject) { reject(3) }).finally(function(r) { a = r }); return a })()");
            Assert.AreEqual(3, (int)result);
        }

        [TestMethod]
        public void Race()
        {
            // No action
            object result = Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var result = 1;
                    var promise = Promise.race([promise1, promise2, promise3]).finally(function() { result = 2 });
                    return result;
                })()");
            Assert.AreEqual(1, (int)result);

            // Resolve
            result = Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var result = 1;
                    var promise = Promise.race([promise1, promise2, promise3]).then(function(r) { result = r });
                    complete2[1](2);
                    return result;
                })()");
            Assert.AreEqual(1, (int)result);

            result = Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var result = 1;
                    var promise = Promise.race([promise1, promise2, promise3]).then(function(r) { result = r });
                    complete2[0](2);
                    return result;
                })()");
            Assert.AreEqual(2, (int)result);

            result = Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var result = 1;
                    var promise = Promise.race([promise1, promise2, promise3]).then(function(r) { result = r });
                    complete3[0](3);
                    complete2[0](2);
                    return result;
                })()");
            Assert.AreEqual(3, (int)result);

            result = Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var result = 1;
                    complete3[0](3);
                    var promise = Promise.race([promise1, promise2, promise3]).then(function(r) { result = r });
                    complete2[0](2);
                    return result;
                })()");
            Assert.AreEqual(3, (int)result);

            result = Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var result = 1;
                    var promise = Promise.race([promise1, promise2, 3]).then(function(r) { result = r });
                    complete2[0](2);
                    return result;
                })()");
            Assert.AreEqual(3, (int)result);

            // Reject
            result = Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var result = 1;
                    var promise = Promise.race([promise1, promise2, promise3]).catch(function(r) { result = r });
                    complete2[0](2);
                    return result;
                })()");
            Assert.AreEqual(1, (int)result);

            result = Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var result = 1;
                    var promise = Promise.race([promise1, promise2, promise3]).catch(function(r) { result = r });
                    complete2[1](2);
                    return result;
                })()");
            Assert.AreEqual(2, (int)result);

            result = Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var result = 1;
                    var promise = Promise.race([promise1, promise2, promise3]).catch(function(r) { result = r });
                    complete3[1](3);
                    complete2[1](2);
                    return result;
                })()");
            Assert.AreEqual(3, (int)result);

            result = Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var result = 1;
                    complete3[1](3);
                    var promise = Promise.race([promise1, promise2, promise3]).catch(function(r) { result = r });
                    complete2[1](2);
                    return result;
                })()");
            Assert.AreEqual(3, (int)result);

            result = Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var result = 1;
                    var promise = Promise.race([promise1, promise2, 3]).then(function(r) { result = r });
                    complete2[1](2);
                    return result;
                })()");
            Assert.AreEqual(3, (int)result);
        }

        [TestMethod]
        public void All()
        {
            // No action
            object result = Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var result = 1;
                    var promise = Promise.all([promise1, promise2, promise3]).finally(function() { result = 2 });
                    return result;
                })()");
            Assert.AreEqual(1, (int)result);

            // Resolve
            result = Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var result = 1;
                    var promise = Promise.all([promise1, promise2, promise3]).then(function(r) { result = r }, function(r) { throw r });
                    complete1[0](2);
                    return result;
                })()");
            Assert.AreEqual(1, (int)result);

            result = Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var result = 1;
                    var promise = Promise.all([promise1, promise2, promise3]).then(function(r) { result = r }, function(r) { throw r });
                    complete1[0](2);
                    complete2[0](3);
                    return result;
                })()");
            Assert.AreEqual(1, (int)result);

            result = Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var result = 1;
                    var promise = Promise.all([promise1, promise2, promise3]).then(function(r) { result = r }, function(r) { throw r });
                    complete3[0](4);
                    complete1[0](2);
                    complete2[0](3);
                    return result;
                })()");
            var arrayInstance = result as ArrayInstance;
            Assert.IsNotNull(arrayInstance);
            Assert.AreEqual(3, (int)arrayInstance.Length);
            Assert.AreEqual(2, (int)arrayInstance[0]);
            Assert.AreEqual(3, (int)arrayInstance[1]);
            Assert.AreEqual(4, (int)arrayInstance[2]);

            result = Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var result = 1;
                    var promise = Promise.all([promise1, 3, 4]).then(function(r) { result = r }, function(r) { throw r });
                    complete1[0](2);
                    return result;
                })()");
            arrayInstance = result as ArrayInstance;
            Assert.IsNotNull(arrayInstance);
            Assert.AreEqual(3, (int)arrayInstance.Length);
            Assert.AreEqual(2, (int)arrayInstance[0]);
            Assert.AreEqual(3, (int)arrayInstance[1]);
            Assert.AreEqual(4, (int)arrayInstance[2]);

            result = Evaluate(@"
                (function()
                {
                    var result = 1;
                    var promise = Promise.all([2, 3, 4]).then(function(r) { result = r }, function(r) { throw r });
                    return result;
                })()");
            arrayInstance = result as ArrayInstance;
            Assert.IsNotNull(arrayInstance);
            Assert.AreEqual(3, (int)arrayInstance.Length);
            Assert.AreEqual(2, (int)arrayInstance[0]);
            Assert.AreEqual(3, (int)arrayInstance[1]);
            Assert.AreEqual(4, (int)arrayInstance[2]);

            // Reject
            result = Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var result = 1;
                    var promise = Promise.all([promise1, promise2, promise3]).then(function(r) { throw r }, function(r) { result = r });
                    complete1[1](2);
                    return result;
                })()");
            Assert.AreEqual(2, (int)result);

            result = Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var result = 1;
                    var promise = Promise.all([promise1, promise2, promise3]).then(function(r) { throw r }, function(r) { result = r });
                    complete2[0](3);
                    complete1[1](2);
                    return result;
                })()");
            Assert.AreEqual(2, (int)result);

            result = Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var result = 1;
                    var promise = Promise.all([promise1, 3, promise3]).then(function(r) { throw r }, function(r) { result = r });
                    complete1[1](2);
                    return result;
                })()");
            Assert.AreEqual(2, (int)result);
        }

        [TestMethod]
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
    }
}
