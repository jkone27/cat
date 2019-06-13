using System;
using System.Collections.Generic;
using System.Text;

namespace Cat.Core.Result.Static
{
    using System;

    public sealed class Result<TOk,TFailure>
    {
        private readonly (bool IsSuccess, TOk Result, TFailure Error) resultTuple;

        private Result((bool IsSuccess, TOk Result, TFailure Error) resultTuple)
        {
            this.resultTuple = resultTuple;
        }

        public static Result<TOk, TFailure> Success(TOk result)
        {
            return new Result<TOk, TFailure>((true, result, default(TFailure)));
        }

        public static Result<TOk, TFailure> Failure(TFailure error)
        {
            return new Result<TOk, TFailure>((false, default(TOk), error));
        }

        public bool IsSuccess => resultTuple.IsSuccess;

        public TFailure Error
        {
            get
            {
                if(resultTuple.IsSuccess)
                    throw new ArgumentException("result is success");

                return resultTuple.Error;
            }
        }

        public TOk Value
        {
            get
            {
                if(!resultTuple.IsSuccess)
                    throw new ArgumentException("result is failure");

                return resultTuple.Result;
            }
        }
        
        public Result<TOk2,TFailure> SelectMany<TOk2>(Func<TOk, Result<TOk2, TFailure>> next)
        {
            return IsSuccess ? next(Value) : Result<TOk2, TFailure>.Failure(Error);
        }

        public Result<TOk2,TFailure> Select<TOk2>(Func<TOk, TOk2> next, Func<Exception,TFailure> err)
        {
            try
            {
                return IsSuccess ? next(Value).AsSuccess<TOk2,TFailure>() : Result<TOk2, TFailure>.Failure(Error);
            }
            catch(Exception ex)
            {
                return err(ex).AsFailure<TOk2,TFailure>();
            }
        }
    }
}