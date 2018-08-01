namespace Cat.Core.Result.Loose
{
    /// <summary>
    /// Loose Choice: not really Choice type.
    /// Implementors are not known at runtime, using dynamic polymorhpism (inheritance),
    /// but does not specify it's valid set of subtypes at compile time (no static-generics polymorhpism).
    /// </summary>
    public interface IResult
    {
        bool IsSuccess { get; }
    }
}