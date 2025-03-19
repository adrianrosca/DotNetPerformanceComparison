using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NetFramework48Benchmark
{
    class Program
    {
        private const int Iterations = 1000;
        private const int ListSize = 100000;
        private const int StringOperations = 1000000;

        static void Main(string[] args)
        {
            Console.WriteLine($"Running on: .NET Framework ({Environment.Version})");
            Console.WriteLine("Warming up...");

            // Warm up
            RunStringConcatenation(100);
            RunLinqOperations(100);
            RunListOperations(100);
            RunJsonSerialization(100);
            RunParallelProcessing(100);

            Console.WriteLine("Starting benchmarks...");
            Console.WriteLine("----------------------");

            // Run actual benchmarks
            BenchmarkStringConcatenation();
            BenchmarkLinqOperations();
            BenchmarkListOperations();
            BenchmarkJsonSerialization();
            BenchmarkParallelProcessing();
            BenchmarkStartupTime();

            Console.WriteLine("----------------------");
            Console.WriteLine("Benchmarks complete. Press any key to exit.");
            Console.ReadKey();
        }

        #region String Operations

        private static void BenchmarkStringConcatenation()
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

                // Reset periodically to avoid excessive memory usage
                if (i % 1000 == 0)
                {
                    result = "";
                }
            }
            return result;
        }

        #endregion

        #region LINQ Operations

        private static void BenchmarkLinqOperations()
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

        #endregion

        #region List Operations

        private static void BenchmarkListOperations()
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
                // Add items
                for (int j = 0; j < 1000; j++)
                {
                    list.Add(j);
                }

                // Sort
                list.Sort();

                // Search
                for (int j = 0; j < 100; j++)
                {
                    list.BinarySearch(j * 5);
                }

                // Remove
                for (int j = 0; j < 999; j++)
                {
                    list.RemoveAt(list.Count - 1);
                }
            }

            return list.Count;
        }

        #endregion

        #region JSON Serialization

        private static void BenchmarkJsonSerialization()
        {
            var sw = Stopwatch.StartNew();
            RunJsonSerialization(Iterations);
            sw.Stop();

            Console.WriteLine($"JSON serialization: {sw.ElapsedMilliseconds}ms");
        }

        private static void RunJsonSerialization(int iterations)
        {
            var testObject = new TestObject
            {
                Id = 12345,
                Name = "Performance Test",
                CreatedDate = DateTime.Now,
                Items = Enumerable.Range(1, 100).Select(i => new TestItem
                {
                    ItemId = i,
                    Value = $"Item {i}",
                    IsActive = i % 2 == 0
                }).ToList()
            };

            string json = "";

            for (int i = 0; i < iterations; i++)
            {
                // .NET Framework 4 - Use Newtonsoft.Json
                json = JsonConvert.SerializeObject(testObject);
                var deserialized = JsonConvert.DeserializeObject<TestObject>(json);

                // Prevent optimization from removing unused variable
                if (deserialized != null && deserialized.Id == -1)
                {
                    Console.WriteLine("This should never happen");
                }
            }
        }

        #endregion

        #region Parallel Processing

        private static void BenchmarkParallelProcessing()
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

                lock (typeof(Program))
                {
                    result += (long)array.Sum();
                }
            });

            return result;
        }

        #endregion

        #region Startup Time

        private static void BenchmarkStartupTime()
        {
            // Create a process to start itself and measure the time
            var processInfo = new ProcessStartInfo
            {
                FileName = Process.GetCurrentProcess().MainModule.FileName,
                Arguments = "measure-startup",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var sw = Stopwatch.StartNew();
            using (var process = Process.Start(processInfo))
            {
                process.WaitForExit();
            }
            sw.Stop();

            Console.WriteLine($"Process startup time: {sw.ElapsedMilliseconds}ms");
        }

        #endregion
    }

    public class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<TestItem> Items { get; set; }
    }

    public class TestItem
    {
        public int ItemId { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }
    }
}