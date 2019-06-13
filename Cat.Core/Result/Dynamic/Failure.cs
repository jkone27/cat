using System;

namespace Cat.Core.Result.Dynamic
{
    public struct Failure<TOk, TError> : IResult<TOk,TError>
    {
        internal Failure(TError error)
        {
            Error = error;
        }

        public TOk AsSuccess => throw new ArgumentException("is failure");
        public TError Error { get; }
        public bool IsSuccess => false;
    }
}