namespace Cat.Core.Result.Loose
{
    public sealed class Failure<T> : IResult
    {
        public Failure(T error)
        {
            Error = error;
        }

        public bool IsSuccess => false;

        public T Error { get; }
    }
}