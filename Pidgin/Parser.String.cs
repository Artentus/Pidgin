using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Pidgin
{
    public static partial class Parser<TUser>
    {
        /// <summary>
        /// Creates a parser that parses and returns a literal string
        /// </summary>
        /// <param name="str">The string to parse</param>
        /// <returns>A parser that parses and returns a literal string</returns>
        /// 
        public static Parser<char, TUser, string> String(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }
            return Parser<char, TUser>.Sequence(str);
        }
        
        /// <summary>
        /// Creates a parser that parses and returns a literal string, in a case insensitive manner.
        /// The parser returns the actual string parsed.
        /// </summary>
        /// <param name="str">The string to parse</param>
        /// <returns>A parser that parses and returns a literal string, in a case insensitive manner.</returns>
        public static Parser<char, TUser, string> CIString(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }
            return new CIStringParser<TUser>(str);
        }
    }

    public static partial class Parser
    {
        /// <summary>
        /// Creates a parser that parses and returns a literal string
        /// </summary>
        /// <param name="str">The string to parse</param>
        /// <returns>A parser that parses and returns a literal string</returns>
        /// 
        public static Parser<char, Unit, string> String(string str)
            => Parser<Unit>.String(str);

        /// <summary>
        /// Creates a parser that parses and returns a literal string, in a case insensitive manner.
        /// The parser returns the actual string parsed.
        /// </summary>
        /// <param name="str">The string to parse</param>
        /// <returns>A parser that parses and returns a literal string, in a case insensitive manner.</returns>
        public static Parser<char, Unit, string> CIString(string str)
            => Parser<Unit>.CIString(str);
    }

    internal sealed class CIStringParser<TUser> : Parser<char, TUser, string>
    {
        private readonly string _value;
        private Expected<char> _expected;
        private Expected<char> Expected
        {
            get
            {
                if (_expected.Tokens.IsDefault)
                {
                    _expected = new Expected<char>(_value.ToImmutableArray());
                }
                return _expected;
            }
        }

        public CIStringParser(string value)
        {
            _value = value;
        }

        public sealed override bool TryParse(ref ParseState<char, TUser> state, ref PooledList<Expected<char>> expecteds, [MaybeNullWhen(false)] out string result)
        {
            var span = state.LookAhead(_value.Length);  // span.Length <= _valueTokens.Length

            var errorPos = -1;
            for (var i = 0; i < span.Length; i++)
            {
                if (!char.ToLowerInvariant(span[i]).Equals(char.ToLowerInvariant(_value[i])))
                {
                    errorPos = i;
                    break;
                }
            }

            if (errorPos != -1)
            {
                // strings didn't match
                state.Advance(errorPos);
                state.SetError(Maybe.Just(span[errorPos]), false, state.Location, null);
                expecteds.Add(Expected);
                result = null;
                return false;
            }

            if (span.Length < _value.Length)
            {
                // strings matched but reached EOF
                state.Advance(span.Length);
                state.SetError(Maybe.Nothing<char>(), true, state.Location, null);
                expecteds.Add(Expected);
                result = null;
                return false;
            }

            // OK
            state.Advance(_value.Length);
            result = span.ToString();
            return true;
        }
    }
}