using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Net8Benchmark.Benchmarks
{
    public static class ParallelBenchmark
    {
        private const int Iterations = 1000;

        public static void Run()
        {
            var sw = Stopwatch.StartNew();
            var result = RunParallelProcessing(Iterations);
            sw.Stop();

            Console.WriteLine($"Parallel processing: {sw.ElapsedMilliseconds}ms (Result: {result})");
        }

        private static long RunParallelProcessing(int iterations)
        {
            long result = 0;

            Parallel.For(0, iterations, i =>
            {
                double[] array = new double[10000];
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = Math.Sqrt(j) * Math.Sin(j);
                }

                lock (typeof(ParallelBenchmark))
                {
                    result += (long)array.Sum();
                }
            });

            return result;
        }
    }
}