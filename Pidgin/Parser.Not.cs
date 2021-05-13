using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Pidgin
{
    public static partial class Parser<TUser>
    {
        /// <summary>
        /// Creates a parser which succeeds only if the given parser fails.
        /// The resulting parser does not perform any backtracking; it consumes the same amount of input as the supplied parser.
        /// Combine this function with <see cref="Parser{TUser}.Try{TToken, T}(Parser{TToken, TUser, T})"/> if this behaviour is undesirable.
        /// </summary>
        /// <param name="parser">The parser that is expected to fail</param>
        /// <returns>A parser which succeeds only if the given parser fails.</returns>
        public static Parser<TToken, TUser, Unit> Not<TToken, T>(Parser<TToken, TUser, T> parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException(nameof(parser));
            }
            return new NegatedParser<TToken, TUser, T>(parser);            
        }
    }

    public static partial class Parser
    {
        /// <summary>
        /// Creates a parser which succeeds only if the given parser fails.
        /// The resulting parser does not perform any backtracking; it consumes the same amount of input as the supplied parser.
        /// Combine this function with <see cref="Parser{TUser}.Try{TToken, T}(Parser{TToken, TUser, T})"/> if this behaviour is undesirable.
        /// </summary>
        /// <param name="parser">The parser that is expected to fail</param>
        /// <returns>A parser which succeeds only if the given parser fails.</returns>
        public static Parser<TToken, Unit, Unit> Not<TToken, T>(Parser<TToken, Unit, T> parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException(nameof(parser));
            }
            return new NegatedParser<TToken, Unit, T>(parser);
        }
    }

    internal sealed class NegatedParser<TToken, TUser, T> : Parser<TToken, TUser, Unit>
    {
        private readonly Parser<TToken, TUser, T> _parser;

        public NegatedParser(Parser<TToken, TUser, T> parser)
        {
            _parser = parser;
        }

        public sealed override bool TryParse(ref ParseState<TToken, TUser> state, ref PooledList<Expected<TToken>> expecteds, [MaybeNullWhen(false)] out Unit result)
        {
            var startingLocation = state.Location;
            var token = state.HasCurrent ? Maybe.Just(state.Current) : Maybe.Nothing<TToken>();

            state.PushBookmark();  // make sure we don't throw out the buffer, we may need it to compute a SourcePos
            var childExpecteds = new PooledList<Expected<TToken>>(state.Configuration.ArrayPoolProvider.GetArrayPool<Expected<TToken>>());

            var success = _parser.TryParse(ref state, ref childExpecteds, out var result1);

            childExpecteds.Dispose();
            state.PopBookmark();
            
            if (success)
            {
                state.SetError(token, false, startingLocation, null);
                result = default;
                return false;
            }

            result = Unit.Value;
            return true;
        }
    }
}