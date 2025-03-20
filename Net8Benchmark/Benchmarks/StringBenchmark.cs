using System;
using System.Diagnostics;

namespace Net8Benchmark.Benchmarks
{
    public static class StringBenchmark
    {
        private const int StringOperations = 1000000;

        public static void Run()
        {
            var sw = Stopwatch.StartNew();
            RunStringConcatenation(StringOperations);
            sw.Stop();

            Console.WriteLine($"String concatenation: {sw.ElapsedMilliseconds}ms");
        }

        private static string RunStringConcatenation(int iterations)
        {
            string result = "";
            for (int i = 0; i < iterations; i++)
            {
                result += i.ToString();

                if (i % 1000 == 0)
                {
                    result = "";
                }
            }
            return result;
        }
    }
}