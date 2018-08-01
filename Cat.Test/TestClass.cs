namespace Cat.Test
{
    public sealed class TestClass
    {
        public TestClass(string property)
        {
            Property = property;
        }

        public string Property { get; }
    }

    public sealed class AnotherTestClass
    {
        public int Property { get; }

        public AnotherTestClass(int property)
        {
            Property = property;
        }
    }
}