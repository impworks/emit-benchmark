using System;
using BenchmarkDotNet.Running;
using EmitExperiment.Mappers;
using EmitExperiment.Tests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EmitExperiment
{
    public class Program
    {
        static void Main()
        {
            TestMapper();

            Console.ReadLine();
        }

        /// <summary>
        /// Displays the work of a mapper.
        /// </summary>
        private static void TestMapper()
        {
            var src = new UserProfile
            {
                Id = 1,
                FirstName = "Василий",
                MiddleName = "Иванович",
                LastName = "Петров",
                Email = "petrov1337@example.com",
                Phone = "+7 (916) 123-45-67",
                IsSuspended = false,
                LastEdit = DateTime.Now,
            };

            Console.WriteLine("Source:\n{0}", JObject.FromObject(src).ToString(Formatting.Indented));

            var mapper = new EmitMapper();
            mapper.Prepare<UserProfile, UserDTO>();

            var dest = mapper.Map<UserProfile, UserDTO>(src);
            Console.WriteLine("\nResult:\n{0}", JObject.FromObject(dest).ToString(Formatting.Indented));
        }

        /// <summary>
        /// Displays the work of a DI container.
        /// </summary>
        private static void TestContainer()
        {

        }

        /// <summary>
        /// Runs benchmarks.
        /// </summary>
        private static void Benchmark()
        {
            // BenchmarkRunner.Run<MapperTest>();
            BenchmarkRunner.Run<ContainerTest>();
        }
    }
}
