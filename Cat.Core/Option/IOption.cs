namespace Cat.Core.Option
{
    public interface IOption<T>
    {
        bool IsSome { get; }
    }
}