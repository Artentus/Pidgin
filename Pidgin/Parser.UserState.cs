using System;
using System.Diagnostics.CodeAnalysis;

namespace Pidgin
{
    public static partial class Parser<TToken, TUser>
    {
        /// <summary>
        /// A parser which returns the current user state without consuming any input
        /// </summary>
        public static Parser<TToken, TUser, TUser> UserState { get; }
            = new UserStateParser<TToken, TUser>();
    }

    public partial class Parser<TToken, TUser, T>
    {
        /// <summary>
        /// Creates a parser equivalent to the current parser that also returns the current user state.
        /// </summary>
        /// <returns>A parser equivalent to the current parser that also returns the current user state</returns>
        public Parser<TToken, TUser, (T, TUser)> WithUserState()
            => new WithUserStateParser<TToken, TUser, T>(this);

        /// <summary>
        /// Creates a parser equivalent to the current parser that maps the user state to a new state.
        /// </summary>
        /// <returns>A parser equivalent to the current parser that maps the user state to a new state</returns>
        public Parser<TToken, TUser, T> MapUserState(Func<T, TUser, TUser> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            return new MapUserStateParser<TToken, TUser, T>(this, func);
        }
    }

    internal sealed class UserStateParser<TToken, TUser> : Parser<TToken, TUser, TUser>
    {
        public sealed override bool TryParse(ref ParseState<TToken, TUser> state, ref PooledList<Expected<TToken>> expecteds, [MaybeNullWhen(false)] out TUser result)
        {
            result = state.UserState;
            return true;
        }
    }

    internal sealed class WithUserStateParser<TToken, TUser, T> : Parser<TToken, TUser, (T, TUser)>
    {
        private readonly Parser<TToken, TUser, T> _parser;

        public WithUserStateParser(Parser<TToken, TUser, T> parser)
        {
            _parser = parser;
        }

        public sealed override bool TryParse(ref ParseState<TToken, TUser> state, ref PooledList<Expected<TToken>> expecteds, [MaybeNullWhen(false)] out (T, TUser) result)
        {
            if (!_parser.TryParse(ref state, ref expecteds, out var innerResult))
            {
                // state.Error set by _parser
                result = default;
                return false;
            }

            result = (innerResult, state.UserState);
            return true;
        }
    }

    internal sealed class MapUserStateParser<TToken, TUser, T> : Parser<TToken, TUser, T>
    {
        private readonly Parser<TToken, TUser, T> _parser;
        private readonly Func<T, TUser, TUser> _func;

        public MapUserStateParser(Parser<TToken, TUser, T> parser, Func<T, TUser, TUser> func)
        {
            _parser = parser;
            _func = func;
        }

        public sealed override bool TryParse(ref ParseState<TToken, TUser> state, ref PooledList<Expected<TToken>> expecteds, [MaybeNullWhen(false)] out T result)
        {
            if (!_parser.TryParse(ref state, ref expecteds, out result))
            {
                // state.Error set by _parser
                result = default;
                return false;
            }

            state.UserState = _func.Invoke(result, state.UserState);
            return true;
        }
    }
}
