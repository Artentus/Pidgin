using System;
using System.Diagnostics.CodeAnalysis;

namespace Pidgin
{
    public static partial class Parser<TUser>
    {
        /// <summary>
        /// Creates a parser which applies <paramref name="parser"/> and backtracks upon failure
        /// </summary>
        /// <typeparam name="TToken">The type of tokens in the parser's input stream</typeparam>
        /// <typeparam name="T">The return type of the parser</typeparam>
        /// <param name="parser">The parser</param>
        /// <returns>A parser which applies <paramref name="parser"/> and backtracks upon failure</returns>
        public static Parser<TToken, TUser, T> Try<TToken, T>(Parser<TToken, TUser, T> parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException(nameof(parser));
            }
            return new TryParser<TToken, TUser, T>(parser);
        }
    }

    public static partial class Parser
    {
        /// <summary>
        /// Creates a parser which applies <paramref name="parser"/> and backtracks upon failure
        /// </summary>
        /// <typeparam name="TToken">The type of tokens in the parser's input stream</typeparam>
        /// <typeparam name="T">The return type of the parser</typeparam>
        /// <param name="parser">The parser</param>
        /// <returns>A parser which applies <paramref name="parser"/> and backtracks upon failure</returns>
        public static Parser<TToken, Unit, T> Try<TToken, T>(Parser<TToken, Unit, T> parser)
            => Parser<Unit>.Try(parser);
    }

    internal sealed class TryParser<TToken, TUser, T> : Parser<TToken, TUser, T>
    {
        private readonly Parser<TToken, TUser, T> _parser;

        public TryParser(Parser<TToken, TUser, T> parser)
        {
            _parser = parser;
        }

        public sealed override bool TryParse(ref ParseState<TToken, TUser> state, ref PooledList<Expected<TToken>> expecteds, [MaybeNullWhen(false)] out T result)
        {
            // start buffering the input
            state.PushBookmark();
            if (!_parser.TryParse(ref state, ref expecteds, out result))
            {
                // return to the start of the buffer and discard the bookmark
                state.Rewind();
                return false;
            }

            // discard the buffer
            state.PopBookmark();
            return true;
        }
    }
}
