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
        // Proxy for debugging algorithms:
        // 
        // var proxy = new Proxy({ a: 1, b: 2 }, {
        //   ownKeys(target) { console.log('ownKeys', target); return Reflect.ownKeys(target); },
        //   has(target, key) { console.log('has', target, key); return Reflect.has(target, key); },
        //   getOwnPropertyDescriptor(target, prop) { console.log('getOwnPropertyDescriptor', target, prop); return Reflect.getOwnPropertyDescriptor(target, prop); },
        //   get(target, prop, receiver) { console.log('get', target, prop, receiver); return Reflect.get(target, prop, receiver); },
        //   isExtensible(target) { console.log('isExtensible', target); return Reflect.isExtensible(target); }
        // });

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
        public void revocable()
        {
            // Proxy.revocable(target, handler)
            // Both parameters must be objects.
            Assert.AreEqual("TypeError: Cannot create proxy with a non-object as target or handler.", EvaluateExceptionMessage("Proxy.revocable()"));
            Assert.AreEqual("TypeError: Cannot create proxy with a non-object as target or handler.", EvaluateExceptionMessage("Proxy.revocable({})"));
            Assert.AreEqual("TypeError: Cannot create proxy with a non-object as target or handler.", EvaluateExceptionMessage("Proxy.revocable({}, 5)"));

            // The properties on the revocable object are regular enumerable properties (including the revoke() function!)
            Assert.AreEqual(@"{""value"":{},""writable"":true,""enumerable"":true,""configurable"":true}",
                Evaluate("JSON.stringify(Object.getOwnPropertyDescriptor(Proxy.revocable({}, {}), 'proxy'))"));
            Assert.AreEqual(@"{""writable"":true,""enumerable"":true,""configurable"":true}",
                Evaluate("JSON.stringify(Object.getOwnPropertyDescriptor(Proxy.revocable({}, {}), 'revoke'))"));

            // Check that the revocable proxy works.
            Assert.AreEqual(2, Evaluate(@"
                var revocable = Proxy.revocable({ a: 1 }, {
                  getPrototypeOf(target) {
                    return { a: 2 };
                  }
                });
                var proxy = revocable.proxy;
                Object.getPrototypeOf(proxy).a"));

            // Check the revocable proxy doesn't work after being revoked.
            Assert.AreEqual("TypeError: Cannot call 'getPrototypeOf' on a proxy that has been revoked.", EvaluateExceptionMessage(@"
                var revocable = Proxy.revocable({ a: 1 }, {
                  getPrototypeOf(target) {
                    return { a: 2 };
                  }
                });
                var proxy = revocable.proxy;
                revocable.revoke();
                Object.getPrototypeOf(proxy)"));
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

            Assert.AreEqual("TypeError: 'getOwnPropertyDescriptor' on proxy: trap returned descriptor for property 'a' that is incompatible with the existing property in the proxy target.", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Object.defineProperty({ a: 1 }, 'a', { configurable: false }), {
                  getOwnPropertyDescriptor(target, prop) {
                    return { configurable: false, value: 5 };
                  }
                });
                JSON.stringify(Reflect.getOwnPropertyDescriptor(proxy, 'a'))"));
            Assert.AreEqual("TypeError: 'getOwnPropertyDescriptor' on proxy: trap reported non-configurability for property 'a' which is either non-existent or configurable in the proxy target.", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Object.defineProperty({ a: 1 }, 'a', { configurable: true }), {
                  getOwnPropertyDescriptor(target, prop) {
                    return { configurable: false, value: 5 };
                  }
                });
                JSON.stringify(Reflect.getOwnPropertyDescriptor(proxy, 'a'))"));
            Assert.AreEqual("TypeError: 'getOwnPropertyDescriptor' on proxy: trap reported non-configurable and writable for property 'a' which is non-configurable, non-writable in the proxy target.", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Object.defineProperty({ a: 1 }, 'a', { configurable: false, writable: true, enumerable: true }), {
                  getOwnPropertyDescriptor(target, prop) {
                    return { configurable: false, writable: false, value: 5, enumerable: true };
                  }
                });
                JSON.stringify(Reflect.getOwnPropertyDescriptor(proxy, 'a'))"));

            Assert.AreEqual("TypeError: 'getOwnPropertyDescriptor' on proxy: trap returned undefined for property 'a' which is non-configurable in the proxy target.", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Object.defineProperty({ a: 1 }, 'a', { configurable: false }), {
                  getOwnPropertyDescriptor(target, prop) {}
                });
                JSON.stringify(Reflect.getOwnPropertyDescriptor(proxy, 'a'))"));

            Assert.AreEqual("TypeError: 'getOwnPropertyDescriptor' on proxy: trap returned undefined for property 'a' which exists in the non-extensible proxy target.", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Object.preventExtensions({ a: 1 }), {
                  getOwnPropertyDescriptor(target, prop) {}
                });
                JSON.stringify(Reflect.getOwnPropertyDescriptor(proxy, 'a'))"));

            Assert.AreEqual("TypeError: 'getOwnPropertyDescriptor' on proxy: trap returned neither object nor undefined for property 'a'.", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Object.preventExtensions({ a: 1 }), {
                  getOwnPropertyDescriptor(target, prop) { return 5; }
                });
                JSON.stringify(Reflect.getOwnPropertyDescriptor(proxy, 'a'))"));
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

        [TestMethod]
        public void has()
        {
            // Check the various methods for checking if a property exists.
            Assert.AreEqual("false/true/false/true/false/true", Evaluate(@"
                var proxy = new Proxy({ a: 1, b: 2 }, {
                  has(target, key) {
                    return key !== 'a';
                  }
                });
                ('a' in proxy) + '/' + ('b' in proxy) + '/' +
                ('a' in Object.create(proxy)) + '/' + ('b' in Object.create(proxy)) + '/' +
                Reflect.has(proxy, 'a') + '/' + Reflect.has(proxy, 'b')"));
            Assert.AreEqual("ReferenceError: a is not defined.", EvaluateExceptionMessage(@"with (proxy) { (a); }"));
            Assert.AreEqual(2, Evaluate(@"with (proxy) { (b); }"));

            // A property cannot be reported as non-existent, if it exists as a non-configurable own property of the target object.
            Assert.AreEqual("TypeError: 'has' on proxy: trap returned falsish for property 'a' which exists in the proxy target as non-configurable.", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Object.defineProperty({ }, 'a', { value: 10, configurable: false }), {
                  has(target, key) {
                    return false;
                  }
                });
                'a' in proxy"));

            // A property cannot be reported as non-existent, if it exists as an own property of the target object and the target object is not extensible.
            Assert.AreEqual("TypeError: 'has' on proxy: trap returned falsish for property 'a' but the proxy target is not extensible.", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Object.preventExtensions({ a: 1 }), {
                  has(target, key) {
                    return false;
                  }
                });
                'a' in proxy"));
        }

        [TestMethod]
        public void get()
        {
            // Check normal case.
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
                    return Reflect.get(target, prop, receiver);
                  }
                };

                const proxy = new Proxy(target, handler);
                proxy.notProxied + ', ' + proxy.proxied;"));

            // The value reported for a property must be the same as the value of the corresponding
            // target object property if the target object property is a non-writable,
            // non-configurable own data property.
            Assert.AreEqual("TypeError: 'get' on proxy: property 'a' is a read-only and non-configurable data property on the proxy target but the proxy did not return its actual value (expected '10' but got '99').", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Object.defineProperty({ }, 'a', { value: 10, writable: false, configurable: false }), {
                  get(target) {
                    return 99;
                  }
                });
                proxy.a;"));

            // The value reported for a property must be undefined if the corresponding target
            // object property is a non-configurable own accessor property that has undefined as
            // its [[Get]] attribute.
            Assert.AreEqual("TypeError: 'get' on proxy: property 'a' is a non-configurable accessor property on the proxy target and does not have a getter function, but the trap did not return 'undefined' (got '99').", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Object.defineProperty({ }, 'a', { configurable: false, set: function(val) { } }), {
                  get(target) {
                    return 99;
                  }
                });
                proxy.a;"));

            // Check 'get' is called on the prototype.
            Assert.AreEqual(5, Evaluate(@"
                var proxied = { };
                var proxy = Object.create(new Proxy(proxied, {
                 get: function (t, k, r) {
                   return t === proxied && k === 'foo' && r === proxy && 5;
                 }
                }));
                proxy.foo;"));
        }

        [TestMethod]
        public void set()
        {
            // Check normal case.
            Assert.AreEqual(20, Evaluate(@"
                const proxy = new Proxy({ a: 50 }, {
                  set(obj, prop, value) {
                    obj[prop] = value * 2;
                  }
                });
                proxy.a = 10;
                proxy.a"));

            // Cannot change the value of a property to be different from the value of the
            // corresponding target object property if the corresponding target object property is
            // a non-writable, non-configurable own data property.
            Assert.AreEqual("TypeError: 'set' on proxy: trap returned truish for property 'a' which exists in the proxy target as a non-configurable and non-writable data property with a different value.", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Object.defineProperty({ }, 'a', { value: 10, writable: false, configurable: false }), {
                  set(obj, prop, value) {
                    return true;
                  }
                });
                proxy.a = 100;"));

            // Cannot set the value of a property if the corresponding target object property is a
            // non-configurable own accessor property that has undefined as its [[Set]] attribute.
            Assert.AreEqual("TypeError: 'set' on proxy: trap returned truish for property 'a' which exists in the proxy target as a non-configurable and non-writable accessor property without a setter.", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Object.defineProperty({ }, 'a', { configurable: false, get: function(val) { } }), {
                  set(obj, prop, value) {
                    return true;
                  }
                });
                proxy.a = 100;"));
        }

        [TestMethod]
        public void deleteProperty()
        {
            // Check normal case.
            Assert.AreEqual("true, 1", Evaluate(@"
                var proxy = new Proxy({ }, {
                  deleteProperty(obj, prop) {
                    obj[prop] = 1;
                    return true;
                  }
                });
                (delete proxy.test) + ', ' + proxy.test;"));

            // A property cannot be reported as deleted, if it exists as a non-configurable own
            // property of the target object.
            Assert.AreEqual("TypeError: 'deleteProperty' on proxy: trap returned truish for property 'a' which is non-configurable in the proxy target.", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Object.defineProperty({ }, 'a', { value: 10, configurable: false }), {
                  deleteProperty(obj, prop) {
                    return true;
                  }
                });
                delete proxy.a;"));

            // A property cannot be reported as deleted, if it exists as an own property of the
            // target object and the target object is non-extensible.
            Assert.AreEqual("TypeError: 'deleteProperty' on proxy: trap returned truish for property 'a' but the proxy target is non-extensible.", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Object.preventExtensions({ a: 1 }), {
                  deleteProperty(obj, prop) {
                    return true;
                  }
                });
                delete proxy.a;"));
        }

        [TestMethod]
        public void ownKeys()
        {
            // Check normal case.
            Assert.AreEqual("a,c,d", Evaluate(@"
                var proxy = new Proxy({ a: 1, b: 2 }, {
                  ownKeys(target) {
                    return [ 'a', 'c', 'd' ];
                  }
                });
                Reflect.ownKeys(proxy).join(',');"));
            Assert.AreEqual("a,c,d", Evaluate(@"Object.getOwnPropertyNames(proxy).join(',');"));
            Assert.AreEqual("a", Evaluate(@"Object.keys(proxy).join(',');"));
            Assert.AreEqual("a,", Evaluate(@"var result = ''; for (var key in proxy) result += key + ','; result"));

            // Callback must return an object.
            Assert.AreEqual("TypeError: 'ownKeys' on proxy: trap returned non-object ('5').", EvaluateExceptionMessage(@"
                var proxy = new Proxy({ a: 1, b: 2 }, {
                  ownKeys(target) { return 5; }
                });
                Reflect.ownKeys(proxy).join(',');"));

            // Callback must return an object with strings or Symbols.
            Assert.AreEqual("TypeError: 3 is not a valid property name.", EvaluateExceptionMessage(@"
                var proxy = new Proxy({ a: 1, b: 2 }, {
                  ownKeys(target) {
                    return [ 'c', 3, 'd' ];
                  }
                });
                Object.getOwnPropertyNames(proxy).join(',');"));

            // Callback must return an object with no duplicates.
            Assert.AreEqual("TypeError: 'ownKeys' on proxy: trap returned duplicate entries.", EvaluateExceptionMessage(@"
                var proxy = new Proxy({ a: 1, b: 2 }, {
                  ownKeys(target) {
                    return [ 'c', 'c' ];
                  }
                });
                Object.getOwnPropertyNames(proxy).join(',');"));

            // Proxy must return any non-configurable keys on the target.
            Assert.AreEqual("TypeError: 'ownKeys' on proxy: trap result did not include 'a'.", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Object.defineProperty({ a: 1, b: 2 }, 'a', { configurable: false }), {
                  ownKeys(target) {
                    return [ 'c', 'd' ];
                  }
                });
                Object.getOwnPropertyNames(proxy).join(',');"));

            // Proxy must include all target keys if target is non-extensible.
            Assert.AreEqual("TypeError: 'ownKeys' on proxy: trap result did not include 'a'.", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Object.preventExtensions({ a: 1, b: 2 }), {
                  ownKeys(target) {
                    return [ ];
                  }
                });
                Object.getOwnPropertyNames(proxy).join(',');"));

            // Proxy cannot return extra keys if target is non-extensible.
            Assert.AreEqual("TypeError: 'ownKeys' on proxy: trap returned extra keys but proxy target is non-extensible.", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Object.preventExtensions({ a: 1, b: 2 }), {
                  ownKeys(target) {
                    return [ 'a', 'b', 'c' ];
                  }
                });
                Object.getOwnPropertyNames(proxy).join(',');"));
        }

        [TestMethod]
        public void apply()
        {
            // Check normal case.
            Assert.AreEqual(64, Evaluate(@"
                var proxy = new Proxy(Math.pow, {
                  apply(target, thisArg, argumentsList) {
                    return target(argumentsList[0], argumentsList[1]);
                  }
                });
                proxy(2, 6);"));

            // Check arguments.
            Assert.AreEqual(99, Evaluate(@"
                var proxy = new Proxy(Math.pow, {
                  bubbles: 99,
                  apply(target, thisArg, argumentsList) {
                    return this.bubbles;
                  }
                });
                var container = { proxy: proxy };
                container.proxy(2, 6);"));
            Assert.AreEqual(true, Evaluate(@"
                var proxy = new Proxy(Math.pow, {
                  apply(target, thisArg, argumentsList) {
                    return thisArg === container;
                  }
                });
                var container = { proxy: proxy };
                container.proxy(2, 6);"));

            // The proxy is only callable if the target is callable.
            Assert.AreEqual("TypeError: 'proxy' is not a function", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Math, {
                  apply(target, thisArg, argumentsList) {
                    return target.pow(argumentsList[0], argumentsList[1]);
                  }
                });
                proxy(2, 6);"));
        }

        [TestMethod]
        public void construct()
        {
            // Check normal case.
            Assert.AreEqual("haha", Evaluate(@"
                var proxy = new Proxy(Error, {
                  construct(target, args) {
                    return new String(args[0]);
                  }
                });
                new proxy('haha').toString();"));

            // No callback.
            Assert.AreEqual("test", Evaluate(@"
                var proxy = new Proxy(String, {});
                new proxy('test').toString();"));

            // Can't use 'new' on built-in functions.
            Assert.AreEqual("TypeError: proxy is not a constructor.", EvaluateExceptionMessage(@"
                var proxy = new Proxy(Math.pow, {
                  construct(target, args) {
                    return new String('test');
                  }
                });
                new proxy(2, 6);"));

            // construct() function must return an object.
            Assert.AreEqual("TypeError: 'construct' on proxy: trap returned non-object ('1').", EvaluateExceptionMessage(@"
                var proxy = new Proxy(String, {
                  construct(target, args) {
                    return 1;
                  }
                });
                new proxy();"));
        }
    }
}
