namespace Pidgin
{
    public static partial class Parser<TToken, TUser>
    {
        /// <summary>
        /// A parser which returns the current source position
        /// </summary>
        /// <returns>A parser which returns the current source position</returns>
        public static Parser<TToken, TUser, SourcePosDelta> CurrentSourcePosDelta { get; }
            = new CurrentPosParser<TToken, TUser>();

        /// <summary>
        /// A parser which returns the current source position
        /// </summary>
        /// <returns>A parser which returns the current source position</returns>
        public static Parser<TToken, TUser, SourcePos> CurrentPos { get; }
            = CurrentSourcePosDelta.Select(d => new SourcePos(1, 1) + d);
    }

    internal sealed class CurrentPosParser<TToken, TUser> : Parser<TToken, TUser, SourcePosDelta>
    {
        public sealed override bool TryParse(ref ParseState<TToken, TUser> state, ref PooledList<Expected<TToken>> expecteds, out SourcePosDelta result)
        {
            result = state.ComputeSourcePosDelta();
            return true;
        }
    }
}