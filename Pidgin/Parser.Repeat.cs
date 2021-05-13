using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Pidgin
{
    public static partial class Parser
    {
        /// <summary>
        /// Creates a parser which applies <paramref name="parser"/> <paramref name="count"/> times,
        /// packing the resulting <c>char</c>s into a <c>string</c>.
        /// 
        /// <para>
        /// Equivalent to <c>parser.Repeat(count).Select(string.Concat)</c>.
        /// </para>
        /// </summary>
        /// <typeparam name="TToken">The type of tokens in the parser's input stream</typeparam>
        /// <typeparam name="TUser">The type of the user state</typeparam>
        /// <param name="parser">The parser</param>
        /// <param name="count">The number of times to apply the parser</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> was less than 0</exception>
        /// <returns>
        /// A parser which applies <paramref name="parser"/> <paramref name="count"/> times,
        /// packing the resulting <c>char</c>s into a <c>string</c>.
        /// </returns>
        public static Parser<TToken, TUser, string> RepeatString<TToken, TUser>(this Parser<TToken, TUser, char> parser, int count)
        {
            if (parser == null)
            {
                throw new ArgumentNullException(nameof(parser));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative");
            }
            return new RepeatStringParser<TToken, TUser>(parser, count);
        }
    }

    public abstract partial class Parser<TToken, TUser, T>
    {
        /// <summary>
        /// Creates a parser which applies the current parser <paramref name="count"/> times.
        /// </summary>
        /// <param name="count">The number of times to apply the current parser</param>
        /// <exception cref="System.InvalidOperationException"><paramref name="count"/> is less than 0</exception>
        /// <returns>A parser which applies the current parser <paramref name="count"/> times.</returns>
        public Parser<TToken, TUser, IEnumerable<T>> Repeat(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative");
            }
            return Parser<TToken, TUser>.Sequence(Enumerable.Repeat(this, count));
        }
    }

    internal sealed class RepeatStringParser<TToken, TUser> : Parser<TToken, TUser, string>
    {
        private readonly Parser<TToken, TUser, char> _parser;
        private readonly int _count;

        public RepeatStringParser(Parser<TToken, TUser, char> parser, int count)
        {
            _parser = parser;
            _count = count;
        }

        public sealed override bool TryParse(ref ParseState<TToken, TUser> state, ref PooledList<Expected<TToken>> expecteds, [MaybeNullWhen(false)] out string result)
        {
            var builder = new InplaceStringBuilder(_count);

            for (var _ = 0; _ < _count; _++)
            {
                var success = _parser.TryParse(ref state, ref expecteds, out var result1);

                if (!success)
                {
                    result = null;
                    return false;
                }

                builder.Append(result1);
            }

            result = builder.ToString();
            return true;
        }
    }
}