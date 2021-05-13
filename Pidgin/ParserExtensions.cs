using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Pidgin.TokenStreams;
using Pidgin.Configuration;
using Config = Pidgin.Configuration.Configuration;

namespace Pidgin
{
    /// <summary>
    /// Extension methods for running parsers
    /// </summary>
    public static class ParserExtensions
    {
        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input string</param>
        /// <param name="userState">The initial user state</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <returns>The result of parsing</returns>
        public static Result<char, TUser, T> Parse<TUser, T>(this Parser<char, TUser, T> parser, string input, TUser userState, IConfiguration<char>? configuration = null)
            => Parse(parser, input.AsSpan(), userState, configuration);

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input list</param>
        /// <param name="userState">The initial user state</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <returns>The result of parsing</returns>
        public static Result<TToken, TUser, T> Parse<TToken, TUser, T>(this Parser<TToken, TUser, T> parser, IList<TToken> input, TUser userState, IConfiguration<TToken>? configuration = null)
            => DoParse(parser, new ListTokenStream<TToken>(input), userState, configuration);

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input list</param>
        /// <param name="userState">The initial user state</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <returns>The result of parsing</returns>
        public static Result<TToken, TUser, T> ParseReadOnlyList<TToken, TUser, T>(this Parser<TToken, TUser, T> parser, IReadOnlyList<TToken> input, TUser userState, IConfiguration<TToken>? configuration = null)
            => DoParse(parser, new ReadOnlyListTokenStream<TToken>(input), userState, configuration);

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input enumerable</param>
        /// <param name="userState">The initial user state</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <returns>The result of parsing</returns>
        public static Result<TToken, TUser, T> Parse<TToken, TUser, T>(this Parser<TToken, TUser, T> parser, IEnumerable<TToken> input, TUser userState, IConfiguration<TToken>? configuration = null)
        {
            using (var e = input.GetEnumerator())
            {
                return Parse(parser, e, userState, configuration);
            }
        }

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input enumerator</param>
        /// <param name="userState">The initial user state</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <returns>The result of parsing</returns>
        public static Result<TToken, TUser, T> Parse<TToken, TUser, T>(this Parser<TToken, TUser, T> parser, IEnumerator<TToken> input, TUser userState, IConfiguration<TToken>? configuration = null)
            => DoParse(parser, new EnumeratorTokenStream<TToken>(input), userState, configuration);

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>.
        /// Note that more characters may be consumed from <paramref name="input"/> than were required for parsing.
        /// You may need to manually rewind <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input stream</param>
        /// <param name="userState">The initial user state</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <returns>The result of parsing</returns>
        public static Result<byte, TUser, T> Parse<TUser, T>(this Parser<byte, TUser, T> parser, Stream input, TUser userState, IConfiguration<byte>? configuration = null)
            => DoParse(parser, new StreamTokenStream(input), userState, configuration);

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input reader</param>
        /// <param name="userState">The initial user state</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <returns>The result of parsing</returns>
        public static Result<char, TUser, T> Parse<TUser, T>(this Parser<char, TUser, T> parser, TextReader input, TUser userState, IConfiguration<char>? configuration = null)
            => DoParse(parser, new ReaderTokenStream(input), userState, configuration);

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input array</param>
        /// <param name="userState">The initial user state</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <returns>The result of parsing</returns>
        public static Result<TToken, TUser, T> Parse<TToken, TUser, T>(this Parser<TToken, TUser, T> parser, TToken[] input, TUser userState, IConfiguration<TToken>? configuration = null)
            => parser.Parse(input.AsSpan(), userState, configuration);
        
        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input span</param>
        /// <param name="userState">The initial user state</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <returns>The result of parsing</returns>
        public static Result<TToken, TUser, T> Parse<TToken, TUser, T>(this Parser<TToken, TUser, T> parser, ReadOnlySpan<TToken> input, TUser userState, IConfiguration<TToken>? configuration = null)
        {
            var state = new ParseState<TToken, TUser>(configuration ?? Config.Default<TToken>(), input, userState);
            var result = DoParse(parser, ref state);
            KeepAlive(ref input);
            return result;
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void KeepAlive<TToken>(ref ReadOnlySpan<TToken> span) {}

        private static Result<TToken, TUser, T> DoParse<TToken, TUser, T>(Parser<TToken, TUser, T> parser, ITokenStream<TToken> stream, TUser userState, IConfiguration<TToken>? configuration)
        {
            var state = new ParseState<TToken, TUser>(configuration ?? Config.Default<TToken>(), stream, userState);
            return DoParse(parser, ref state);
        }
        private static Result<TToken, TUser, T> DoParse<TToken, TUser, T>(Parser<TToken, TUser, T> parser, ref ParseState<TToken, TUser> state)
        {
            var startingLoc = state.Location;
            var expecteds = new PooledList<Expected<TToken>>(state.Configuration.ArrayPoolProvider.GetArrayPool<Expected<TToken>>());

            var result1 = parser.TryParse(ref state, ref expecteds, out var result)
                ? new Result<TToken, TUser, T>(state.Location > startingLoc, result, state.UserState)
                : new Result<TToken, TUser, T>(state.Location > startingLoc, state.BuildError(ref expecteds), state.UserState);

            expecteds.Dispose();
            state.Dispose();  // ensure we return the state's buffers to the buffer pool

            return result1;
        }


        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input string</param>
        /// <param name="userState">The initial user state</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <exception cref="ParseException">Thrown when an error occurs during parsing</exception>
        /// <returns>The result of parsing</returns>
        public static T ParseOrThrow<TUser, T>(this Parser<char, TUser, T> parser, string input, TUser userState, IConfiguration<char>? configuration = null)
            => GetValueOrThrow(parser.Parse(input, userState, configuration));

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input list</param>
        /// <param name="userState">The initial user state</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <exception cref="ParseException">Thrown when an error occurs during parsing</exception>
        /// <returns>The result of parsing</returns>
        public static T ParseOrThrow<TToken, TUser, T>(this Parser<TToken, TUser, T> parser, IList<TToken> input, TUser userState, IConfiguration<TToken>? configuration = null)
            => GetValueOrThrow(parser.Parse(input, userState, configuration));

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input list</param>
        /// <param name="userState">The initial user state</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <exception cref="ParseException">Thrown when an error occurs during parsing</exception>
        /// <returns>The result of parsing</returns>
        public static T ParseReadOnlyListOrThrow<TToken, TUser, T>(this Parser<TToken, TUser, T> parser, IReadOnlyList<TToken> input, TUser userState, IConfiguration<TToken>? configuration = null)
            => GetValueOrThrow(parser.ParseReadOnlyList(input, userState, configuration));

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input enumerable</param>
        /// <param name="userState">The initial user state</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <exception cref="ParseException">Thrown when an error occurs during parsing</exception>
        /// <returns>The result of parsing</returns>
        public static T ParseOrThrow<TToken, TUser, T>(this Parser<TToken, TUser, T> parser, IEnumerable<TToken> input, TUser userState, IConfiguration<TToken>? configuration = null)
            => GetValueOrThrow(parser.Parse(input, userState, configuration));

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input enumerator</param>
        /// <param name="userState">The initial user state</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <exception cref="ParseException">Thrown when an error occurs during parsing</exception>
        /// <returns>The result of parsing</returns>
        public static T ParseOrThrow<TToken, TUser, T>(this Parser<TToken, TUser, T> parser, IEnumerator<TToken> input, TUser userState, IConfiguration<TToken>? configuration = null)
            => GetValueOrThrow(parser.Parse(input, userState, configuration));

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input stream</param>
        /// <param name="userState">The initial user state</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <exception cref="ParseException">Thrown when an error occurs during parsing</exception>
        /// <returns>The result of parsing</returns>
        public static T ParseOrThrow<TUser, T>(this Parser<byte, TUser, T> parser, Stream input, TUser userState, IConfiguration<byte>? configuration = null)
            => GetValueOrThrow(parser.Parse(input, userState, configuration));

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input reader</param>
        /// <param name="userState">The initial user state</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <exception cref="ParseException">Thrown when an error occurs during parsing</exception>
        /// <returns>The result of parsing</returns>
        public static T ParseOrThrow<TUser, T>(this Parser<char, TUser, T> parser, TextReader input, TUser userState, IConfiguration<char>? configuration = null)
            => GetValueOrThrow(parser.Parse(input, userState, configuration));

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input array</param>
        /// <param name="userState">The initial user state</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <exception cref="ParseException">Thrown when an error occurs during parsing</exception>
        /// <returns>The result of parsing</returns>
        public static T ParseOrThrow<TToken, TUser, T>(this Parser<TToken, TUser, T> parser, TToken[] input, TUser userState, IConfiguration<TToken>? configuration = null)
            => GetValueOrThrow(parser.Parse(input, userState, configuration));

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input span</param>
        /// <param name="userState">The initial user state</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <exception cref="ParseException">Thrown when an error occurs during parsing</exception>
        /// <returns>The result of parsing</returns>
        public static T ParseOrThrow<TToken, TUser, T>(this Parser<TToken, TUser, T> parser, ReadOnlySpan<TToken> input, TUser userState, IConfiguration<TToken>? configuration = null)
            => GetValueOrThrow(parser.Parse(input, userState, configuration));

        private static T GetValueOrThrow<TToken, TUser, T>(Result<TToken, TUser, T> result)
            => result.Success ? result.Value : throw new ParseException(result.Error!.RenderErrorMessage());






        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input string</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <returns>The result of parsing</returns>
        public static Result<char, Unit, T> Parse<T>(this Parser<char, Unit, T> parser, string input, IConfiguration<char>? configuration = null)
            => Parse(parser, input.AsSpan(), configuration);

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input list</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <returns>The result of parsing</returns>
        public static Result<TToken, Unit, T> Parse<TToken, T>(this Parser<TToken, Unit, T> parser, IList<TToken> input, IConfiguration<TToken>? configuration = null)
            => DoParse(parser, new ListTokenStream<TToken>(input), configuration);

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input list</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <returns>The result of parsing</returns>
        public static Result<TToken, Unit, T> ParseReadOnlyList<TToken, T>(this Parser<TToken, Unit, T> parser, IReadOnlyList<TToken> input, IConfiguration<TToken>? configuration = null)
            => DoParse(parser, new ReadOnlyListTokenStream<TToken>(input), configuration);

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input enumerable</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <returns>The result of parsing</returns>
        public static Result<TToken, Unit, T> Parse<TToken, T>(this Parser<TToken, Unit, T> parser, IEnumerable<TToken> input, IConfiguration<TToken>? configuration = null)
        {
            using (var e = input.GetEnumerator())
            {
                return Parse(parser, e, configuration);
            }
        }

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input enumerator</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <returns>The result of parsing</returns>
        public static Result<TToken, Unit, T> Parse<TToken, T>(this Parser<TToken, Unit, T> parser, IEnumerator<TToken> input, IConfiguration<TToken>? configuration = null)
            => DoParse(parser, new EnumeratorTokenStream<TToken>(input), configuration);

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>.
        /// Note that more characters may be consumed from <paramref name="input"/> than were required for parsing.
        /// You may need to manually rewind <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input stream</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <returns>The result of parsing</returns>
        public static Result<byte, Unit, T> Parse<T>(this Parser<byte, Unit, T> parser, Stream input, IConfiguration<byte>? configuration = null)
            => DoParse(parser, new StreamTokenStream(input), configuration);

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input reader</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <returns>The result of parsing</returns>
        public static Result<char, Unit, T> Parse<T>(this Parser<char, Unit, T> parser, TextReader input, IConfiguration<char>? configuration = null)
            => DoParse(parser, new ReaderTokenStream(input), configuration);

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input array</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <returns>The result of parsing</returns>
        public static Result<TToken, Unit, T> Parse<TToken, T>(this Parser<TToken, Unit, T> parser, TToken[] input, IConfiguration<TToken>? configuration = null)
            => parser.Parse(input.AsSpan(), configuration);

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input span</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <returns>The result of parsing</returns>
        public static Result<TToken, Unit, T> Parse<TToken, T>(this Parser<TToken, Unit, T> parser, ReadOnlySpan<TToken> input, IConfiguration<TToken>? configuration = null)
        {
            var state = new ParseState<TToken, Unit>(configuration ?? Config.Default<TToken>(), input, Unit.Value);
            var result = DoParse(parser, ref state);
            KeepAlive(ref input);
            return result;
        }

        private static Result<TToken, Unit, T> DoParse<TToken, T>(Parser<TToken, Unit, T> parser, ITokenStream<TToken> stream, IConfiguration<TToken>? configuration)
        {
            var state = new ParseState<TToken, Unit>(configuration ?? Config.Default<TToken>(), stream, Unit.Value);
            return DoParse(parser, ref state);
        }
        private static Result<TToken, Unit, T> DoParse<TToken, T>(Parser<TToken, Unit, T> parser, ref ParseState<TToken, Unit> state)
        {
            var startingLoc = state.Location;
            var expecteds = new PooledList<Expected<TToken>>(state.Configuration.ArrayPoolProvider.GetArrayPool<Expected<TToken>>());

            var result1 = parser.TryParse(ref state, ref expecteds, out var result)
                ? new Result<TToken, Unit, T>(state.Location > startingLoc, result, Unit.Value)
                : new Result<TToken, Unit, T>(state.Location > startingLoc, state.BuildError(ref expecteds), Unit.Value);

            expecteds.Dispose();
            state.Dispose();  // ensure we return the state's buffers to the buffer pool

            return result1;
        }


        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input string</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <exception cref="ParseException">Thrown when an error occurs during parsing</exception>
        /// <returns>The result of parsing</returns>
        public static T ParseOrThrow<T>(this Parser<char, Unit, T> parser, string input, IConfiguration<char>? configuration = null)
            => GetValueOrThrow(parser.Parse(input, configuration));

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input list</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <exception cref="ParseException">Thrown when an error occurs during parsing</exception>
        /// <returns>The result of parsing</returns>
        public static T ParseOrThrow<TToken, T>(this Parser<TToken, Unit, T> parser, IList<TToken> input, IConfiguration<TToken>? configuration = null)
            => GetValueOrThrow(parser.Parse(input, configuration));

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input list</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <exception cref="ParseException">Thrown when an error occurs during parsing</exception>
        /// <returns>The result of parsing</returns>
        public static T ParseReadOnlyListOrThrow<TToken, T>(this Parser<TToken, Unit, T> parser, IReadOnlyList<TToken> input, IConfiguration<TToken>? configuration = null)
            => GetValueOrThrow(parser.ParseReadOnlyList(input, configuration));

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input enumerable</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <exception cref="ParseException">Thrown when an error occurs during parsing</exception>
        /// <returns>The result of parsing</returns>
        public static T ParseOrThrow<TToken, T>(this Parser<TToken, Unit, T> parser, IEnumerable<TToken> input, IConfiguration<TToken>? configuration = null)
            => GetValueOrThrow(parser.Parse(input, configuration));

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input enumerator</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <exception cref="ParseException">Thrown when an error occurs during parsing</exception>
        /// <returns>The result of parsing</returns>
        public static T ParseOrThrow<TToken, T>(this Parser<TToken, Unit, T> parser, IEnumerator<TToken> input, IConfiguration<TToken>? configuration = null)
            => GetValueOrThrow(parser.Parse(input, configuration));

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input stream</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <exception cref="ParseException">Thrown when an error occurs during parsing</exception>
        /// <returns>The result of parsing</returns>
        public static T ParseOrThrow<T>(this Parser<byte, Unit, T> parser, Stream input, IConfiguration<byte>? configuration = null)
            => GetValueOrThrow(parser.Parse(input, configuration));

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input reader</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <exception cref="ParseException">Thrown when an error occurs during parsing</exception>
        /// <returns>The result of parsing</returns>
        public static T ParseOrThrow<T>(this Parser<char, Unit, T> parser, TextReader input, IConfiguration<char>? configuration = null)
            => GetValueOrThrow(parser.Parse(input, configuration));

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input array</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <exception cref="ParseException">Thrown when an error occurs during parsing</exception>
        /// <returns>The result of parsing</returns>
        public static T ParseOrThrow<TToken, T>(this Parser<TToken, Unit, T> parser, TToken[] input, IConfiguration<TToken>? configuration = null)
            => GetValueOrThrow(parser.Parse(input, configuration));

        /// <summary>
        /// Applies <paramref name="parser"/> to <paramref name="input"/>
        /// </summary>
        /// <param name="parser">A parser</param>
        /// <param name="input">An input span</param>
        /// <param name="configuration">The configuration, or null to use the default configuration</param>
        /// <exception cref="ParseException">Thrown when an error occurs during parsing</exception>
        /// <returns>The result of parsing</returns>
        public static T ParseOrThrow<TToken, T>(this Parser<TToken, Unit, T> parser, ReadOnlySpan<TToken> input, IConfiguration<TToken>? configuration = null)
            => GetValueOrThrow(parser.Parse(input, configuration));
    }
}
