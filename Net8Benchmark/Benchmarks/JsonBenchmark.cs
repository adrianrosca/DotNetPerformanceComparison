using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Net8Benchmark.Models;

namespace Net8Benchmark.Benchmarks
{
    public static class JsonBenchmark
    {
        private const int Iterations = 1000;

        public static void Run()
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
                json = JsonSerializer.Serialize(testObject);
                var deserialized = JsonSerializer.Deserialize<TestObject>(json);

                if (deserialized != null && deserialized.Id == -1)
                {
                    Console.WriteLine("This should never happen");
                }
            }
        }
    }
}