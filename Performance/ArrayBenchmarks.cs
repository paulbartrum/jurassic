using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Performance
{

    /// <summary>
    /// Benchmarks the global Array object.
    /// </summary>
    [TestClass]
    public class ArrayBenchmarks
    {
        //[TestMethod]
        //public void arrayConstructionViaLength()
        //{
        //    TestUtils.Benchmark(() =>
        //    {
        //        for (int i = 0; i < 1024 * 15; i++)
        //        {
        //            var array = Jurassic.Library.GlobalObject.Array.New();
        //            array["length"] = 1024;
        //        }
        //    });
        //}

        //[TestMethod]
        //public void arrayConstructionViaNew()
        //{
        //    TestUtils.Benchmark(() =>
        //    {
        //        Jurassic.Library.ArrayInstance array;
        //        for (int i = 0; i < 1024 * 10; i++)
        //            array = Jurassic.Library.GlobalObject.Array.New(1024);
        //    });
        //}

        [TestMethod]
        public void unshift()
        {
            TestUtils.Benchmark(() =>
            {
                // 2,080,000 inner loops/s
                var engine = new Jurassic.ScriptEngine();
                var array = engine.Array.New();
                for (int i = 0; i < 1000; i++)
                    Jurassic.Library.ArrayInstance.Unshift(array, i);
            });
        }

        [TestMethod]
        public void pop()
        {
            var engine = new Jurassic.ScriptEngine();
            var array = engine.Array.New();
            for (int i = 0; i < 1024 * 100; i++)
                Jurassic.Library.ArrayInstance.Push(array, i);
            TestUtils.Benchmark(() =>
            {
                for (int i = 0; i < 1024 * 100; i++)
                    array.Pop();
            });
        }

        [TestMethod]
        public void sum()
        {
            var engine = new Jurassic.ScriptEngine();
            var array = engine.Array.New();
            for (int i = 0; i < 1024 * 100; i++)
                Jurassic.Library.ArrayInstance.Push(array, i);
            TestUtils.Benchmark(() =>
            {
                int sum = 0;
                for (uint i = 0; i < 1024 * 100; i++)
                    sum += (int)array[i];
            });
        }

        [TestMethod]
        public void push()
        {
            var engine = new Jurassic.ScriptEngine();
            var array = engine.Array.New();
            TestUtils.Benchmark(() =>
            {
                for (int i = 0; i < 1024 * 100; i++)
                    Jurassic.Library.ArrayInstance.Push(array, i);
            });
        }

        //[TestMethod]
        //public void every()
        //{
        //    var array = Jurassic.Library.Global.Array.Construct();
        //    for (int i = 0; i < 1024 * 100; i++)
        //        array[i] = i;

        //    TestUtils.Benchmark(() =>
        //    {
        //        var callback = Jurassic.Library.Global.Function.Construct("test", new Jurassic.Library.ArrayConditionDelegate((thisObj, value, index, arr) => true));
        //        for (int i = 0; i < 10; i++)
        //        {
        //            Jurassic.Library.ArrayInstance.every(array, callback);
        //        }
        //    });
        //}
    }

}