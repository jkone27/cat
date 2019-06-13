namespace Cat.Core.Option
{
    public sealed class Some<T> : IOption<T>
    {
        internal Some(T value)
        {
            Value = value;
        }

        public T Value { get; }

        public bool IsSome => true;
    }
}