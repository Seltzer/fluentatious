using System;
using System.Linq;

namespace CSharp.Fluency.Extensions
{

    public static class ObjectExt
    {
        /// <summary>
        /// Extension method
        /// <para>
        /// If the specified predicate is satisfied, transform input using the specified transform function. Else, simply return input.
        /// </para>
        /// </summary>
        public static T If<T>(this T @this, Func<bool> predicate, Func<T, T> transformFunc)
        {
            return @this.If(_ => predicate(), transformFunc);
        }


        /// <summary>
        /// Extension method
        /// <para>
        /// Overload of <see cref="If{T}(T, Func{bool}, Func{T, T})"/> which takes a bool instead of a Func{bool}
        /// </para>
        /// </summary>
        public static T If<T>(this T @this, bool condition, Func<T, T> transformFunc)
        {
            return @this.If(() => condition, transformFunc);
        }


        /// <summary>
        /// Extension method
        /// <para>
        /// Same as <see cref="If{T}(T, Func{bool}, Func{T, T})"/> except that the predicate is a little more helpful
        /// </para>
        /// </summary>
        public static T If<T>(this T @this, Func<T, bool> predicate, Func<T, T> transformFunc)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");
            if (transformFunc == null)
                throw new ArgumentNullException("transformFunc");

            return predicate(@this) ? transformFunc(@this) : @this;
        }


        /// <summary>
        /// Extension method
        /// <para>
        /// Pretty simple. If @this is not null, apply <see cref="func"/> to it and return the result. Otherwise return null.
        /// </para>
        /// </summary>
        public static TReturn IfNotNull<T, TReturn>(this T @this, Func<T, TReturn> func)
            where T : class
            where TReturn : class
        {
            if (func == null)
                throw new ArgumentNullException("func");

            return @this != null ? func(@this) : null;
        }

        
        /// <summary>
        /// Extension method
        /// <para>
        /// Called CastTo to disambiguate from <see cref="Enumerable.Cast{T}"/>Enumerable.Cast
        /// </para>
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        public static T CastTo<T>(this object @this)
        {
            return (T) @this;
        }


        /// <summary>
        /// Extension method
        /// </summary>
        public static T As<T>(this object @this)
            where T : class
        {
            return @this as T;
        }


        /// <summary>
        /// Extension method
        /// <para>
        /// Similar to tap in Ruby / Underscore .JS.
        /// </para>
        /// </summary>
        public static T Do<T>(this T @this, params Action<T>[] actions)
        {
            if (actions == null || !actions.Any())
                throw new ArgumentOutOfRangeException("actions", "Must be non-empty");

            foreach (var action in actions)
                action(@this);

            return @this;
        }
    }

}
