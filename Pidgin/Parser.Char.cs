using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Pidgin
{
    public static partial class Parser<TUser>
    {
        /// <summary>
        /// Creates a parser which parses and returns a single character.
        /// </summary>
        /// <param name="character">The character to parse</param>
        /// <returns>A parser which parses and returns a single character</returns>
        public static Parser<char, TUser, char> Char(char character) => Parser<char, TUser>.Token(character);

        /// <summary>
        /// Creates a parser which parses and returns a single character, in a case insensitive manner.
        /// The parser returns the actual character parsed.
        /// </summary>
        /// <param name="character">The character to parse</param>
        /// <returns>A parser which parses and returns a single character</returns>
        public static Parser<char, TUser, char> CIChar(char character)
        {
            var theChar = char.ToLowerInvariant(character);
            var expected = ImmutableArray.Create(
                new Expected<char>(ImmutableArray.Create(char.ToUpperInvariant(character))),
                new Expected<char>(ImmutableArray.Create(char.ToLowerInvariant(character)))
            );
            return Parser<char, TUser>.Token(c => char.ToLowerInvariant(c) == theChar)
                .WithExpected(expected);
        }

        /// <summary>
        /// Creates a parser which parses and returns a character if it is not one of the specified characters.
        /// When the character is one of the given characters, the parser fails without consuming input.
        /// </summary>
        /// <param name="chars">A sequence of characters that should not be matched</param>
        /// <returns>A parser which parses and returns a character that does not match one of the specified characters</returns>
        public static Parser<char, TUser, char> AnyCharExcept(params char[] chars)
        {
            if (chars == null)
            {
                throw new ArgumentNullException(nameof(chars));
            }
            return AnyCharExcept(chars.AsEnumerable());
        }

        /// <summary>
        /// Creates a parser which parses and returns a character if it is not one of the specified characters.
        /// When the character is one of the given characters, the parser fails without consuming input.
        /// </summary>
        /// <param name="chars">A sequence of characters that should not be matched</param>
        /// <returns>A parser which parses and returns a character that does not match one of the specified characters</returns>
        public static Parser<char, TUser, char> AnyCharExcept(IEnumerable<char> chars)
        {
            if (chars == null)
            {
                throw new ArgumentNullException(nameof(chars));
            }
            var cs = chars.ToArray();
            return Parser<char, TUser>.Token(c => Array.IndexOf(cs, c) == -1);
        }

        /// <summary>
        /// A parser that parses and returns a single digit character (0-9)
        /// </summary>
        /// <returns>A parser that parses and returns a single digit character</returns>
        public static Parser<char, TUser, char> Digit { get; } = Parser<char, TUser>.Token(c => char.IsDigit(c)).Labelled("digit");

        /// <summary>
        /// A parser that parses and returns a single letter character
        /// </summary>
        /// <returns>A parser that parses and returns a single letter character</returns>
        public static Parser<char, TUser, char> Letter { get; } = Parser<char, TUser>.Token(c => char.IsLetter(c)).Labelled("letter");

        /// <summary>
        /// A parser that parses and returns a single letter or digit character
        /// </summary>
        /// <returns>A parser that parses and returns a single letter or digit character</returns>
        public static Parser<char, TUser, char> LetterOrDigit { get; } = Parser<char, TUser>.Token(c => char.IsLetterOrDigit(c)).Labelled("letter or digit");

        /// <summary>
        /// A parser that parses and returns a single lowercase letter character
        /// </summary>
        /// <returns>A parser that parses and returns a single lowercase letter character</returns>
        public static Parser<char, TUser, char> Lowercase { get; } = Parser<char, TUser>.Token(c => char.IsLower(c)).Labelled("lowercase letter");

        /// <summary>
        /// A parser that parses and returns a single uppercase letter character
        /// </summary>
        /// <returns>A parser that parses and returns a single uppercase letter character</returns>
        public static Parser<char, TUser, char> Uppercase { get; } = Parser<char, TUser>.Token(c => char.IsUpper(c)).Labelled("uppercase letter");

        /// <summary>
        /// A parser that parses and returns a single Unicode punctuation character
        /// </summary>
        /// <returns>A parser that parses and returns a single Unicode punctuation character</returns>
        public static Parser<char, TUser, char> Punctuation { get; } = Parser<char, TUser>.Token(c => char.IsPunctuation(c)).Labelled("punctuation");

        /// <summary>
        /// A parser that parses and returns a single Unicode symbol character
        /// </summary>
        /// <returns>A parser that parses and returns a single Unicode symbol character</returns>
        public static Parser<char, TUser, char> Symbol { get; } = Parser<char, TUser>.Token(c => char.IsSymbol(c)).Labelled("symbol");

        /// <summary>
        /// A parser that parses and returns a single Unicode separator character
        /// </summary>
        /// <returns>A parser that parses and returns a single Unicode separator character</returns>
        public static Parser<char, TUser, char> Separator { get; } = Parser<char, TUser>.Token(c => char.IsSeparator(c)).Labelled("separator");
    }



    public static partial class Parser
    {
        /// <summary>
        /// Creates a parser which parses and returns a single character.
        /// </summary>
        /// <param name="character">The character to parse</param>
        /// <returns>A parser which parses and returns a single character</returns>
        public static Parser<char, Unit, char> Char(char character) => Parser<Unit>.Char(character);

        /// <summary>
        /// Creates a parser which parses and returns a single character, in a case insensitive manner.
        /// The parser returns the actual character parsed.
        /// </summary>
        /// <param name="character">The character to parse</param>
        /// <returns>A parser which parses and returns a single character</returns>
        public static Parser<char, Unit, char> CIChar(char character) => Parser<Unit>.CIChar(character);

        /// <summary>
        /// Creates a parser which parses and returns a character if it is not one of the specified characters.
        /// When the character is one of the given characters, the parser fails without consuming input.
        /// </summary>
        /// <param name="chars">A sequence of characters that should not be matched</param>
        /// <returns>A parser which parses and returns a character that does not match one of the specified characters</returns>
        public static Parser<char, Unit, char> AnyCharExcept(params char[] chars) => Parser<Unit>.AnyCharExcept(chars);

        /// <summary>
        /// Creates a parser which parses and returns a character if it is not one of the specified characters.
        /// When the character is one of the given characters, the parser fails without consuming input.
        /// </summary>
        /// <param name="chars">A sequence of characters that should not be matched</param>
        /// <returns>A parser which parses and returns a character that does not match one of the specified characters</returns>
        public static Parser<char, Unit, char> AnyCharExcept(IEnumerable<char> chars) => Parser<Unit>.AnyCharExcept(chars);

        /// <summary>
        /// A parser that parses and returns a single digit character (0-9)
        /// </summary>
        /// <returns>A parser that parses and returns a single digit character</returns>
        public static Parser<char, Unit, char> Digit { get; } = Parser<Unit>.Digit;

        /// <summary>
        /// A parser that parses and returns a single letter character
        /// </summary>
        /// <returns>A parser that parses and returns a single letter character</returns>
        public static Parser<char, Unit, char> Letter { get; } = Parser<Unit>.Letter;

        /// <summary>
        /// A parser that parses and returns a single letter or digit character
        /// </summary>
        /// <returns>A parser that parses and returns a single letter or digit character</returns>
        public static Parser<char, Unit, char> LetterOrDigit { get; } = Parser<Unit>.LetterOrDigit;

        /// <summary>
        /// A parser that parses and returns a single lowercase letter character
        /// </summary>
        /// <returns>A parser that parses and returns a single lowercase letter character</returns>
        public static Parser<char, Unit, char> Lowercase { get; } = Parser<Unit>.Lowercase;

        /// <summary>
        /// A parser that parses and returns a single uppercase letter character
        /// </summary>
        /// <returns>A parser that parses and returns a single uppercase letter character</returns>
        public static Parser<char, Unit, char> Uppercase { get; } = Parser<Unit>.Uppercase;

        /// <summary>
        /// A parser that parses and returns a single Unicode punctuation character
        /// </summary>
        /// <returns>A parser that parses and returns a single Unicode punctuation character</returns>
        public static Parser<char, Unit, char> Punctuation { get; } = Parser<Unit>.Punctuation;

        /// <summary>
        /// A parser that parses and returns a single Unicode symbol character
        /// </summary>
        /// <returns>A parser that parses and returns a single Unicode symbol character</returns>
        public static Parser<char, Unit, char> Symbol { get; } = Parser<Unit>.Symbol;

        /// <summary>
        /// A parser that parses and returns a single Unicode separator character
        /// </summary>
        /// <returns>A parser that parses and returns a single Unicode separator character</returns>
        public static Parser<char, Unit, char> Separator { get; } = Parser<Unit>.Separator;
    }
}