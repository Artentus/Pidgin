namespace Pidgin
{
    public static partial class Parser<TToken, TUser>
    {
        /// <summary>
        /// Creates a parser which returns the specified value without consuming any input
        /// </summary>
        /// <param name="value">The value to return</param>
        /// <typeparam name="T">The type of the value to return</typeparam>
        /// <returns>A parser which returns the specified value without consuming any input</returns>
        public static Parser<TToken, TUser, T> Return<T>(T value)
            => new ReturnParser<TToken, TUser, T>(value);
    }

    internal sealed class ReturnParser<TToken, TUser, T> : Parser<TToken, TUser, T>
    {
        private readonly T _value;

        public ReturnParser(T value)
        {
            _value = value;
        }

        public sealed override bool TryParse(ref ParseState<TToken, TUser> state, ref PooledList<Expected<TToken>> expecteds, out T result)
        {
            result = _value;
            return true;
        }
    }
}
