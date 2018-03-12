namespace EmitExperiment.Tests
{
    class A
    {
        public int Value { get; set; } = 1;
        public string Description => $"A (Value = {Value})";
    }

    class B
    {
        public B(int value)
        {
            Value = value;
        }

        public int Value { get; set; }
        public string Description => $"B (Value = {Value})";
    }

    class C
    {
        public C(A parentA, B parentB)
        {
            ParentA = parentA;
            ParentB = parentB;
        }

        public A ParentA { get; set; }
        public B ParentB { get; set; }
        public int Value { get; set; } = 3;

        public string Description => $"B (Value = {Value}, Parent A = {ParentA.Description}, Parent B = {ParentB.Description})";
    }
}
