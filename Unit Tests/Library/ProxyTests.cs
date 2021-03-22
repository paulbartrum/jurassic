using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace UnitTests
{
    /// <summary>
    /// Test the Proxy object.
    /// </summary>
    [TestClass]
    public class ProxyTests : TestBase
    {
        [TestMethod]
        public void Constructor()
        {
            // new Proxy(target, handler)


            // Both parameters must be objects.
            Assert.AreEqual("TypeError: Cannot create proxy with a non-object as target or handler.", EvaluateExceptionMessage("new Proxy()"));
            Assert.AreEqual("TypeError: Cannot create proxy with a non-object as target or handler.", EvaluateExceptionMessage("new Proxy({})"));
            Assert.AreEqual("TypeError: Cannot create proxy with a non-object as target or handler.", EvaluateExceptionMessage("new Proxy({}, 5)"));

            // Proxy() cannot be called as a function.
            Assert.AreEqual("TypeError: Constructor Proxy requires 'new'", EvaluateExceptionMessage("Proxy()"));

            // name, length and prototype.
            Assert.AreEqual("Proxy", Evaluate("Proxy.name"));
            Assert.AreEqual(2, Evaluate("Proxy.length"));
            Assert.AreEqual(Undefined.Value, Evaluate("Proxy.prototype"));
        }

        [TestMethod]
        [Ignore]
        public void get()
        {
            Assert.AreEqual("original value, replaced value", Evaluate(@"
                const target = {
                  notProxied: 'original value',
                  proxied: 'original value'
                };

                const handler = {
                  get: function(target, prop, receiver) {
                    if (prop === 'proxied') {
                      return 'replaced value';
                    }
                    return Reflect.get.apply(null, arguments);
                  }
                };

                const proxy = new Proxy(target, handler);

                proxy.notProxied + ', ' + proxy.proxied;"));
        }

        [TestMethod]
        public void getPrototypeOf()
        {
            // The trap can return a new prototype.
            Assert.AreEqual(2, Evaluate(@"
                var proxy = new Proxy({ a: 1 }, {
                  getPrototypeOf(target) {
                    return { a: 2 };
                  }
                });
                Object.getPrototypeOf(proxy).a;"));

            // The trap can return null.
            Assert.AreEqual(Null.Value, Evaluate(@"
                var proxy = new Proxy({ a: 1 }, {
                  getPrototypeOf(target) {
                    return null;
                  }
                });
                Object.getPrototypeOf(proxy);"));

            // If getPrototypeOf doesn't exist, then it falls back to the default behaviour.
            Assert.AreEqual(true, Evaluate(@"
                var proxy = new Proxy({ a: 1 }, { });
                Object.getPrototypeOf(proxy) === Object.getPrototypeOf({});"));

            // getPrototypeOf() must return an object.
            Assert.AreEqual("TypeError: 'getPrototypeOf' on proxy: trap returned neither object nor null.", EvaluateExceptionMessage(@"
                var proxy = new Proxy({ a: 1 }, {
                  getPrototypeOf(target) {
                    return 5;
                  }
                });
                Object.getPrototypeOf(proxy);"));

            // getPrototypeOf must be a function.
            Assert.AreEqual("TypeError: '2' returned for property 'getPrototypeOf' of object '[object Object]' is not a function.", EvaluateExceptionMessage(@"
                var proxy = new Proxy({ a: 1 }, {
                  getPrototypeOf: 2
                });
                Object.getPrototypeOf(proxy);"));

            // If the target is non-extensible, then getPrototypeOf must return the unaltered prototype.
            Assert.AreEqual("TypeError: 'getPrototypeOf' on proxy: proxy target is non-extensible but the trap did not return its actual prototype.", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Object.preventExtensions({ a: 1 }), {
                  getPrototypeOf(target) {
                    return { a: 2 };
                  }
                });
                Object.getPrototypeOf(proxy).a;"));

            // If the target is non-extensible, then getPrototypeOf must return the unaltered prototype.
            Assert.AreEqual(true, Evaluate(@"
                var proxy = new Proxy(Object.preventExtensions({ a: 1 }), {
                  getPrototypeOf(target) {
                    return Object.getPrototypeOf(target);
                  }
                });
                Object.getPrototypeOf(proxy) === Object.getPrototypeOf({});"));
        }

        [TestMethod]
        public void setPrototypeOf()
        {
            // The trap can set a different prototype.
            Assert.AreEqual(3, Evaluate(@"
                var proxy = new Proxy({ a: 1 }, {
                  setPrototypeOf(target, prototype) {
                    Object.setPrototypeOf(target, { a: 3 })
                    return true;
                  }
                });
                Object.setPrototypeOf(proxy, { a: 2 });
                Object.getPrototypeOf(proxy).a"));

            // If setPrototypeOf doesn't exist, then it falls back to the default behaviour.
            Assert.AreEqual(2, Evaluate(@"
                var target = { a: 1 };
                var proxy = new Proxy(target, { });
                Object.setPrototypeOf(proxy, { a: 2 });
                Object.getPrototypeOf(target).a"));

            // The trap can return false to indicate an error.
            Assert.AreEqual(false, Evaluate(@"
                var proxy = new Proxy({ a: 1 }, {
                  setPrototypeOf(target, prototype) {
                    return false;
                  }
                });
                Reflect.setPrototypeOf(proxy, { a: 2 })"));

            // Returning false get translated to an error when calling Object.setPrototypeOf().
            Assert.AreEqual("TypeError: 'setPrototypeOf' on proxy: trap returned falsish.", EvaluateExceptionMessage(@"
                var proxy = new Proxy({ a: 1 }, {
                  setPrototypeOf(target, prototype) {
                    return false;
                  }
                });
                Object.setPrototypeOf(proxy, { a: 2 })"));
        }

        [TestMethod]
        public void isExtensible()
        {
            // Can return true if the target is extensible.
            Assert.AreEqual(true, Evaluate(@"
                var proxy = new Proxy({ a: 1 }, {
                  isExtensible(target) {
                    return true;
                  }
                });
                Reflect.isExtensible(proxy);"));

            // Can return false if the target is not extensible.
            Assert.AreEqual(false, Evaluate(@"
                var proxy = new Proxy(Object.preventExtensions({ a: 1 }), {
                  isExtensible(target) {
                    return false;
                  }
                });
                Reflect.isExtensible(proxy);"));

            // Can't return false if the target is actually extensible.
            Assert.AreEqual("TypeError: 'isExtensible' on proxy: trap result does not reflect extensibility of proxy target (which is 'true').", EvaluateExceptionMessage(@"
                var proxy = new Proxy({ a: 1 }, {
                  isExtensible(target) {
                    return false;
                  }
                });
                Reflect.isExtensible(proxy);"));
        }

        [TestMethod]
        public void preventExtensions()
        {
            // OK to return true if we actually did call preventExtensions() on the target.
            Assert.AreEqual(true, Evaluate(@"
                var proxy = new Proxy({ a: 1 }, {
                  preventExtensions(target) {
                    Object.preventExtensions(target);
                    return true;
                  }
                });
                Reflect.preventExtensions(proxy);"));

            // Reflect.preventExtensions will return false if preventExtensions returns false.
            Assert.AreEqual(false, Evaluate(@"
                var proxy = new Proxy({ a: 1 }, {
                  preventExtensions(target) {
                    return false;
                  }
                });
                Reflect.preventExtensions(proxy);"));

            // Object.preventExtensions will throw an error if preventExtensions returns false.
            Assert.AreEqual("TypeError: 'preventExtensions' on proxy: trap returned falsish.", EvaluateExceptionMessage(@"
                var proxy = new Proxy({ a: 1 }, {
                  preventExtensions(target) {
                    return false;
                  }
                });
                Object.preventExtensions(proxy);"));

            // Object.seal will throw an error if preventExtensions returns false.
            Assert.AreEqual("TypeError: 'preventExtensions' on proxy: trap returned falsish.", EvaluateExceptionMessage(@"
                var proxy = new Proxy({ a: 1 }, {
                  preventExtensions(target) {
                    return false;
                  }
                });
                Object.seal(proxy);"));

            // If preventExtensions returns true then the target must be non-extensible.
            Assert.AreEqual("TypeError: 'preventExtensions' on proxy: trap returned truish but the proxy target is extensible.", EvaluateExceptionMessage(@"
                var proxy = new Proxy({ a: 1 }, {
                  preventExtensions(target) {
                    return true;
                  }
                });
                Reflect.preventExtensions(proxy);"));
        }

        [TestMethod]
        public void getOwnPropertyDescriptor()
        {
            Assert.AreEqual(@"{""value"":5,""writable"":false,""enumerable"":true,""configurable"":true}", Evaluate(@"
                var proxy = new Proxy({ a: 1 }, {
                  getOwnPropertyDescriptor(target, prop) {
                    return { configurable: true, enumerable: true, value: 5 };
                  }
                });
                JSON.stringify(Reflect.getOwnPropertyDescriptor(proxy, 'test'))"));
        }

        [TestMethod]
        public void defineProperty()
        {
            // The trap can throw an exception.
            Assert.AreEqual("Error: Key cannot start with underscore.", EvaluateExceptionMessage(@"
                var proxy = new Proxy({ a: 1 }, {
                  defineProperty(target, key, descriptor) {
                    if (key[0] === '_')
                      throw new Error('Key cannot start with underscore.');
                    return Object.defineProperty(target, key, descriptor);
                  }
                });
                Reflect.defineProperty(proxy, '_test', { value: 5 })"));

            // If defineProperty doesn't exist, then it falls back to the default behaviour.
            Assert.AreEqual(2, Evaluate(@"
                var target = { a: 1 };
                var proxy = new Proxy(target, { });
                Reflect.defineProperty(proxy, 'a', { value: 2 });
                target.a"));

            // Reflect.defineProperty is okay with defineProperty returning false.
            Assert.AreEqual(false, Evaluate(@"
                var proxy = new Proxy({ a: 1 }, {
                  defineProperty(target, key, descriptor) {
                    return false;
                  }
                });
                Reflect.defineProperty(proxy, 'test', { value: 5 })"));

            // Object.defineProperty throws if defineProperty returns false.
            Assert.AreEqual("TypeError: 'defineProperty' on proxy: trap returned falsish for property 'test'.", EvaluateExceptionMessage(@"
                var proxy = new Proxy({ a: 1 }, {
                  defineProperty(target, key, descriptor) {
                    return false;
                  }
                });
                Object.defineProperty(proxy, 'test', { value: 5 })"));

            // A property cannot be non-configurable, unless there exists a corresponding non-configurable own property of the target object.
            Assert.AreEqual("TypeError: 'defineProperty' on proxy: trap returned truish for defining non-configurable property 'a' which is either non-existent or configurable in the proxy target.", EvaluateExceptionMessage(@"
                var proxy = new Proxy({ a: 1 }, {
                  defineProperty(target, key, descriptor) {
                    return true;
                  }
                });
                Object.defineProperty(proxy, 'a', { value: 5, configurable: false })"));
            Assert.AreEqual("TypeError: 'defineProperty' on proxy: trap returned truish for defining non-configurable property 'b' which is either non-existent or configurable in the proxy target.", EvaluateExceptionMessage(@"
                var proxy = new Proxy({ a: 1 }, {
                  defineProperty(target, key, descriptor) {
                    return true;
                  }
                });
                Object.defineProperty(proxy, 'b', { value: 5, configurable: false })"));
            Assert.AreEqual("TypeError: 'defineProperty' on proxy: trap returned truish for adding property 'a' that is incompatible with the existing property in the proxy target.", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Object.defineProperty({ }, 'a', { value: 1, configurable: false }), {
                  defineProperty(target, key, descriptor) {
                    return true;
                  }
                });
                Object.defineProperty(proxy, 'a', { value: 5, configurable: false })"));
            Assert.AreEqual(true, Evaluate(@"
                var proxy = new Proxy(Object.defineProperty({ }, 'a', { value: 1, configurable: false }), {
                  defineProperty(target, key, descriptor) {
                    return true;
                  }
                });
                Reflect.defineProperty(proxy, 'a', { value: 1, configurable: false })"));   // Doesn't error because it matches the existing property.

            // A property cannot be added, if the target object is not extensible.
            Assert.AreEqual("TypeError: 'defineProperty' on proxy: trap returned truish for adding property 'b' to the non-extensible proxy target.", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Object.preventExtensions({ a: 1 }), {
                  defineProperty(target, key, descriptor) {
                    return true;
                  }
                });
                Object.defineProperty(proxy, 'b', { value: 5 })"));

            // A non-configurable property cannot be non-writable, unless there exists a corresponding non-configurable, non-writable own property of the target object.
            Assert.AreEqual("TypeError: 'defineProperty' on proxy: trap returned truish for defining non-configurable property 'x' which cannot be non-writable, unless there exists a corresponding non-configurable, non-writable own property of the target object.", EvaluateExceptionMessage(@"
                var target = { };
                Object.defineProperty(target, 'x', { value: 10, configurable: false, writable: true });
                var proxy = new Proxy(target, {
                  defineProperty(target, key, descriptor) {
                    return true;
                  }
                });
                Reflect.defineProperty(proxy, 'x', { value: 11, writable: false })"));
            Assert.AreEqual(true, Evaluate(@"
                var target = { };
                Object.defineProperty(target, 'x', { value: 10, configurable: false, writable: true });
                var proxy = new Proxy(target, {
                  defineProperty(target, key, descriptor) {
                    return true;
                  }
                });
                Reflect.defineProperty(proxy, 'x', { value: 11, writable: true })"));
            Assert.AreEqual(true, Evaluate(@"
                var target = { };
                Object.defineProperty(target, 'x', { value: 10, configurable: true, writable: true });
                var proxy = new Proxy(target, {
                  defineProperty(target, key, descriptor) {
                    return true;
                  }
                });
                Reflect.defineProperty(proxy, 'x', { value: 11, writable: false })"));
        }
    }
}
