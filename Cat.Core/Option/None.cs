namespace Cat.Core.Option
{
    public sealed class None<T> : IOption<T>
    {
        internal None()
        {

        }

        public bool IsSome => false;
    }
}