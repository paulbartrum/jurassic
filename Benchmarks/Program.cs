using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Jurassic;

namespace MyBenchmarks
{
    public class TypeConverterBenchmarks
    {
        private readonly Random _random = new(42);

        [Benchmark]
        public int CheckForNaN() => TypeConverter.ToInteger(_random.Next());

        [Benchmark]
        public int CheckForNaN2() => TypeConverter.ToInteger2(_random.Next());
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<TypeConverterBenchmarks>();
        }
    }
}