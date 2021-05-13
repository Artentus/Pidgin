using System;
using System.Diagnostics.CodeAnalysis;

namespace Pidgin
{
    public static partial class Parser
    {
        /// <summary>
        /// If <paramref name="parser"/> succeeds, <c>Lookahead(parser)</c> backtracks,
        /// behaving as if <paramref name="parser"/> had not consumed any input.
        /// No backtracking is performed upon failure.
        /// </summary>
        /// <param name="parser">The parser to look ahead with</param>
        /// <returns>A parser which rewinds the input stream if <paramref name="parser"/> succeeds.</returns>
        public static Parser<TToken, TUser, T> Lookahead<TToken, TUser, T>(Parser<TToken, TUser, T> parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException(nameof(parser));
            }
            return new LookaheadParser<TToken, TUser, T>(parser);
        }
    }

    internal sealed class LookaheadParser<TToken, TUser, T> : Parser<TToken, TUser, T>
    {
        private readonly Parser<TToken, TUser, T> _parser;

        public LookaheadParser(Parser<TToken, TUser, T> parser)
        {
            _parser = parser;
        }

        public sealed override bool TryParse(ref ParseState<TToken, TUser> state, ref PooledList<Expected<TToken>> expecteds, [MaybeNullWhen(false)] out T result)
        {
            state.PushBookmark();

            if (_parser.TryParse(ref state, ref expecteds, out result))
            {
                state.Rewind();
                return true;
            }
            
            state.PopBookmark();
            return false;
        }
    }
}