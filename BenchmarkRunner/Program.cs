using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace BenchmarkRunner
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Get the solution directory
            string solutionDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."));

            // Define paths to the benchmark executables
            string net48Path = Path.Combine(solutionDir, "NetFramework48Benchmark", "bin", "Debug", "NetFramework48Benchmark.exe");
            string net8Path = Path.Combine(solutionDir, "Net8Benchmark", "bin", "Debug", "net8.0", "Net8Benchmark.exe");

            // Verify the executables exist
            if (!File.Exists(net48Path))
            {
                Console.WriteLine($"ERROR: .NET Framework 4.8 benchmark executable not found at: {net48Path}");
                Console.WriteLine("Please build the NetFramework48Benchmark project first.");
                Console.ReadKey();
                return;
            }

            if (!File.Exists(net8Path))
            {
                Console.WriteLine($"ERROR: .NET 8.0 benchmark executable not found at: {net8Path}");
                Console.WriteLine("Please build the Net8Benchmark project first.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Starting comparison benchmark of .NET Framework 4.8 vs .NET 8.0");
            Console.WriteLine("--------------------------------------------------------");

            // Create temporary log files
            string net48Log = Path.GetTempFileName();
            string net8Log = Path.GetTempFileName();

            try
            {
                // Run both benchmarks in parallel
                Console.WriteLine("Running benchmarks in parallel...");
                var net48Task = RunBenchmark(net48Path, ".NET Framework 4.8", net48Log);
                var net8Task = RunBenchmark(net8Path, ".NET 8.0", net8Log);

                await Task.WhenAll(net48Task, net8Task);

                // Compare and display results
                await CompareResults(net48Log, net8Log);
            }
            finally
            {
                // Clean up temp files
                try
                {
                    File.Delete(net48Log);
                    File.Delete(net8Log);
                }
                catch { /* Ignore cleanup errors */ }
            }

            Console.WriteLine("\nComparison complete. Press any key to exit.");
            Console.ReadKey();
        }

        static async Task RunBenchmark(string exePath, string frameworkName, string logFile)
        {
            try
            {
                Console.WriteLine($"Starting {frameworkName} benchmark...");

                var processInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = processInfo };
                process.Start();

                // Capture output to log file
                using var outputWriter = new StreamWriter(logFile);
                outputWriter.WriteLine($"--- {frameworkName} Benchmark Results ---");
                outputWriter.WriteLine($"Started at: {DateTime.Now}");
                outputWriter.WriteLine();

                while (!process.StandardOutput.EndOfStream)
                {
                    string line = await process.StandardOutput.ReadLineAsync();
                    outputWriter.WriteLine(line);
                    Console.WriteLine($"[{frameworkName}] {line}");
                }

                await Task.Run(() => process.WaitForExit());
                Console.WriteLine($"{frameworkName} benchmark completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running {frameworkName} benchmark: {ex.Message}");
                File.WriteAllText(logFile, $"ERROR: {ex.Message}");
            }
        }

        static async Task CompareResults(string net48LogPath, string net8LogPath)
        {
            Console.WriteLine("\nResults Comparison:");
            Console.WriteLine("--------------------------------------------------------");

            var net48Results = await ParseBenchmarkResults(net48LogPath);
            var net8Results = await ParseBenchmarkResults(net8LogPath);

            if (net48Results.Count == 0 || net8Results.Count == 0)
            {
                Console.WriteLine("Could not parse benchmark results. Check if benchmarks completed successfully.");
                return;
            }

            Console.WriteLine("| Benchmark             | .NET 4.8 (ms) | .NET 8.0 (ms) | Improvement |");
            Console.WriteLine("|----------------------|--------------|--------------|-------------|");

            double totalImprovement = 0;
            int benchmarkCount = 0;

            foreach (var benchmark in net48Results.Keys)
            {
                if (net8Results.TryGetValue(benchmark, out var net8Time))
                {
                    var net48Time = net48Results[benchmark];
                    double improvementPercent = 100 * (1 - ((double)net8Time / net48Time));
                    double speedupFactor = (double)net48Time / net8Time;

                    totalImprovement += improvementPercent;
                    benchmarkCount++;

                    Console.WriteLine($"| {benchmark,-20} | {net48Time,12} | {net8Time,12} | {improvementPercent,5:F1}% ({speedupFactor:F1}x) |");
                }
            }

            if (benchmarkCount > 0)
            {
                double avgImprovement = totalImprovement / benchmarkCount;
                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine($"Average improvement: {avgImprovement:F1}%");
            }
        }

        static async Task<System.Collections.Generic.Dictionary<string, long>> ParseBenchmarkResults(string logPath)
        {
            var results = new System.Collections.Generic.Dictionary<string, long>();

            string[] lines = await File.ReadAllLinesAsync(logPath);
            foreach (var line in lines)
            {
                // Parse benchmark results lines
                foreach (var prefix in new[] {
                    "String concatenation: ",
                    "LINQ operations: ",
                    "List operations: ",
                    "JSON serialization: ",
                    "Parallel processing: ",
                    "Process startup time: "
                })
                {
                    if (line.StartsWith(prefix))
                    {
                        var timeStr = line.Substring(prefix.Length);
                        int endIndex = timeStr.IndexOf("ms");
                        if (endIndex > 0)
                        {
                            timeStr = timeStr.Substring(0, endIndex);
                            if (long.TryParse(timeStr, out long time))
                            {
                                string benchmarkName = prefix.TrimEnd(": ".ToCharArray());
                                results[benchmarkName] = time;
                            }
                        }
                        break;
                    }
                }
            }

            return results;
        }
    }
}