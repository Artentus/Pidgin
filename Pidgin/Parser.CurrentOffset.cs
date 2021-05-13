namespace Pidgin
{
    public static partial class Parser<TToken, TUser>
    {
        /// <summary>
        /// A parser which returns the number of input tokens which have been consumed.
        /// </summary>
        /// <returns>A parser which returns the number of input tokens which have been consumed</returns>
        public static Parser<TToken, TUser, int> CurrentOffset { get; }
            = new CurrentOffsetParser<TToken, TUser>();
    }

    internal sealed class CurrentOffsetParser<TToken, TUser> : Parser<TToken, TUser, int>
    {
        public sealed override bool TryParse(ref ParseState<TToken, TUser> state, ref PooledList<Expected<TToken>> expecteds, out int result)
        {
            result = state.Location;
            return true;
        }
    }
}