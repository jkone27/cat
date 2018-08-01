namespace Cat.Core.Result.Loose
{
    public sealed class Success<T> : IResult
    {
        public Success(T data)
        {
            Data = data;
        }

        public bool IsSuccess => true;

        public T Data { get; }
    }
}