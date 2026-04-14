using BenchmarkDotNet.Running;
using GMenu.Benchmarks;

#if RELEASE
BenchmarkRunner.Run<ReadDesktopFilesHeaderBenchmarkWithMoreThan200Files>();
#else 
Console.WriteLine("Cannot run benchmarks in DEBUG mode");
#endif

Console.ReadKey();
