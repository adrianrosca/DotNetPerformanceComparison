using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NetFramework48Benchmark.Benchmarks
{
    public static class ListBenchmark
    {
        private const int Iterations = 1000;

        public static void Run()
        {
            var sw = Stopwatch.StartNew();
            var result = RunListOperations(Iterations);
            sw.Stop();

            Console.WriteLine($"List operations: {sw.ElapsedMilliseconds}ms (Count: {result})");
        }

        private static int RunListOperations(int iterations)
        {
            var list = new List<int>();

            for (int i = 0; i < iterations; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    list.Add(j);
                }

                list.Sort();

                for (int j = 0; j < 100; j++)
                {
                    list.BinarySearch(j * 5);
                }

                for (int j = 0; j < 999; j++)
                {
                    list.RemoveAt(list.Count - 1);
                }
            }

            return list.Count;
        }
    }
}