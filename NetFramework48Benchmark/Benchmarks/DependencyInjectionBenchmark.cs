using System;
using System.Diagnostics;
using Prism.Unity;
using NetFramework48Benchmark.Services;

namespace NetFramework48Benchmark.Benchmarks
{
    public static class DependencyInjectionBenchmark
    {
        private const int Iterations = 1000;

        public static void Run()
        {
            var sw = Stopwatch.StartNew();
            var result = RunDependencyInjection(Iterations);
            sw.Stop();

            Console.WriteLine($"Dependency injection: {sw.ElapsedMilliseconds}ms (Resolved: {result})");
        }

        private static int RunDependencyInjection(int iterations)
        {
            int resolvedCount = 0;

            for (int i = 0; i < iterations; i++)
            {
                var container = new UnityContainerExtension();

                container.RegisterInstance(typeof(ITestService), new TestService());
                container.Register(typeof(IComplexService), typeof(ComplexService));
                container.Register(typeof(IDataProcessor), typeof(DataProcessor));

                var service1 = container.Resolve(typeof(ITestService));
                var service2 = container.Resolve(typeof(IComplexService));
                var service3 = container.Resolve(typeof(IDataProcessor));

                if (service1 != null && service2 != null && service3 != null)
                {
                    resolvedCount++;
                }
            }

            return resolvedCount;
        }
    }
}