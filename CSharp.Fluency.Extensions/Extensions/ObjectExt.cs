using System;
using System.Linq;

namespace CSharp.Fluency.Extensions.Extensions
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
        /// Same as <see cref="If{T}(T, bool, Func{T, T})"/> except that the condition is inverted.
        /// </para>
        /// </summary>
        public static T Unless<T>(this T @this, bool condition, Func<T, T> transformFunc)
        {
            return @this.If(() => !condition, transformFunc);
        }


        /// <summary>
        /// Extension method
        /// <para>
        /// Same as <see cref="If{T}(T, Func{bool}, Func{T, T})"/> except that the predicate is inverted.
        /// </para>
        /// </summary>
        public static T Unless<T>(this T @this, Func<bool> predicate, Func<T, T> transformFunc)
        {
            return @this.If(() => !predicate(), transformFunc);
        }


        /// <summary>
        /// Extension method
        /// <para>
        /// Same as <see cref="If{T}(T, Func{T, bool}, Func{T, T})"/> except that the predicate is inverted.
        /// </para>
        /// </summary>
        public static T Unless<T>(this T @this, Func<T, bool> predicate, Func<T, T> transformFunc)
        {
            return @this.If(t => !predicate(t), transformFunc);
        }


        #region IfNotNull methods

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
        /// Variation of <see cref="IfNotNull{T, TReturn}(T, Func{T, TReturn})"/> which takes two transforms.
        /// </para>
        /// </summary>
        public static TReturn IfNotNull<T, TIntermediate, TReturn>(this T @this, Func<T, TIntermediate> func1, Func<TIntermediate, TReturn> func2)
            where T : class
            where TIntermediate : class
            where TReturn : class
        {
            return @this.IfNotNull(func1).IfNotNull(func2);
        }
        

        /// <summary>
        /// Extension method
        /// <para>
        /// Variation of <see cref="IfNotNull{T, TReturn}(T, Func{T, TReturn})"/> which takes three transforms.
        /// </para>
        /// </summary>
        public static TReturn IfNotNull<T, TIntermediate1, TIntermediate2, TReturn>(this T @this, 
            Func<T, TIntermediate1> func1, Func<TIntermediate1, TIntermediate2> func2, Func<TIntermediate2, TReturn> func3)
            where T : class
            where TIntermediate1 : class
            where TIntermediate2 : class
            where TReturn : class
        {
            return @this.IfNotNull(func1, func2).IfNotNull(func3);
        }


        /// <summary>
        /// Extension method
        /// <para>
        /// Variation of <see cref="IfNotNull{T, TReturn}(T, Func{T, TReturn})"/> which takes four transforms.
        /// </para>
        /// </summary>
        public static TReturn IfNotNull<T, TIntermediate1, TIntermediate2, TIntermediate3, TReturn>(this T @this, 
                Func<T, TIntermediate1> func1, Func<TIntermediate1, TIntermediate2> func2, Func<TIntermediate2, TIntermediate3> func3, 
                Func<TIntermediate3, TReturn> func4)
            where T : class
            where TIntermediate1 : class
            where TIntermediate2 : class
            where TIntermediate3 : class
            where TReturn : class
        {
            return @this.IfNotNull(func1, func2, func3).IfNotNull(func4);
        }


        /// <summary>
        /// Extension method
        /// <para>
        /// Variation of <see cref="IfNotNull{T, TReturn}(T, Func{T, TReturn})"/> which takes five transforms.
        /// </para>
        /// </summary>
        public static TReturn IfNotNull<T, TIntermediate1, TIntermediate2, TIntermediate3, TIntermediate4, TReturn>(this T @this, 
                Func<T, TIntermediate1> func1, Func<TIntermediate1, TIntermediate2> func2, Func<TIntermediate2, TIntermediate3> func3, 
                Func<TIntermediate3, TIntermediate4> func4, Func<TIntermediate4, TReturn> func5)
            where T : class
            where TIntermediate1 : class
            where TIntermediate2 : class
            where TIntermediate3 : class
            where TIntermediate4 : class
            where TReturn : class
        {
            return @this.IfNotNull(func1, func2, func3, func4).IfNotNull(func5);
        }

        #endregion


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
        /// Similar to tap in Ruby / Underscore .JS. Takes any number of actions, performs them sequentially on this, and then returns this.
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


        /// <summary>
        /// Extension method
        /// <para>
        /// Alias of <see cref="Do{T}"/>
        /// </para>
        /// </summary>
        public static T Tap<T>(this T @this, params Action<T>[] actions)
        {
            return @this.Do(actions);
        }


        #region Pipe methods

        /// <summary>
        /// Extension method
        /// <para>
        /// Takes @this and pipes it into func as an argument. Like a non-variadic version of JavaScript apply with @this as the argument.
        /// </para>
        /// </summary>
        public static TReturn Pipe<T, TReturn>(this T @this, Func<T, TReturn> func)
        {
            return func(@this);
        }


        /// <summary>
        /// Extension method
        /// <para>
        /// Variation of <see cref="Pipe{T, TReturn}(T, Func{T, TReturn})"/> which takes two transforms.
        /// </para>
        /// </summary>
        public static TReturn Pipe<T, TIntermediate1, TReturn>(this T @this, Func<T, TIntermediate1> transform1, 
            Func<TIntermediate1, TReturn> transform2)
        {
            return @this.Pipe(transform1).Pipe(transform2);
        }


        /// <summary>
        /// Extension method
        /// <para>
        /// Variation of <see cref="Pipe{T, TReturn}(T, Func{T, TReturn})"/> which takes three transforms.
        /// </para>
        /// </summary>
        public static TReturn Pipe<T, TIntermediate1, TIntermediate2, TReturn>(
            this T @this, 
            Func<T, TIntermediate1> transform1, 
            Func<TIntermediate1, TIntermediate2> transform2, 
            Func<TIntermediate2, TReturn> transform3)
        {
            return @this.Pipe(transform1, transform2).Pipe(transform3);
        }


        /// <summary>
        /// Extension method
        /// <para>
        /// Variation of <see cref="Pipe{T, TReturn}(T, Func{T, TReturn})"/> which takes four transforms.
        /// </para>
        /// </summary>
        public static TReturn Pipe<T, TIntermediate1, TIntermediate2, TIntermediate3, TReturn>(
            this T @this, 
            Func<T, TIntermediate1> transform1, 
            Func<TIntermediate1, TIntermediate2> transform2, 
            Func<TIntermediate2, TIntermediate3> transform3, 
            Func<TIntermediate3, TReturn> transform4)
        {
            return @this.Pipe(transform1, transform2, transform3).Pipe(transform4);
        }

        
        /// <summary>
        /// Extension method
        /// <para>
        /// Variation of <see cref="Pipe{T, TReturn}(T, Func{T, TReturn})"/> which takes four transforms
        /// </para>
        /// </summary>
        public static TReturn Pipe<T, TIntermediate1, TIntermediate2, TIntermediate3, TIntermediate4, TReturn>(
            this T @this, 
            Func<T, TIntermediate1> transform1, 
            Func<TIntermediate1, TIntermediate2> transform2, 
            Func<TIntermediate2, TIntermediate3> transform3, 
            Func<TIntermediate3, TIntermediate4> transform4,
            Func<TIntermediate4, TReturn> transform5)
        {
            return @this.Pipe(transform1, transform2, transform3, transform4).Pipe(transform5);
        }

        #endregion
    }

}
