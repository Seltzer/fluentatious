using System;

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
    }

}
