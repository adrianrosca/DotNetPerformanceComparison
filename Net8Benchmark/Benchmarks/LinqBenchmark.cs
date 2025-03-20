using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace Net8Benchmark.Benchmarks
{
    public static class LinqBenchmark
    {
        private const int Iterations = 1000;
        private const int ListSize = 100000;

        public static void Run()
        {
            var sw = Stopwatch.StartNew();
            var result = RunLinqOperations(Iterations);
            sw.Stop();

            Console.WriteLine($"LINQ operations: {sw.ElapsedMilliseconds}ms (Sum: {result})");
        }

        private static long RunLinqOperations(int iterations)
        {
            var numbers = Enumerable.Range(1, ListSize).ToList();
            long totalSum = 0;

            for (int i = 0; i < iterations; i++)
            {
                totalSum += numbers
                    .Where(n => n % 2 == 0)
                    .Select(n => n * n)
                    .Sum(n => (long)n);
            }

            return totalSum;
        }
    }
}