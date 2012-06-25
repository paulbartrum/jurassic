using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace UnitTests
{
    /// <summary>
    /// Test the dynamic keyword.
    /// </summary>
    [TestClass]
    public class DynamicMetaObjectTests
    {
        private class DynamicMetaObjectWrapper : System.Dynamic.DynamicObject
        {
            private ObjectInstance obj;

            public DynamicMetaObjectWrapper(ObjectInstance obj)
            {
                this.obj = obj;
            }

            /// <summary>
            /// Provides the implementation for operations that get member values.
            /// </summary>
            /// <param name="binder"> Provides information about the object that called the dynamic
            /// operation. </param>
            /// <param name="result"> The result of the get operation. </param>
            /// <returns> <c>true</c> if the operation is successful; otherwise, <c>false</c>. </returns>
            public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
            {
                result = this.obj[binder.Name];
                if (TypeUtilities.IsUndefined(result))
                    result = Undefined.Value;
                return true;
            }

            /// <summary>
            /// Provides the implementation for operations that set member values.
            /// </summary>
            /// <param name="binder"> Provides information about the object that called the dynamic
            /// operation. </param>
            /// <param name="value"> The value to set to the member. </param>
            /// <returns> <c>true</c> if the operation is successful; otherwise, <c>false</c>. </returns>
            public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value)
            {
                this.obj[binder.Name] = value;
                return true;
            }

            /// <summary>
            /// Provides the implementation for operations that get a value by index.
            /// </summary>
            /// <param name="binder"> Provides information about the object that called the dynamic
            /// operation. </param>
            /// <param name="indexes"> The indexes that are used in the operation. </param>
            /// <param name="result"> The result of the index operation. </param>
            /// <returns> <c>true</c> if the operation is successful; otherwise, <c>false</c>. </returns>
            public override bool TryGetIndex(System.Dynamic.GetIndexBinder binder, object[] indexes, out object result)
            {
                if (indexes.Length != 1)
                    throw new NotImplementedException();
                result = this.obj[indexes[0].ToString()];
                if (TypeUtilities.IsUndefined(result))
                    result = Undefined.Value;
                return true;
            }

            /// <summary>
            /// Provides the implementation for operations that set a value by index.
            /// </summary>
            /// <param name="binder"> Provides information about the object that called the dynamic
            /// operation. </param>
            /// <param name="indexes"> The indexes that are used in the operation. </param>
            /// <param name="value"> The value to set to the object that has the specified index. </param>
            /// <returns> <c>true</c> if the operation is successful; otherwise, <c>false</c>. </returns>
            public override bool TrySetIndex(System.Dynamic.SetIndexBinder binder, object[] indexes, object value)
            {
                if (indexes.Length != 1)
                    throw new NotImplementedException();
                this.obj[indexes[0].ToString()] = value;
                return true;
            }

            /// <summary>
            /// Provides the implementation for operations that invoke a member.
            /// </summary>
            /// <param name="binder"> Provides information about the dynamic operation. </param>
            /// <param name="args"> The arguments that are passed to the object member during the
            /// invoke operation. </param>
            /// <param name="result"> The result of the member invocation. </param>
            /// <returns> <c>true</c> if the operation is successful; otherwise, <c>false</c>. </returns>
            public override bool TryInvokeMember(System.Dynamic.InvokeMemberBinder binder, object[] args, out object result)
            {
                return this.obj.TryCallMemberFunction(out result, binder.Name, args);
            }
        }

        [TestMethod]
        public void PropertyAccess()
        {
            var engine = new ScriptEngine();

            // Create a new object.
            dynamic obj = new DynamicMetaObjectWrapper((ObjectInstance)engine.Evaluate("({ one: 1, two: 2 })"));

            // Check property access works.
            Assert.AreEqual(1, obj.one);
            Assert.AreEqual(2, obj.two);

            // Check property setters work.
            obj.one = 5;
            Assert.AreEqual(5, obj.one);
            obj.three = 3;
            Assert.AreEqual(3, obj.three);

            // Undefined is returned if the property doesn't exist.
            Assert.AreEqual(Undefined.Value, obj.four);

            // Check indexer getters work.
            Assert.AreEqual(5, obj["one"]);
            Assert.AreEqual(2, obj["two"]);

            // Check indexer setters work.
            obj["one"] = 6;
            Assert.AreEqual(6, obj["one"]);
            obj["four"] = 7;
            Assert.AreEqual(7, obj["four"]);

            // Now check an array works
            dynamic array = new DynamicMetaObjectWrapper((ObjectInstance)engine.Evaluate("[9, 10]"));

            // Check indexer getters work.
            Assert.AreEqual(9, array[0]);
            Assert.AreEqual(10, array[1]);

            // Check indexer setters work.
            array[0] = 4;
            Assert.AreEqual(4, array[0]);
            array[2] = 5;
            Assert.AreEqual(5, array[2]);

            // Undefined is returned if the property doesn't exist.
            Assert.AreEqual(Undefined.Value, array[3]);
        }

        [TestMethod]
        public void MethodCall()
        {
            var engine = new ScriptEngine();

            // Create a new object.
            dynamic obj = new DynamicMetaObjectWrapper((ObjectInstance)engine.Evaluate("({ f: function(a) { return a + 5; } })"));

            // Check calling the method works.
            Assert.AreEqual(8, obj.f(3));
            Assert.AreEqual(9, obj.f(4, 5));
            Assert.AreEqual(double.NaN, obj.f());

            // Attempting to call an invalid function should throw an exception.
            TestUtils.ExpectException<Microsoft.CSharp.RuntimeBinder.RuntimeBinderException>(() => obj.g(3));
        }
    }
}
