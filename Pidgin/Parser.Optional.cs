namespace Pidgin
{
    public abstract partial class Parser<TToken, TUser, T>
    {
        private static Parser<TToken, TUser, Maybe<T>>? _returnNothing;
        private static Parser<TToken, TUser, Maybe<T>> ReturnNothing 
        {
            get
            {
                if (_returnNothing == null)
                {
                    _returnNothing = Parser<TToken, TUser>.Return(Maybe.Nothing<T>());
                }
                return _returnNothing;
            }
        }
        /// <summary>
        /// Creates a parser which applies the current parser and returns <see cref="Maybe.Nothing{T}()"/> if the current parser fails without consuming any input.
        /// The resulting parser fails if the current parser fails after consuming input.
        /// </summary>
        /// <returns>A parser which applies the current parser and returns <see cref="Maybe.Nothing{T}()"/> if the current parser fails without consuming any input</returns>
        public Parser<TToken, TUser, Maybe<T>> Optional()
            => this.Select(Maybe.Just).Or(ReturnNothing);
    }
}