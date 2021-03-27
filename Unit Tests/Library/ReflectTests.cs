using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace UnitTests
{
    /// <summary>
    /// Test the global Reflect object.
    /// </summary>
    [TestClass]
    public class ReflectTests : TestBase
    {
        [TestMethod]
        public void Constructor()
        {
            // Reflect is not a function.
            Assert.AreEqual("TypeError: The new operator requires a function, found a 'object' instead", EvaluateExceptionMessage("new Reflect"));
            Assert.AreEqual("TypeError: 'Reflect' is not a function", EvaluateExceptionMessage("Reflect()"));
        }

        [TestMethod]
        public void apply()
        {
            Assert.AreEqual(1, Evaluate("Reflect.apply(Math.floor, undefined, [1.75])"));
            Assert.AreEqual("hello", Evaluate("Reflect.apply(String.fromCharCode, undefined, [104, 101, 108, 108, 111])"));
            Assert.AreEqual(4, Evaluate("Reflect.apply(RegExp.prototype.exec, /ab/, ['confabulation']).index"));
            Assert.AreEqual("i", Evaluate("Reflect.apply(''.charAt, 'ponies', [3])"));

            // The first argument must be a function.
            Assert.AreEqual("TypeError: Incorrect argument type.", EvaluateExceptionMessage("Reflect.apply(6, undefined, [])"));

            // The third argument must be an array-like.
            //Assert.AreEqual("TypeError: Incorrect argument type.", EvaluateExceptionMessage("Reflect.apply(Math.floor, undefined, 1.75)"));

            // length
            Assert.AreEqual(3, Evaluate("Reflect.apply.length"));
        }

        [TestMethod]
        public void construct()
        {
            Assert.AreEqual(5, Evaluate("Reflect.construct(Number, [5]).valueOf()"));
            Assert.AreEqual(5, Evaluate("Reflect.construct(Number, [5], Number).valueOf()"));
            Assert.AreEqual(true, Evaluate(@"
                function F() { }
                var obj = Reflect.construct(function() { this.y = 1; }, [], F);
                obj.y === 1 && obj instanceof F;"));
            Assert.AreEqual(true, Evaluate("Reflect.construct(function () { return new.target; }, [], Number) === Number"));
            Assert.AreEqual(true, Evaluate(@"
                function F() { }
                var obj = Reflect.construct(Array, [], F);
                obj[2] = 'foo';
                obj.length === 3 && obj instanceof F;"));
            Assert.AreEqual(true, Evaluate(@"
                function F(){}
                var obj = Reflect.construct(RegExp, ['baz','g'], F);
                RegExp.prototype.exec.call(obj, 'foobarbaz')[0] === 'baz' && obj.lastIndex === 9 && obj instanceof F;"));

            // The first parameter must be a constructor.
            Assert.AreEqual("TypeError: undefined cannot be converted to an object", EvaluateExceptionMessage("Reflect.construct(Math)"));
            
            // The second parameter must be an object.
            Assert.AreEqual("TypeError: undefined cannot be converted to an object", EvaluateExceptionMessage("Reflect.construct(Number)"));
            
            // The third parameter must be undefined or a constructor.
            Assert.AreEqual("TypeError: Incorrect argument type.", EvaluateExceptionMessage("Reflect.construct(Number, [5], Math)"));
            
            // length
            Assert.AreEqual(2, Evaluate("Reflect.construct.length"));
        }

        [TestMethod]
        public void defineProperty()
        {
            Assert.AreEqual(5, Evaluate("var x = {}; Reflect.defineProperty(x, 'test', { value: 5 }); x.test"));

            // The first parameter must be an object.
            Assert.AreEqual("TypeError: Reflect.defineProperty called on non-object.",
                EvaluateExceptionMessage("Reflect.defineProperty(5, 'test', { value: 5 })"));

            // The descriptor must be an object.
            Assert.AreEqual("TypeError: Invalid property descriptor '5'.", EvaluateExceptionMessage("Reflect.defineProperty({}, 'test', 5)"));

            // Can be called on a string index.
            Assert.AreEqual(true, Evaluate("Reflect.defineProperty(new String('test'), '1', { value: 'e', writable: false, enumerable: true, configurable: false })"));
            Assert.AreEqual(false, Evaluate("Reflect.defineProperty(new String('test'), '1', { value: 'f', writable: false, enumerable: true, configurable: false })"));

            // length
            Assert.AreEqual(3, Evaluate("Reflect.defineProperty.length"));
        }

        [TestMethod]
        public void deleteProperty()
        {
            Assert.AreEqual(true, Evaluate("var x = { a: 1 }; Reflect.deleteProperty(x, 'a')"));
            Assert.AreEqual(false, Evaluate("var x = { a: 1 }; Reflect.deleteProperty(x, 'a'); 'a' in x"));

            // The first parameter must be an object.
            Assert.AreEqual("TypeError: Reflect.deleteProperty called on non-object.",
                EvaluateExceptionMessage("Reflect.deleteProperty(5, 'test')"));

            // length
            Assert.AreEqual(2, Evaluate("Reflect.deleteProperty.length"));
        }

        [TestMethod]
        public void get()
        {
            Assert.AreEqual(1, Evaluate("Reflect.get({ x: 1, y: 2 }, 'x')"));
            Assert.AreEqual("one", Evaluate("Reflect.get(['zero', 'one'], 1)"));

            // receiver
            Assert.AreEqual(true, Evaluate("var x = { get a() { return this; } }; Reflect.get(x, 'a') === x"));
            Assert.AreEqual(5, Evaluate("Reflect.get({ get a() { return this; } }, 'a', 5).valueOf()"));

            // The first parameter must be an object.
            Assert.AreEqual("TypeError: Reflect.get called on non-object.",
                EvaluateExceptionMessage("Reflect.get(5, 'test')"));

            // length
            Assert.AreEqual(2, Evaluate("Reflect.get.length"));
        }

        [TestMethod]
        public void getOwnPropertyDescriptor()
        {
            Assert.AreEqual(@"{""value"":1,""writable"":true,""enumerable"":true,""configurable"":true}",
                Evaluate("JSON.stringify(Reflect.getOwnPropertyDescriptor({ a: 1 }, 'a'))"));

            // The first parameter must be an object.
            Assert.AreEqual("TypeError: Reflect.getOwnPropertyDescriptor called on non-object.",
                EvaluateExceptionMessage("Reflect.getOwnPropertyDescriptor('foo', 0)"));

            // length
            Assert.AreEqual(2, Evaluate("Reflect.getOwnPropertyDescriptor.length"));
        }

        [TestMethod]
        public void getPrototypeOf()
        {
            Assert.AreEqual(true, Evaluate("Reflect.getPrototypeOf({}) === Object.prototype"));
            Assert.AreEqual(true, Evaluate("Reflect.getPrototypeOf([]) === Array.prototype"));

            // The first parameter must be an object.
            Assert.AreEqual("TypeError: Reflect.getPrototypeOf called on non-object.",
                EvaluateExceptionMessage("Reflect.getPrototypeOf(5)"));

            // length
            Assert.AreEqual(1, Evaluate("Reflect.getPrototypeOf.length"));
        }

        [TestMethod]
        public void has()
        {
            // Regular objects.
            Assert.AreEqual(true, Evaluate("Reflect.has({ a: 1 }, 'a')"));
            Assert.AreEqual(false, Evaluate("Reflect.has({ a: 1 }, 'b')"));

            // Symbols.
            Assert.AreEqual(true, Evaluate("var sym = Symbol('test'); Reflect.has({ [sym]: 1 }, sym)"));
            Assert.AreEqual(false, Evaluate("var sym1 = Symbol('test'); var sym2 = Symbol('test'); Reflect.has({ [sym1]: 1 }, sym2)"));

            // Arrays.
            Assert.AreEqual(true, Evaluate("Reflect.has([5], 'length')"));
            Assert.AreEqual(true, Evaluate("Reflect.has([5], '0')"));
            Assert.AreEqual(false, Evaluate("Reflect.has([5], '1')"));
            Assert.AreEqual(false, Evaluate("Reflect.has([5,,6], '1')"));
            Assert.AreEqual(true, Evaluate("Reflect.has([5,,6], '2')"));

            // Prototypes.
            Assert.AreEqual(true, Evaluate("Reflect.has(new Number(5), 'toString')"));

            // length
            Assert.AreEqual(2, Evaluate("Reflect.has.length"));

            // The first argument must be an object.
            Assert.AreEqual("TypeError: Reflect.has called on non-object.",
                EvaluateExceptionMessage("Reflect.has(5, 'toString')"));
        }

        [TestMethod]
        public void isExtensible()
        {
            Assert.AreEqual(true, Evaluate("Reflect.isExtensible({})"));
            Assert.AreEqual(true, Evaluate("Reflect.isExtensible({a: 1})"));
            Assert.AreEqual(false, Evaluate("Reflect.isExtensible(Object.seal({a: 1}))"));
            Assert.AreEqual(false, Evaluate("Reflect.isExtensible(Object.freeze({a: 1}))"));
            Assert.AreEqual(false, Evaluate("Reflect.isExtensible(Object.preventExtensions({}))"));
            Assert.AreEqual(true, Evaluate("Reflect.isExtensible(new Boolean(true))"));

            // length
            Assert.AreEqual(1, Evaluate("Reflect.isExtensible.length"));

            // Argument must be an object.
            Assert.AreEqual("TypeError: Reflect.isExtensible called on non-object.", EvaluateExceptionMessage("Reflect.isExtensible(true)"));
            Assert.AreEqual("TypeError: Reflect.isExtensible called on non-object.", EvaluateExceptionMessage("Reflect.isExtensible(5)"));
            Assert.AreEqual("TypeError: Reflect.isExtensible called on non-object.", EvaluateExceptionMessage("Reflect.isExtensible('test')"));
            Assert.AreEqual("TypeError: Reflect.isExtensible called on non-object.", EvaluateExceptionMessage("Reflect.isExtensible()"));
            Assert.AreEqual("TypeError: Reflect.isExtensible called on non-object.", EvaluateExceptionMessage("Reflect.isExtensible(undefined)"));
            Assert.AreEqual("TypeError: Reflect.isExtensible called on non-object.", EvaluateExceptionMessage("Reflect.isExtensible(null)"));
        }

        [TestMethod]
        public void ownKeys()
        {
            Assert.AreEqual("z,y,x", Evaluate("Reflect.ownKeys({z: 3, y: 2, x: 1}).toString()"));
            Assert.AreEqual("length", Evaluate("Reflect.ownKeys([]).toString()"));
        
            Execute(@"
                var sym = Symbol('comet')
                var sym2 = Symbol('meteor')
                var obj = {[sym]: 0, 'str': 0, '773': 0, '0': 0,
                           [sym2]: 0, '-1': 0, '8': 0, 'second str': 0}");
            Assert.AreEqual(8, Evaluate("Reflect.ownKeys(obj).length"));
            Assert.AreEqual("0", Evaluate("Reflect.ownKeys(obj)[0]"));
            Assert.AreEqual("8", Evaluate("Reflect.ownKeys(obj)[1]"));
            Assert.AreEqual("773", Evaluate("Reflect.ownKeys(obj)[2]"));
            Assert.AreEqual("str", Evaluate("Reflect.ownKeys(obj)[3]"));
            Assert.AreEqual("-1", Evaluate("Reflect.ownKeys(obj)[4]"));
            Assert.AreEqual("second str", Evaluate("Reflect.ownKeys(obj)[5]"));
            Assert.AreEqual(true, Evaluate("Reflect.ownKeys(obj)[6] === sym"));
            Assert.AreEqual(true, Evaluate("Reflect.ownKeys(obj)[7] === sym2"));
            Assert.AreEqual(@"[""0"",""1"",""2"",""3"",""length""]", Evaluate("JSON.stringify(Reflect.ownKeys(new String('test')))"));

            // The first argument must be an object.
            Assert.AreEqual("TypeError: Reflect.ownKeys called on non-object.",
                EvaluateExceptionMessage("Reflect.ownKeys(5)"));

            // length
            Assert.AreEqual(1, Evaluate("Reflect.ownKeys.length"));
        }

        [TestMethod]
        public void preventExtensions()
        {
            // Simple object
            Assert.AreEqual(true, Evaluate("var x = {a: 1, c: 2}; Reflect.preventExtensions(x)"));
            Assert.AreEqual(2, Evaluate("x.a = 2; x.a"));
            Assert.AreEqual(true, Evaluate("delete x.a"));
            Assert.AreEqual(Undefined.Value, Evaluate("x.b = 6; x.b"));
            Assert.AreEqual(false, Evaluate("Reflect.isExtensible(x)"));
            Assert.AreEqual(PropertyAttributes.FullAccess, EvaluateAccessibility("x", "c"));

            // length
            Assert.AreEqual(1, Evaluate("Reflect.preventExtensions.length"));

            // If the argument is not an object, this method throws a TypeError.
            Assert.AreEqual("TypeError: Reflect.preventExtensions called on non-object.", EvaluateExceptionMessage("Reflect.preventExtensions()"));
            Assert.AreEqual("TypeError: Reflect.preventExtensions called on non-object.", EvaluateExceptionMessage("Reflect.preventExtensions(undefined)"));
            Assert.AreEqual("TypeError: Reflect.preventExtensions called on non-object.", EvaluateExceptionMessage("Reflect.preventExtensions(null)"));
            Assert.AreEqual("TypeError: Reflect.preventExtensions called on non-object.", EvaluateExceptionMessage("Reflect.preventExtensions(5)"));
        }

        [TestMethod]
        public void set()
        {
            Assert.AreEqual(true, Evaluate("var x = { a: 1 }; Reflect.set(x, 'a', 2)"));
            Assert.AreEqual(2, Evaluate("x.a"));
            Assert.AreEqual("goose", Evaluate("var arr = ['duck', 'duck', 'duck']; Reflect.set(x, 2, 'goose'); x[2]"));
            Assert.AreEqual("duck", Evaluate("var arr = ['duck', 'duck', 'duck']; Reflect.set(arr, 'length', 1); arr.toString()"));

            // The property is read-only.
            Assert.AreEqual(false, Evaluate("var x = {}; Reflect.defineProperty(x, 'test', { value: 5, writable: false }); Reflect.set(x, 'test', 2)"));
            Assert.AreEqual(5, Evaluate("x.test"));

            // The property doesn't exist and the object is non-extensible.
            Assert.AreEqual(false, Evaluate("var x = {}; Reflect.preventExtensions(x); Reflect.set(x, 'test', 2)"));
            Assert.AreEqual(Undefined.Value, Evaluate("x.test"));

            // The first argument must be an object.
            Assert.AreEqual("TypeError: Reflect.set called on non-object.",
                EvaluateExceptionMessage("Reflect.set(5, 'a', 2)"));

            // length
            Assert.AreEqual(3, Evaluate("Reflect.set.length"));
        }

        [TestMethod]
        public void setPrototypeOf()
        {
            // Returns true on success.
            Assert.AreEqual(true, Evaluate("var a = {}; Reflect.setPrototypeOf(a, Math)"));
            Assert.AreEqual(true, Evaluate("var a = {}; Reflect.setPrototypeOf(a, null)"));

            // Check that the prototype was changed.
            Assert.AreEqual(true, Evaluate("var a = {}; Reflect.setPrototypeOf(a, Math); Reflect.getPrototypeOf(a) === Math && Object.getPrototypeOf(a) === Math"));
            Assert.AreEqual(true, Evaluate("var a = {}; Reflect.setPrototypeOf(a, null); Reflect.getPrototypeOf(a) === null && Object.getPrototypeOf(a) === null"));
            Assert.AreEqual(5, Evaluate("var a = {}; Reflect.setPrototypeOf(a, Math); a.abs(-5)"));

            // length
            Assert.AreEqual(2, Evaluate("Reflect.setPrototypeOf.length"));

            // The first argument must be an object.
            Assert.AreEqual("TypeError: Reflect.setPrototypeOf called on non-object.",
                EvaluateExceptionMessage("Reflect.setPrototypeOf(5, null)"));

            // Argument must be an object or null.
            Assert.AreEqual("TypeError: Object prototype may only be an Object or null.", EvaluateExceptionMessage("Reflect.setPrototypeOf({}, undefined)"));

            // Object must be extensible.
            Assert.AreEqual(false, Evaluate("Reflect.setPrototypeOf(Object.preventExtensions({}), {})"));

            // No cyclic references.
            Assert.AreEqual(false, Evaluate("var a = {}; Reflect.setPrototypeOf(a, a)"));
        }
    }
}
