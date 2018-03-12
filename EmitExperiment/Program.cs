using System;
using BenchmarkDotNet.Running;
using EmitExperiment.Tests;

namespace EmitExperiment
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<ContainerTest>();
            Console.ReadLine();
        }
    }
}
