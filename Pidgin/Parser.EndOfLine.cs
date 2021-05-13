namespace Pidgin
{
    public static partial class Parser<TUser>
    {
        /// <summary>
        /// A parser that parses and returns either the literal string "\r\n" or the literal string "\n"
        /// </summary>
        /// <returns>A parser that parses and returns either the literal string "\r\n" or the literal string "\n"</returns>
        public static Parser<char, TUser, string> EndOfLine { get; }
            = String("\r\n")
                .Or(String("\n"))
                .Labelled("end of line");
    }

    public static partial class Parser
    {
        /// <summary>
        /// A parser that parses and returns either the literal string "\r\n" or the literal string "\n"
        /// </summary>
        /// <returns>A parser that parses and returns either the literal string "\r\n" or the literal string "\n"</returns>
        public static Parser<char, Unit, string> EndOfLine { get; } = Parser<Unit>.EndOfLine;
    }
}