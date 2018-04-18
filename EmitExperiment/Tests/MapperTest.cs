using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using EmitExperiment.Mappers;

namespace EmitExperiment.Tests
{
    /// <summary>
    /// Fixture set for comparing mapper implementation.
    /// </summary>
    [CoreJob]
    public class MapperTest
    {
        /// <summary>
        /// Number of test repetitions.
        /// </summary>
        [Params(100, 1000, 10000)]
        public int Runs = 1;

        [Benchmark]
        public void Simple() => Test(new SimpleMapper());

        [Benchmark]
        public void SimpleCache() => Test(new SimpleCacheMapper());

        [Benchmark]
        public void Emitter() => Test(new EmitMapper());

        #region Testing

        /// <summary>
        /// Configures and tests the mapper.
        /// </summary>
        private void Test(IMapper mapper)
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

            mapper.Prepare<UserProfile, UserDTO>();

            var results = new List<UserDTO>(Runs);

            for(var i = 0; i < Runs; i++)
                results.Add(mapper.Map<UserProfile, UserDTO>(src));
        }

        #endregion
    }
}
