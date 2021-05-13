namespace Pidgin
{
    public static partial class Parser<TToken, TUser>
    {
        /// <summary>
        /// Creates a parser that parses any single character
        /// </summary>
        /// <returns>A parser that parses any single character</returns>
        public static Parser<TToken, TUser, TToken> Any { get; }
            = Token(_ => true).Labelled("any character");
    }
}