using System;

namespace Cat.Core.Result.Strict
{
    public static class StrictResultExtensions
    {
        public static Success<TOk, TError> ToSuccess<TOk, TError>(this TOk okData)
        {
            return new Success<TOk, TError>(okData);
        }

        public static Failure<TOk, TError> ToFailure<TOk, TError>(this TError errorData)
        {
            return new Failure<TOk, TError>(errorData);
        }

        /// <summary>
        /// Bind
        /// </summary>
        /// <typeparam name="TOk"></typeparam>
        /// <typeparam name="TNext"></typeparam>
        /// <typeparam name="TError"></typeparam>
        /// <param name="result"></param>
        /// <param name="onNext"></param>
        /// <param name="onError"></param>
        /// <returns></returns>
        public static IResult<TNext, TError> Then<TOk, TNext, TError>(
            this IResult<TOk, TError> result, 
            Func<TOk, TNext> onNext, 
            Func<TError, TError> onError)
        {
            return result.IsSuccess ?
                onNext(result.AsSuccess).ToSuccess<TNext,TError>() 
                : onError(result.Error).ToFailure<TNext, TError>() 
                    as IResult<TNext,TError>;
        }

        /// <summary>
        /// Bind
        /// </summary>
        /// <typeparam name="TOk"></typeparam>
        /// <typeparam name="TNext"></typeparam>
        /// <typeparam name="TError"></typeparam>
        /// <param name="result"></param>
        /// <param name="onNext"></param>
        /// <returns></returns>
        public static IResult<TNext, TError> Then<TOk, TNext, TError>(
            this IResult<TOk, TError> result, 
            Func<TOk, TNext> onNext)
        {
            return result.IsSuccess ?
                onNext(result.AsSuccess).ToSuccess<TNext, TError>() : 
                    result.Error.ToFailure<TNext, TError>() as IResult<TNext,TError>;
        }

        public static void Check<TOk, TError>(this IResult<TOk, TError> result, Action<TError> onError)
        {
            if (!result.IsSuccess)
                onError(result.Error);
        }
    }
}
