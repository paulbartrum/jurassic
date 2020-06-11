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
        [ThreadStatic]
        public static TestingContext testingContext;

        protected override ScriptEngine InitializeScriptEngine()
        {
            var scriptEngine = base.InitializeScriptEngine();
            testingContext = new TestingContext(scriptEngine);
            scriptEngine.SetGlobalValue("testingContext", testingContext);
            return scriptEngine;
        }

        protected override void OnBeforeExecute()
        {
            testingContext.Clear();
        }

        private PromiseInstance EvaluatePromise(string script)
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
            Assert.AreEqual("", Evaluate("var f; var x = new Promise(function(resolve, reject) { f = resolve; }); f.name"));
            Assert.AreEqual("", Evaluate("var f; var x = new Promise(function(resolve, reject) { f = reject; }); f.name"));

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

            promise = EvaluatePromise("var resolver; p = new Promise(function(resolve, reject) { resolver = resolve; }); resolver(2); p");
            Assert.AreEqual(PromiseState.Fulfilled, promise.State);
            Assert.AreEqual(2, promise.Result);

            promise = EvaluatePromise("Promise.resolve(1)");
            Assert.AreEqual(PromiseState.Fulfilled, promise.State);
            Assert.AreEqual(1, (int)promise.Result);

            promise = EvaluatePromise("Promise.resolve()");
            Assert.AreEqual(PromiseState.Fulfilled, promise.State);
            Assert.AreEqual(Undefined.Value, promise.Result);

            // Resolve to a static value.
            Execute(@"
                Promise.resolve('success').then(function(value) {
                    testingContext.push(value);
                }, function(value) {
                    testingContext.push('fail');
                });
                testingContext.push('end of script');
            ");
            Assert.AreEqual(2, (int)testingContext.Length);
            Assert.AreEqual("end of script", testingContext[0]);
            Assert.AreEqual("success", testingContext[1]);

            // Resolve another promise.
            Execute(@"
                var original = Promise.resolve('success');
                var cast = Promise.resolve(original);
                cast.then(function(value) {
                    testingContext.push(value);
                }, function(value) {
                    testingContext.push('fail');
                });
                testingContext.push('identical: ' + (cast === original));
            ");
            Assert.AreEqual(2, (int)testingContext.Length);
            Assert.AreEqual("identical: true", testingContext[0].ToString());
            Assert.AreEqual("success", testingContext[1]);

            Execute(@"
                (function() {
                    Promise.resolve({ ""then"": function() { testingContext.push(""then() called""); } });
                    testingContext.push(""end of script"");
                })()");
            Assert.AreEqual(2, (int)testingContext.Length);
            Assert.AreEqual("end of script", testingContext[0]);
            Assert.AreEqual("then() called", testingContext[1]);

            // Pending callbacks are called at the very end of the execution process.
            Assert.AreEqual(1, Evaluate("var f = 1; Promise.resolve().then(function() { f = 2; }); f"));
            Assert.AreEqual(2, Evaluate("f"));
            Assert.AreEqual(3, Evaluate("var f = 3; eval('Promise.resolve().then(function() { f = 4; })'); f"));
            Assert.AreEqual(4, Evaluate("f"));
            Assert.AreEqual(5, Evaluate("var f = 5; Promise.resolve().then(function() { f = 6; Promise.resolve().then(function() { f = 7; }); }); f"));
            Assert.AreEqual(7, Evaluate("f"));
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
            // A pending promise will not call onResolve or onReject.
            Execute(@"
                (function(){
                    new Promise(function(resolve, reject) { }).then(function(r) { testingContext.push(r) }, function(r) { testingContext.push(r) }); 
                })()");
            Assert.AreEqual(0, (int)testingContext.Length);

            // then() should call the onResolve function if the promise is fulfilled (asynchronously).
            Execute(@"
                (function(){ 
                    new Promise(function(resolve, reject) { resolve('success') }).then(function(r) { testingContext.push(r) }, function(r) { throw r });
                    testingContext.push('end of script');
                })()");
            Assert.AreEqual(2, (int)testingContext.Length);
            Assert.AreEqual("end of script", testingContext[0]);
            Assert.AreEqual("success", testingContext[1]);

            // then() should call the onReject function if the promise is rejected.
            Execute(@"(function() { 
                    new Promise(function(resolve, reject) { reject(3) }).then(function(r) { throw r }, function(r) { testingContext.push(r) });
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(3, (int)testingContext[0]);

            // then() should return a new Promise.
            Assert.AreEqual(false, Evaluate(@"(function() { 
                    p = new Promise(function(resolve, reject) { reject(3) });
                    return p === p.then(function(r) { }, function(r) { });
                })()"));

            // then() should accept parameters that are not functions (treating them as undefined).
            Assert.AreEqual(true, Evaluate(@"(function() { 
                    return new Promise(function(resolve, reject) { }).then(5, 6);
                })()") is PromiseInstance);

            // If then() returns a value, that's the value of the new promise.
            Execute(@"(function() { 
                    new Promise(function(resolve, reject) { resolve(10) })
                        .then(function(r) { return 11; })
                        .then(function(r) { testingContext.push(r) });
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(11, (int)testingContext[0]);

            // If then() doesn't return a value, then the value of the new promise is undefined.
            Execute(@"(function() { 
                    new Promise(function(resolve, reject) { resolve(10) })
                        .then(function(r) { })
                        .then(function(r) { testingContext.push(r) });
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(Undefined.Value, testingContext[0]);

            // The default onResolve is a function that returns the value that was passed in.
            Execute(@"(function() { 
                    new Promise(function(resolve, reject) { resolve(10) })
                        .then()
                        .then(function(r) { testingContext.push(r) });
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(10, (int)testingContext[0]);

            // The default onReject throws the value that was passed in.
            Execute(@"(function() { 
                    new Promise(function(resolve, reject) { reject(new Error(""rejected!"")) })
                        .then()
                        .then(null, function(r) { testingContext.push(r) });
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual("rejected!", ((ErrorInstance)testingContext[0]).Message);

            // Resolving with the promise instance is an error.
            var exceptionMessage = EvaluateExceptionMessage(@"
                (function(){ 
                    var resolver;
                    p = new Promise(function(resolve, reject) { resolver = resolve });
                    p.then(null, function(r) { testingContext.push(r) });
                    resolver(p);
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual("A promise cannot be resolved with itself.", ((ErrorInstance)testingContext[0]).Message);
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

            // finally() doesn't pass any values to the supplied function.
            Execute(@"
                (function(){ 
                    p = new Promise(function(resolve, reject) { reject(2) }).finally(function() { testingContext.push(arguments.length) });
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(0, (int)testingContext[0]);

            // finally() preserves the resolved result.
            Execute(@"
                (function(){ 
                    p = new Promise(function(resolve, reject) { resolve(5) }).finally(function() { }).then(function(r) { testingContext.push(r) });
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(5, (int)testingContext[0]);

            // finally() preserves the rejected result.
            Execute(@"
                (function(){ 
                    p = new Promise(function(resolve, reject) { reject(6) }).finally(function() { }).then(null, function(r) { testingContext.push(r) });
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(6, (int)testingContext[0]);
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
            // No action
            Execute(@"
                (function() {
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
            Execute(@"
                (function() {
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

            Execute(@"
                (function() {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    complete1[0](2);
                    var promise = Promise.all([promise1, promise2, promise3]).then(function(r) { throw r }, function(r) { throw r });
                })()");
            Assert.AreEqual(0, (int)testingContext.Length);

            Execute(@"
                (function() {
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

            Execute(@"
                (function() {
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
            var arrayInstance = testingContext[0] as ArrayInstance;
            Assert.IsNotNull(arrayInstance);
            Assert.AreEqual(3, (int)arrayInstance.Length);
            Assert.AreEqual(2, (int)arrayInstance[0]);
            Assert.AreEqual(3, (int)arrayInstance[1]);
            Assert.AreEqual(4, (int)arrayInstance[2]);

            Execute(@"
                (function() {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    complete3[0](4);
                    var promise = Promise.all([promise1, promise2, promise3]).then(function(r) { testingContext.push(r) }, function(r) { throw r });
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

            Execute(@"
                (function() {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete2;
                    var promise2 = new Promise(function(resolve, reject) { complete2 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    complete3[0](4);
                    complete1[0](2);
                    complete2[0](3);
                    var promise = Promise.all([promise1, promise2, promise3]).then(function(r) { testingContext.push(r) }, function(r) { throw r });
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            arrayInstance = testingContext[0] as ArrayInstance;
            Assert.IsNotNull(arrayInstance);
            Assert.AreEqual(3, (int)arrayInstance.Length);
            Assert.AreEqual(2, (int)arrayInstance[0]);
            Assert.AreEqual(3, (int)arrayInstance[1]);
            Assert.AreEqual(4, (int)arrayInstance[2]);

            Execute(@"
                (function() {
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

            Execute(@"
                (function() {
                    var promise = Promise.all([2, 3, 4]).then(function(r) { testingContext.push(r) }, function(r) { throw r });
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            arrayInstance = testingContext[0] as ArrayInstance;
            Assert.AreEqual(3, (int)arrayInstance.Length);
            Assert.AreEqual(2, (int)arrayInstance[0]);
            Assert.AreEqual(3, (int)arrayInstance[1]);
            Assert.AreEqual(4, (int)arrayInstance[2]);

            // Reject
            Execute(@"
                (function() {
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

            Execute(@"
                (function() {
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

            Execute(@"
                (function() {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    var promise = Promise.all([promise1, 3, promise3]).then(function(r) { throw r }, function(r) { testingContext.push(r) });
                    complete1[1](2);
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(2, (int)testingContext[0]);

            Execute(@"
                (function() {
                    var complete1;
                    var promise1 = new Promise(function(resolve, reject) { complete1 = [ resolve, reject ]; });
                    var complete3;
                    var promise3 = new Promise(function(resolve, reject) { complete3 = [ resolve, reject ]; });
                    complete1[1](2);
                    var promise = Promise.all([promise1, 3, promise3]).then(function(r) { throw r }, function(r) { testingContext.push(r) });
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(2, (int)testingContext[0]);

            // An empty iterable will resolve immediately with an empty array as the resolution.
            Execute(@"
                (function() {
                    Promise.all([]).then(function(r) { testingContext.push(r) }, function(r) { testingContext.push('fail') });
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(0, (int)((ArrayInstance)testingContext[0]).Length);
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
        public void FromTask_Object()
        {
            var tcs = new TaskCompletionSource<object>();
            var promise = ScriptEngine.Promise.FromTask(tcs.Task);
            ScriptEngine.SetGlobalValue("promise", promise);

            Execute("promise.then(function (r) { testingContext.push(r) }, function (r) { throw r })");
            Assert.AreEqual(0, (int)testingContext.Length);

            tcs.SetResult(100);
            Assert.AreEqual(0, (int)testingContext.Length);

            while (ScriptEngine.ExecuteNextEventQueueAction()) { }
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(100, (int)testingContext[0]);
        }

        [TestMethod]
        public void FromTask_Void()
        {
            var tcs = new TaskCompletionSource<object>();
            var promise = ScriptEngine.Promise.FromTask(Task.Delay(0));
            ScriptEngine.SetGlobalValue("promise", promise);

            Execute("promise.then(function (r) { testingContext.push(r) }, function (r) { throw r })");
            Assert.AreEqual(0, (int)testingContext.Length);

            tcs.SetResult(100);
            Assert.AreEqual(0, (int)testingContext.Length);

            while (ScriptEngine.ExecuteNextEventQueueAction()) { }
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(Undefined.Value, testingContext[0]);
        }

        [TestMethod]
        public void FromTask_JavaScriptException()
        {
            var tcs = new TaskCompletionSource<object>();
            var promise = ScriptEngine.Promise.FromTask(tcs.Task);
            ScriptEngine.SetGlobalValue("promise", promise);

            Execute("promise.then(function (r) { throw r }, function (r) { testingContext.push(r) })");
            Assert.AreEqual(0, (int)testingContext.Length);

            tcs.SetException(new JavaScriptException(100, 0, null));
            Assert.AreEqual(0, (int)testingContext.Length);

            while (ScriptEngine.ExecuteNextEventQueueAction()) { }
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(100, (int)testingContext[0]);
        }

        [TestMethod]
        public void FromTask_Exception()
        {
            var tcs = new TaskCompletionSource<object>();
            var promise = ScriptEngine.Promise.FromTask(tcs.Task);
            ScriptEngine.SetGlobalValue("promise", promise);

            Execute("promise.then(function (r) { throw r }, function (r) { testingContext.push(r) })");
            Assert.AreEqual(0, (int)testingContext.Length);

            tcs.SetException(new InvalidOperationException("This is an invalid operation exception."));
            Assert.AreEqual(0, (int)testingContext.Length);

            while (ScriptEngine.ExecuteNextEventQueueAction()) { }
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual("This is an invalid operation exception.", (string)testingContext[0]);
        }

        [TestMethod]
        public void Thenable()
        {
            // Do nothing
            Execute(@"
                (function() {
                    var thenable = {
                        then: function(resolve, reject) {}
                    };
                    new Promise(function(resolve) { resolve(thenable) }).then(function(r) { throw r }, function(r) { throw r })
                })()");

            // Resolve
            Execute(@"
                (function() {
                    var thenable = {
                        then: function(resolve, reject) { resolve(2) }
                    };
                    new Promise(function(resolve) { resolve(thenable) }).then(function(r) { testingContext.push(r) }, function(r) { throw r })
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(2, (int)testingContext[0]);

            // Reject
            Execute(@"
                (function() {
                    var thenable = {
                        then: function(resolve, reject) { reject(2) }
                    };
                    new Promise(function(resolve) { resolve(thenable) }).then(function(r) { throw r }, function(r) { testingContext.push(r) })
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(2, (int)testingContext[0]);

            // Throw
            Execute(@"
                (function() {
                    var thenable = {};
                    Object.defineProperty(thenable, 'then', {
                        get: function() { throw 2 }
                    });
                    new Promise(function(resolve) { resolve(thenable) }).then(function(r) { throw r }, function(r) { testingContext.push(r) })
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(2, (int)testingContext[0]);

            Evaluate(@"
                (function() {
                    var thenable = {
                        then: function(resolve, reject) {
                            throw 2
                        }
                    };
                    new Promise(function(resolve) { resolve(thenable) }).then(function(r) { throw r }, function(r) { testingContext.push(r) })
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(2, (int)testingContext[0]);
        }

        [TestMethod]
        public void All_Thenable()
        {
            ArrayInstance arrayInstance;

            // No action
            Execute(@"
                (function()
                {
                    var complete1;
                    var promise1 = {
                        then: function(resolve, reject) { complete1 = [ resolve, reject ]; }
                    };
                    var complete2;
                    var promise2 = {
                        then: function(resolve, reject) { complete2 = [ resolve, reject ]; }
                    };
                    var complete3;
                    var promise3 = {
                        then: function(resolve, reject) { complete3 = [ resolve, reject ]; }
                    };
                    var promise = Promise.all([promise1, promise2, promise3]).finally(function() { throw 'Called' });
                })()");
            Assert.AreEqual(0, (int)testingContext.Length);

            // Resolve
            Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = {
                        then: function(resolve, reject) { complete1 = [ resolve, reject ]; }
                    };
                    var complete2;
                    var promise2 = {
                        then: function(resolve, reject) { complete2 = [ resolve, reject ]; }
                    };
                    var complete3;
                    var promise3 = {
                        then: function(resolve, reject) { complete3 = [ resolve, reject ]; }
                    };
                    var promise = Promise.all([promise1, promise2, promise3]).then(function(r) { throw r }, function(r) { throw r });
                    complete1[0](2);
                })()");
            Assert.AreEqual(0, (int)testingContext.Length);

            Evaluate(@"
                (function()
                {
                    var promise1 = {
                        then: function(resolve, reject) { resolve(2) }
                    };
                    var complete2;
                    var promise2 = {
                        then: function(resolve, reject) { complete2 = [ resolve, reject ]; }
                    };
                    var complete3;
                    var promise3 = {
                        then: function(resolve, reject) { complete3 = [ resolve, reject ]; }
                    };
                    var promise = Promise.all([promise1, promise2, promise3]).then(function(r) { throw r }, function(r) { throw r });
                })()");
            Assert.AreEqual(0, (int)testingContext.Length);

            Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = {
                        then: function(resolve, reject) { complete1 = [ resolve, reject ]; }
                    };
                    var complete2;
                    var promise2 = {
                        then: function(resolve, reject) { complete2 = [ resolve, reject ]; }
                    };
                    var complete3;
                    var promise3 = {
                        then: function(resolve, reject) { complete3 = [ resolve, reject ]; }
                    };
                    var promise = Promise.all([promise1, promise2, promise3]).then(function(r) { throw r }, function(r) { throw r });
                    complete1[0](2);
                    complete2[0](3);
                })()");
            Assert.AreEqual(0, (int)testingContext.Length);

            Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = {
                        then: function(resolve, reject) { complete1 = [ resolve, reject ]; }
                    };
                    var complete2;
                    var promise2 = {
                        then: function(resolve, reject) { complete2 = [ resolve, reject ]; }
                    };
                    var complete3;
                    var promise3 = {
                        then: function(resolve, reject) { complete3 = [ resolve, reject ]; }
                    };
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
                    var promise1 = {
                        then: function(resolve, reject) { complete1 = [ resolve, reject ]; }
                    };
                    var complete2;
                    var promise2 = {
                        then: function(resolve, reject) { complete2 = [ resolve, reject ]; }
                    };
                    var promise3 = {
                        then: function(resolve, reject) { resolve(4) }
                    };
                    var promise = Promise.all([promise1, promise2, promise3]).then(function(r) { testingContext.push(r) }, function(r) { throw r });
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
                    var promise1 = {
                        then: function(resolve, reject) { resolve(2) }
                    };
                    var promise2 = {
                        then: function(resolve, reject) { resolve(3) }
                    };
                    var promise3 = {
                        then: function(resolve, reject) { resolve(4) }
                    };
                    var promise = Promise.all([promise1, promise2, promise3]).then(function(r) { testingContext.push(r) }, function(r) { throw r });
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
                    var promise1 = {
                        then: function(resolve, reject) { complete1 = [ resolve, reject ]; }
                    };
                    var promise = Promise.all([promise1, 3, 4]).then(function(r) { testingContext.push(r) }, function(r) { throw r });
                    complete1[0](2);
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
                    var promise1 = {
                        then: function(resolve, reject) { complete1 = [ resolve, reject ]; }
                    };
                    var complete2;
                    var promise2 = {
                        then: function(resolve, reject) { complete2 = [ resolve, reject ]; }
                    };
                    var complete3;
                    var promise3 = {
                        then: function(resolve, reject) { complete3 = [ resolve, reject ]; }
                    };
                    var promise = Promise.all([promise1, promise2, promise3]).then(function(r) { throw r }, function(r) { testingContext.push(r) });
                    complete1[1](2);
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(2, (int)testingContext[0]);

            Evaluate(@"
                (function()
                {
                    var complete1;
                    var promise1 = {
                        then: function(resolve, reject) { complete1 = [ resolve, reject ]; }
                    };
                    var complete2;
                    var promise2 = {
                        then: function(resolve, reject) { complete2 = [ resolve, reject ]; }
                    };
                    var complete3;
                    var promise3 = {
                        then: function(resolve, reject) { complete3 = [ resolve, reject ]; }
                    };
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
                    var promise1 = {
                        then: function(resolve, reject) { complete1 = [ resolve, reject ]; }
                    };
                    var complete3;
                    var promise3 = {
                        then: function(resolve, reject) { complete3 = [ resolve, reject ]; }
                    };
                    var promise = Promise.all([promise1, 3, promise3]).then(function(r) { throw r }, function(r) { testingContext.push(r) });
                    complete1[1](2);
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(2, (int)testingContext[0]);

            Evaluate(@"
                (function()
                {
                    var promise1 = {
                        then: function(resolve, reject) { reject(2) }
                    };
                    var complete3;
                    var promise3 = {
                        then: function(resolve, reject) { complete3 = [ resolve, reject ]; }
                    };
                    var promise = Promise.all([promise1, 3, promise3]).then(function(r) { throw r }, function(r) { testingContext.push(r) });
                })()");
            Assert.AreEqual(1, (int)testingContext.Length);
            Assert.AreEqual(2, (int)testingContext[0]);
        }
    }
}