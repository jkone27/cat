using System;

namespace Cat.Core.Result.Strict
{
    public struct Success<TOk,TError> : IResult<TOk,TError>
    {
        public TOk AsSuccess { get; }
        public bool IsSuccess => true;
        public TError Error => throw new ArgumentException("is success");

        internal Success(TOk asSuccess)
        {
            AsSuccess = asSuccess;
        }
    }
}