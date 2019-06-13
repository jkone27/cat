using System;

namespace Cat.Core.Result.Dynamic
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
        /// Select
        /// </summary>
        /// <typeparam name="TOk"></typeparam>
        /// <typeparam name="TNext"></typeparam>
        /// <typeparam name="TError"></typeparam>
        /// <param name="result"></param>
        /// <param name="onNext"></param>
        /// <param name="onError"></param>
        /// <returns></returns>
        public static IResult<TNext, TError> Select<TOk, TNext, TError>(
            this IResult<TOk, TError> result,
            Func<TOk, TNext> onNext,
            Func<Exception, IResult<TNext, TError>> onError)
        {
            try
            {
                return result.IsSuccess ?
               onNext(result.AsSuccess).ToSuccess<TNext, TError>()
               : result as IResult<TNext, TError>;
            }
            catch (Exception ex)
            {
                return onError(ex);
            }
        }

        /// <summary>
        /// SelectMany
        /// </summary>
        /// <typeparam name="TOk"></typeparam>
        /// <typeparam name="TNext"></typeparam>
        /// <typeparam name="TError"></typeparam>
        /// <param name="result"></param>
        /// <param name="onNext"></param>
        /// <param name="onError"></param>
        /// <returns></returns>
        public static IResult<TNext, TError> SelectMany<TOk, TNext, TError>(
            this IResult<TOk, TError> result,
            Func<TOk, IResult<TNext, TError>> onNext)
        {
            return result.IsSuccess ?
                onNext(result.AsSuccess)
                : result.Error as IResult<TNext, TError>;
        }
    }
}
