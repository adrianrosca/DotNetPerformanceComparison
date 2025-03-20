//using System;
//using System.Diagnostics;
//using Prism.Ioc;
//using Prism.Container;
//using Net8Benchmark.Services;

//namespace Net8Benchmark.Benchmarks
//{
//    public static class DependencyInjectionBenchmark
//    {
//        private const int Iterations = 1000;

//        public static void Run()
//        {
//            var sw = Stopwatch.StartNew();
//            var result = RunDependencyInjection(Iterations);
//            sw.Stop();

//            Console.WriteLine($"Dependency injection: {sw.ElapsedMilliseconds}ms (Resolved: {result})");
//        }

//        private static int RunDependencyInjection(int iterations)
//        {
//            int resolvedCount = 0;

//            for (int i = 0; i < iterations; i++)
//            {
//                var containerRegistry = new UnityContainerExtension();

//                containerRegistry.RegisterInstance<ITestService>(new TestService());
//                containerRegistry.Register<IComplexService, ComplexService>();
//                containerRegistry.Register<IDataProcessor, DataProcessor>();

//                var service1 = containerRegistry.Resolve<ITestService>();
//                var service2 = containerRegistry.Resolve<IComplexService>();
//                var service3 = containerRegistry.Resolve<IDataProcessor>();

//                if (service1 != null && service2 != null && service3 != null)
//                {
//                    resolvedCount++;
//                }
//            }

//            return resolvedCount;
//        }
//    }
//}