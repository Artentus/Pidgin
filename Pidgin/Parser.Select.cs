using System;

namespace Pidgin
{
    public abstract partial class Parser<TToken, TUser, T>
    {
        /// <summary>
        /// Creates a parser which applies the specified transformation function to the result of the current parser.
        /// This is an infix synonym for <see cref="Parser{TUser}.Map{TToken, T1, R}(Func{T1, R}, Parser{TToken, TUser, T1})"/>.
        /// </summary>
        /// <typeparam name="U">The return type of the transformation function</typeparam>
        /// <param name="selector">A transformation function</param>
        /// <returns>A parser which applies <paramref name="selector"/> to the result of the current parser</returns>
        public Parser<TToken, TUser, U> Select<U>(Func<T, U> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            return Parser<TUser>.Map(selector, this);
        }

        /// <summary>
        /// Creates a parser which applies the specified transformation function to the result of the current parser.
        /// This is an infix synonym for <see cref="Parser{TUser}.Map{TToken, T1, R}(Func{T1, R}, Parser{TToken, TUser, T1})"/>.
        /// </summary>
        /// <typeparam name="U">The return type of the transformation function</typeparam>
        /// <param name="selector">A transformation function</param>
        /// <returns>A parser which applies <paramref name="selector"/> to the result of the current parser</returns>
        public Parser<TToken, TUser, U> Map<U>(Func<T, U> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            return Parser<TUser>.Map(selector, this);
        }
    }
}