using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Pidgin
{
    public static partial class Parser<TUser>
    {
        /// <summary>
        /// Creates a parser which parses and returns one of the specified characters.
        /// </summary>
        /// <param name="chars">A sequence of characters to choose between</param>
        /// <returns>A parser which parses and returns one of the specified characters</returns>
        public static Parser<char, TUser, char> OneOf(params char[] chars)
        {
            if (chars == null)
            {
                throw new ArgumentNullException(nameof(chars));
            }
            return OneOf(chars.AsEnumerable());
        }

        /// <summary>
        /// Creates a parser which parses and returns one of the specified characters.
        /// </summary>
        /// <param name="chars">A sequence of characters to choose between</param>
        /// <returns>A parser which parses and returns one of the specified characters</returns>
        public static Parser<char, TUser, char> OneOf(IEnumerable<char> chars)
        {
            if (chars == null)
            {
                throw new ArgumentNullException(nameof(chars));
            }
            var cs = chars.ToArray();
            return Parser<char, TUser>
                .Token(c => Array.IndexOf(cs, c) != -1)
                .WithExpected(cs.Select(c => new Expected<char>(ImmutableArray.Create(c))).ToImmutableArray());
        }

        /// <summary>
        /// Creates a parser which parses and returns one of the specified characters, in a case insensitive manner.
        /// The parser returns the actual character parsed.
        /// </summary>
        /// <param name="chars">A sequence of characters to choose between</param>
        /// <returns>A parser which parses and returns one of the specified characters, in a case insensitive manner.</returns>
        public static Parser<char, TUser, char> CIOneOf(params char[] chars)
        {
            if (chars == null)
            {
                throw new ArgumentNullException(nameof(chars));
            }
            return CIOneOf(chars.AsEnumerable());
        }

        /// <summary>
        /// Creates a parser which parses and returns one of the specified characters, in a case insensitive manner.
        /// The parser returns the actual character parsed.
        /// </summary>
        /// <param name="chars">A sequence of characters to choose between</param>
        /// <returns>A parser which parses and returns one of the specified characters, in a case insensitive manner.</returns>
        public static Parser<char, TUser, char> CIOneOf(IEnumerable<char> chars)
        {
            if (chars == null)
            {
                throw new ArgumentNullException(nameof(chars));
            }
            var cs = chars.Select(char.ToLowerInvariant).ToArray();
            var builder = ImmutableArray.CreateBuilder<Expected<char>>(cs.Length * 2);
            foreach (var c in cs)
            {
                builder.Add(new Expected<char>(ImmutableArray.Create(char.ToLowerInvariant(c))));
                builder.Add(new Expected<char>(ImmutableArray.Create(char.ToUpperInvariant(c))));
            }
            return Parser<char, TUser>
                .Token(c => Array.IndexOf(cs, char.ToLowerInvariant(c)) != -1)
                .WithExpected(builder.MoveToImmutable());
        }

        /// <summary>
        /// Creates a parser which applies one of the specified parsers.
        /// The resulting parser fails if all of the input parsers fail without consuming input, or if one of them fails after consuming input
        /// </summary>
        /// <typeparam name="TToken">The type of tokens in the parsers' input stream</typeparam>
        /// <typeparam name="T">The return type of the parsers</typeparam>
        /// <param name="parsers">A sequence of parsers to choose between</param>
        /// <returns>A parser which applies one of the specified parsers</returns>
        public static Parser<TToken, TUser, T> OneOf<TToken, T>(params Parser<TToken, TUser, T>[] parsers)
        {
            if (parsers == null)
            {
                throw new ArgumentNullException(nameof(parsers));
            }
            return OneOf(parsers.AsEnumerable());
        }

        /// <summary>
        /// Creates a parser which applies one of the specified parsers.
        /// The resulting parser fails if all of the input parsers fail without consuming input, or if one of them fails after consuming input.
        /// The input enumerable is enumerated and copied to a list.
        /// </summary>
        /// <typeparam name="TToken">The type of tokens in the parsers' input stream</typeparam>
        /// <typeparam name="T">The return type of the parsers</typeparam>
        /// <param name="parsers">A sequence of parsers to choose between</param>
        /// <returns>A parser which applies one of the specified parsers</returns>
        public static Parser<TToken, TUser, T> OneOf<TToken, T>(IEnumerable<Parser<TToken, TUser, T>> parsers)
        {
            if (parsers == null)
            {
                throw new ArgumentNullException(nameof(parsers));
            }
            return OneOfParser<TToken, TUser, T>.Create(parsers);
        }
    }

    public static partial class Parser
    {
        /// <summary>
        /// Creates a parser which parses and returns one of the specified characters.
        /// </summary>
        /// <param name="chars">A sequence of characters to choose between</param>
        /// <returns>A parser which parses and returns one of the specified characters</returns>
        public static Parser<char, Unit, char> OneOf(params char[] chars)
            => Parser<Unit>.OneOf(chars);

        /// <summary>
        /// Creates a parser which parses and returns one of the specified characters.
        /// </summary>
        /// <param name="chars">A sequence of characters to choose between</param>
        /// <returns>A parser which parses and returns one of the specified characters</returns>
        public static Parser<char, Unit, char> OneOf(IEnumerable<char> chars)
            => Parser<Unit>.OneOf(chars);

        /// <summary>
        /// Creates a parser which parses and returns one of the specified characters, in a case insensitive manner.
        /// The parser returns the actual character parsed.
        /// </summary>
        /// <param name="chars">A sequence of characters to choose between</param>
        /// <returns>A parser which parses and returns one of the specified characters, in a case insensitive manner.</returns>
        public static Parser<char, Unit, char> CIOneOf(params char[] chars)
            => Parser<Unit>.CIOneOf(chars);

        /// <summary>
        /// Creates a parser which parses and returns one of the specified characters, in a case insensitive manner.
        /// The parser returns the actual character parsed.
        /// </summary>
        /// <param name="chars">A sequence of characters to choose between</param>
        /// <returns>A parser which parses and returns one of the specified characters, in a case insensitive manner.</returns>
        public static Parser<char, Unit, char> CIOneOf(IEnumerable<char> chars)
            => Parser<Unit>.CIOneOf(chars);

        /// <summary>
        /// Creates a parser which applies one of the specified parsers.
        /// The resulting parser fails if all of the input parsers fail without consuming input, or if one of them fails after consuming input
        /// </summary>
        /// <typeparam name="TToken">The type of tokens in the parsers' input stream</typeparam>
        /// <typeparam name="T">The return type of the parsers</typeparam>
        /// <param name="parsers">A sequence of parsers to choose between</param>
        /// <returns>A parser which applies one of the specified parsers</returns>
        public static Parser<TToken, Unit, T> OneOf<TToken, T>(params Parser<TToken, Unit, T>[] parsers)
            => Parser<Unit>.OneOf(parsers);

        /// <summary>
        /// Creates a parser which applies one of the specified parsers.
        /// The resulting parser fails if all of the input parsers fail without consuming input, or if one of them fails after consuming input.
        /// The input enumerable is enumerated and copied to a list.
        /// </summary>
        /// <typeparam name="TToken">The type of tokens in the parsers' input stream</typeparam>
        /// <typeparam name="T">The return type of the parsers</typeparam>
        /// <param name="parsers">A sequence of parsers to choose between</param>
        /// <returns>A parser which applies one of the specified parsers</returns>
        public static Parser<TToken, Unit, T> OneOf<TToken, T>(IEnumerable<Parser<TToken, Unit, T>> parsers)
            => Parser<Unit>.OneOf(parsers);
    }

    internal sealed class OneOfParser<TToken, TUser, T> : Parser<TToken, TUser, T>
    {
        private readonly Parser<TToken, TUser, T>[] _parsers;

        private OneOfParser(Parser<TToken, TUser, T>[] parsers)
        {
            _parsers = parsers;
        }

        // see comment about expecteds in ParseState.Error.cs
        public sealed override bool TryParse(ref ParseState<TToken, TUser> state, ref PooledList<Expected<TToken>> expecteds, [MaybeNullWhen(false)] out T result)
        {
            var firstTime = true;
            var err = new InternalError<TToken>(
                Maybe.Nothing<TToken>(),
                false,
                state.Location,
                "OneOf had no arguments"
            );

            var childExpecteds = new PooledList<Expected<TToken>>(state.Configuration.ArrayPoolProvider.GetArrayPool<Expected<TToken>>());  // the expecteds for all loop iterations
            var grandchildExpecteds = new PooledList<Expected<TToken>>(state.Configuration.ArrayPoolProvider.GetArrayPool<Expected<TToken>>());  // the expecteds for the current loop iteration
            foreach (var p in _parsers)
            {
                var thisStartLoc = state.Location;
                
                if (p.TryParse(ref state, ref grandchildExpecteds, out result))
                {
                    // throw out all expecteds
                    grandchildExpecteds.Dispose();
                    childExpecteds.Dispose();
                    return true;
                }

                // we'll usually return the error from the first parser that didn't backtrack,
                // even if other parsers had a longer match.
                // There is some room for improvement here.
                if (state.Location > thisStartLoc)
                {
                    // throw out all expecteds except this one
                    expecteds.AddRange(grandchildExpecteds.AsSpan());
                    childExpecteds.Dispose();
                    grandchildExpecteds.Dispose();
                    result = default;
                    return false;
                }

                childExpecteds.AddRange(grandchildExpecteds.AsSpan());
                grandchildExpecteds.Clear();
                // choose the longest match, preferring the left-most error in a tie,
                // except the first time (avoid returning "OneOf had no arguments").
                if (firstTime || state.ErrorLocation > err.ErrorLocation)
                {
                    err = state.GetError();
                }
                firstTime = false;
            }
            state.SetError(err);
            expecteds.AddRange(childExpecteds.AsSpan());
            childExpecteds.Dispose();
            grandchildExpecteds.Dispose();
            result = default;
            return false;
        }

        internal static OneOfParser<TToken, TUser, T> Create(IEnumerable<Parser<TToken, TUser, T>> parsers)
        {
            // if we know the length of the collection,
            // we know we're going to need at least that much room in the list
            var list = parsers is ICollection<Parser<TToken, TUser, T>> coll
                ? new List<Parser<TToken, TUser, T>>(coll.Count)
                : new List<Parser<TToken, TUser, T>>();
            
            foreach (var p in parsers)
            {
                if (p == null)
                {
                    throw new ArgumentNullException(nameof(parsers));
                }
                if (p is OneOfParser<TToken, TUser, T> o)
                {
                    list.AddRange(o._parsers);
                }
                else
                {
                    list.Add(p);
                }
            }
            return new OneOfParser<TToken, TUser, T>(list.ToArray());
        }
    }
}