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
                new object[] {1, 2},
                new object[] {4, 3},
            };

            foreach (var set in sets)
            {
                var max = met.Invoke(null, set);
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
