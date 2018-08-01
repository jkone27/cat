using System;

namespace Cat.Core.Option
{
    public static class OptionExtensions
    {
        private static Some<T> ToSome<T>(this T some)
        {
            return new Some<T>(some);
        }


        public static T UnwrapSome<T>(this IOption<T> some)
        {
            return ((Some<T>) some).Value;
        }


        public static IOption<T> ToOption<T>(this T reference)
        {
            return reference != null ? reference.ToSome() : new None<T>() as IOption<T>;
        }


        public static IOption<T> ToOption<T>(this T? nullable) where T : struct
        {
            return nullable.HasValue ? nullable.Value.ToSome() : new None<T>() as IOption<T>;
        }


        /// <summary>
        /// Bind
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TNext"></typeparam>
        /// <param name="option"></param>
        /// <param name="whenSome"></param>
        /// <returns></returns>
        public static IOption<TNext> Then<T, TNext>(this IOption<T> option, Func<T,TNext> whenSome)
        {
            return option.IsSome ? whenSome(option.UnwrapSome()).ToOption() : new None<TNext>();
        }

        /// <summary>
        /// Bind
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TNext"></typeparam>
        /// <param name="option"></param>
        /// <param name="whenSome"></param>
        /// <returns></returns>
        public static IOption<TNext> Then<T, TNext>(this IOption<T> option, Func<T,TNext?> whenSome) where TNext : struct
        {
            return option.IsSome ? whenSome(option.UnwrapSome()).ToOption() : new None<TNext>();
        }
    }
}