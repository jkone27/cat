using System;
using Cat.Core.Result.Strict;

namespace Cat.Core.Result.Loose
{
    public static class LooseResultExtensions
    {

        public static Success<T> ToSuccess<T>(this T okData)
        {
            return new Success<T>(okData);
        }

        public static Failure<T> ToFailure<T>(this T errorData)
        {
            return new Failure<T>(errorData);
        }

        public static T Unwrap<T>(this IResult result)
        {
            return ((Success<T>) result).Data;
        }

        /// <summary>
        /// Bind
        /// </summary>
        /// <typeparam name="TOk"></typeparam>
        /// <typeparam name="TNext"></typeparam>
        /// <param name="result"></param>
        /// <param name="onNext"></param>
        /// <returns></returns>
        public static IResult Then<TOk, TNext>(
            this IResult result, 
            Func<TOk, TNext> onNext)
        {
            return result.IsSuccess ?
                onNext(result.Unwrap<TOk>()).ToSuccess() 
                : result as IResult;
        }

        /// <summary>
        /// Bind - lifts IResult
        /// </summary>
        /// <param name="result"></param>
        /// <param name="Func<TOk"></param>
        /// <param name="onNext"></param>
        /// <typeparam name="TOk"></typeparam>
        /// <returns></returns>
         public static IResult ThenLift<TOk>(
            this IResult result, 
            Func<TOk, IResult> onNext)
        {
            return result.IsSuccess ?
                onNext(result.Unwrap<TOk>())
                : result as IResult;
        }

        public static void Check<TOk, TError>(this IResult<TOk, TError> result, Action<TError> onError)
        {
            if (!result.IsSuccess)
                onError(result.Error);
        }
    }
    
}
