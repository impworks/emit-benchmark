using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using EmitExperiment.Containers;

namespace EmitExperiment.Tests
{
    /// <summary>
    /// Fixture set for comparing container implementation.
    /// </summary>
    [CoreJob]
    public class ContainerTest
    {
        /// <summary>
        /// Number of test repetitions.
        /// </summary>
        [Params(10000)]
        public int Runs = 1;

        [Benchmark]
        public void Simple() => Test(new SimpleContainer());
        
        [Benchmark]
        public void SimpleCache() => Test(new SimpleCacheContainer());
        
        [Benchmark]
        public void Emitter() => Test(new EmitContainer());

        [Benchmark]
        public void EmitterNonGeneric() => Test(new EmitNonGenericContainer());

        [Benchmark]
        public void Expression() => Test(new ExpressionContainer());

        [Benchmark]
        public void Activator() => Test(new ActivatorContainer());

        #region Testing

        /// <summary>
        /// Configures and tests the container.
        /// </summary>
        private void Test(IContainer container)
        {
            container.Register<A>()
                     .Register<B>(() => new B(2))
                     .Register<C>();

            container.Prepare();

            var results = new List<C>(Runs);

            for(var i = 0; i < Runs; i++)
                results.Add(container.Get<C>());
        }

        #endregion
    }
}
