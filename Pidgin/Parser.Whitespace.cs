using System.Collections.Generic;

namespace Pidgin
{
    public static partial class Parser<TUser>
    {
        /// <summary>
        /// A parser that parses and returns a single whitespace character
        /// </summary>
        /// <returns>A parser that parses and returns a single whitespace character</returns>
        public static Parser<char, TUser, char> Whitespace { get; }
            = Parser<char, TUser>.Token(char.IsWhiteSpace).Labelled("whitespace");

        /// <summary>
        /// A parser that parses and returns a sequence of whitespace characters
        /// </summary>
        /// <returns>A parser that parses and returns a sequence of whitespace characters</returns>
        public static Parser<char, TUser, IEnumerable<char>> Whitespaces { get; }
            = Whitespace.Many().Labelled("whitespace");

        /// <summary>
        /// A parser that parses and returns a sequence of whitespace characters packed into a string
        /// </summary>
        /// <returns>A parser that parses and returns a sequence of whitespace characters packed into a string</returns>
        public static Parser<char, TUser, string> WhitespaceString { get; }
            = Whitespace.ManyString().Labelled("whitespace");

        /// <summary>
        /// A parser that discards a sequence of whitespace characters
        /// </summary>
        /// <returns>A parser that discards a sequence of whitespace characters</returns>
        public static Parser<char, TUser, Unit> SkipWhitespaces { get; }
            = new SkipWhitespacesParser<TUser>();
    }

    public static partial class Parser
    {
        /// <summary>
        /// A parser that parses and returns a single whitespace character
        /// </summary>
        /// <returns>A parser that parses and returns a single whitespace character</returns>
        public static Parser<char, Unit, char> Whitespace { get; } = Parser<Unit>.Whitespace;

        /// <summary>
        /// A parser that parses and returns a sequence of whitespace characters
        /// </summary>
        /// <returns>A parser that parses and returns a sequence of whitespace characters</returns>
        public static Parser<char, Unit, IEnumerable<char>> Whitespaces { get; } = Parser<Unit>.Whitespaces;

        /// <summary>
        /// A parser that parses and returns a sequence of whitespace characters packed into a string
        /// </summary>
        /// <returns>A parser that parses and returns a sequence of whitespace characters packed into a string</returns>
        public static Parser<char, Unit, string> WhitespaceString { get; } = Parser<Unit>.WhitespaceString;

        /// <summary>
        /// A parser that discards a sequence of whitespace characters
        /// </summary>
        /// <returns>A parser that discards a sequence of whitespace characters</returns>
        public static Parser<char, Unit, Unit> SkipWhitespaces { get; } = Parser<Unit>.SkipWhitespaces;
    }

    internal class SkipWhitespacesParser<TUser> : Parser<char, TUser, Unit>
    {
        public unsafe override bool TryParse(ref ParseState<char, TUser> state, ref PooledList<Expected<char>> expecteds, out Unit result)
        {
            const long space = (long)' ';
            const long fourSpaces = space | space << 16 | space << 32 | space << 48;
            result = Unit.Value;


            var chunk = state.LookAhead(32);
            while (chunk.Length == 32)
            {
                fixed (char* ptr = chunk)
                {
                    for (var i = 0; i < 8; i++)
                    {
                        if (*(long*)(ptr + i * 4) != fourSpaces)
                        {
                            // there's a non-' ' character somewhere in this group of four
                            for (var j = 0; j < 4; j++)
                            {
                                if (!char.IsWhiteSpace(chunk[i * 4 + j]))
                                {
                                    state.Advance(i * 4 + j);
                                    return true;
                                }
                            }
                        }
                    }
                }
                state.Advance(32);
                chunk = state.LookAhead(32);
            }

            var remainingGroupsOfFour = chunk.Length / 4;
            fixed (char* ptr = chunk)
            {
                for (var i = 0; i < remainingGroupsOfFour; i++)
                {
                    if (*(long*)(ptr + i * 4) != fourSpaces)
                    {
                        for (var j = 0; j < 4; j++)
                        {
                            if (!char.IsWhiteSpace(chunk[i * 4 + j]))
                            {
                                state.Advance(i * 4 + j);
                                return true;
                            }
                        }
                    }
                }
            }

            for (var i = remainingGroupsOfFour * 4; i < chunk.Length; i++)
            {
                if (!char.IsWhiteSpace(chunk[i]))
                {
                    state.Advance(i);
                    return true;
                }
            }

            state.Advance(chunk.Length);
            return true;
        }
    }
}