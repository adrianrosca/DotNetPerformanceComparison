using System;
using System.Diagnostics;
using NetFramework48Benchmark.Benchmarks;

namespace NetFramework48Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "measure-startup")
            {
                // Just exit for startup time measurement
                return;
            }

            Console.WriteLine($"Running on: .NET Framework ({Environment.Version})");
            Console.WriteLine("Warming up...");

            // Run actual benchmarks
            StringBenchmark.Run();
            LinqBenchmark.Run();
            ListBenchmark.Run();
            JsonBenchmark.Run();
            ParallelBenchmark.Run();
            DependencyInjectionBenchmark.Run();
            StartupBenchmark.Run();

            Console.WriteLine("----------------------");

            if (args.Length > 0 && args[0] == "skip-wait")
            {
                Console.WriteLine("Benchmarks complete.");
            }
            else
            {
                Console.WriteLine("Benchmarks complete. Press any key to exit.");
            }
        }
    }
}