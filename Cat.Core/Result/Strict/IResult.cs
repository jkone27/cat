namespace Cat.Core.Result.Strict
{
    /// <summary>
    /// Choice of (TOk|TError)
    /// </summary>
    /// <typeparam name="TOk"></typeparam>
    /// <typeparam name="TError"></typeparam>
    public interface IResult<out TOk, out TError>
    {
        bool IsSuccess { get; }

        TOk AsSuccess { get; }
        TError Error { get; }
    }
}