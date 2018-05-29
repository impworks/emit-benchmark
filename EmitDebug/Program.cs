using System;
namespace EmitDebug
{
    class Program
    {
        static void Main(string[] args)
        {
            var emitter = new DebugEmitter();
            var met = emitter.CreateDebuggableMethod();
            var sets = new[]
            {
                new [] {1, 2, 4, 5, 3},
                new [] {4, 3},
            };

            foreach (var set in sets)
            {
                var max = met.Invoke(null, new object[] { set });
                Console.WriteLine(
                    "Max of {0}: {1}",
                    string.Join(", ", set),
                    max
                );
            }

            Console.ReadLine();
        }
    }
}
