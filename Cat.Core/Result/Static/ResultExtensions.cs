using System;
using System.Collections.Generic;
using System.Text;

namespace Cat.Core.Result.Static
{
    public static class ResultExtensions
    {
        public static Result<TOk, TFailure> AsSuccess<TOk,TFailure>(this TOk result)
        {
            return Result<TOk, TFailure>.Success(result);
        }

        public static Result<TOk, TFailure> AsFailure<TOk,TFailure>(this TFailure error)
        {
            return Result<TOk, TFailure>.Failure(error);
        }
    }
}
