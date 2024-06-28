// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using StarKindred.API.Benchmarks.Endpoints.Vassals;

BenchmarkRunner.Run<SearchBenchmark>();
