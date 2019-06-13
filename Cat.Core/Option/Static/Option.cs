using System;
using System.Collections.Generic;
using System.Text;

namespace Cat.Core.Option.Static
{
    public sealed class Option<T>
    {
        private readonly T value;
        private Option(T value)
        {
            this.value = value;
        }

        internal static Option<T> AsOption(T value)
        {
            return new Option<T>(value);
        }

        public bool IsSome => value != null;

        public static Option<T> None() => AsOption(default(T));

        public Option<U> SelectMany<U>(Func<T, Option<U>> func) 
        {
            return IsSome ? func(value) : new Option<U>(default(U));
        }

        public Option<U> Select<U>(Func<T, U> func)
        {
            return IsSome? new Option<U>(func(value)) : new Option<U>(default(U));
        }

        public T Unwrap() => IsSome ? value : throw new ArgumentException("none");
    }

    public static class OptionExtensions
    {
        public static Option<T> AsOption<T>(this T value) where T : class
        {
            return Option<T>.AsOption(value);
        }

        public static Option<T> AsOption<T>(this T? value) where T : struct
        {
            return value.HasValue ? Option<T>.AsOption(value.Value) :  Option<T>.AsOption(default(T));
        }
    }
}
