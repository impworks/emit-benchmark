using System;
using BenchmarkDotNet.Running;
using EmitExperiment.Tests;

namespace EmitExperiment
{
    public class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<MapperTest>();
            Console.ReadLine();
        }
    }
}
