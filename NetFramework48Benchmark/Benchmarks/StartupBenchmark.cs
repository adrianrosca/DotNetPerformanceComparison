using System;
using System.Diagnostics;

namespace NetFramework48Benchmark.Benchmarks
{
    public static class StartupBenchmark
    {
        public static void Run()
        {
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
    }
}