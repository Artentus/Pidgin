using System;

namespace Pidgin.Comment
{
    /// <summary>
    /// Contains functions to build parsers which skip over comments
    /// </summary>
    public static class CommentParser<TUser>
    {
        /// <summary>
        /// Creates a parser which runs <paramref name="lineCommentStart"/>, then skips the rest of the line.
        /// </summary>
        /// <param name="lineCommentStart">A parser to recognise a lexeme which starts a line comment</param>
        /// <returns>A parser which runs <paramref name="lineCommentStart"/>, then skips the rest of the line.</returns>
        public static Parser<char, TUser, Unit> SkipLineComment<T>(Parser<char, TUser, T> lineCommentStart)
        {
            if (lineCommentStart == null)
            {
                throw new ArgumentNullException(nameof(lineCommentStart));
            }

            var eol = Parser<TUser>.Try(Parser<TUser>.EndOfLine).IgnoreResult();
            return lineCommentStart
                .Then(Parser<char, TUser>.Any.SkipUntil(Parser<char, TUser>.End.Or(eol)))
                .Labelled("line comment");
        }

        /// <summary>
        /// Creates a parser which runs <paramref name="blockCommentStart"/>,
        /// then skips everything until <paramref name="blockCommentEnd"/>.
        /// </summary>
        /// <param name="blockCommentStart">A parser to recognise a lexeme which starts a multi-line block comment</param>
        /// <param name="blockCommentEnd">A parser to recognise a lexeme which ends a multi-line block comment</param>
        /// <returns>
        /// A parser which runs <paramref name="blockCommentStart"/>, then skips everything until <paramref name="blockCommentEnd"/>.
        /// </returns>
        public static Parser<char, TUser, Unit> SkipBlockComment<T, U>(Parser<char, TUser, T> blockCommentStart, Parser<char, TUser, U> blockCommentEnd)
        {
            if (blockCommentStart == null)
            {
                throw new ArgumentNullException(nameof(blockCommentStart));
            }
            if (blockCommentEnd == null)
            {
                throw new ArgumentNullException(nameof(blockCommentEnd));
            }

            return blockCommentStart
                .Then(Parser<char, TUser>.Any.SkipUntil(blockCommentEnd))
                .Labelled("block comment");
        }

        /// <summary>
        /// Creates a parser which runs <paramref name="blockCommentStart"/>,
        /// then skips everything until <paramref name="blockCommentEnd"/>, accounting for nested comments.
        /// </summary>
        /// <param name="blockCommentStart">A parser to recognise a lexeme which starts a multi-line block comment</param>
        /// <param name="blockCommentEnd">A parser to recognise a lexeme which ends a multi-line block comment</param>
        /// <returns>
        /// A parser which runs <paramref name="blockCommentStart"/>,
        /// then skips everything until <paramref name="blockCommentEnd"/>, accounting for nested comments.
        /// </returns>
        public static Parser<char, TUser, Unit> SkipNestedBlockComment<T, U>(Parser<char, TUser, T> blockCommentStart, Parser<char, TUser, U> blockCommentEnd)
        {
            if (blockCommentStart == null)
            {
                throw new ArgumentNullException(nameof(blockCommentStart));
            }
            if (blockCommentEnd == null)
            {
                throw new ArgumentNullException(nameof(blockCommentEnd));
            }

            Parser<char, TUser, Unit>? parser = null;

            parser = blockCommentStart.Then(
                Parser<TUser>.Rec(() => parser!).Or(Parser<char, TUser>.Any.IgnoreResult()).SkipUntil(blockCommentEnd)
            ).Labelled("block comment");

            return parser;
        }
    }



    /// <summary>
    /// Contains functions to build parsers which skip over comments
    /// </summary>
    public static class CommentParser
    {
        /// <summary>
        /// Creates a parser which runs <paramref name="lineCommentStart"/>, then skips the rest of the line.
        /// </summary>
        /// <param name="lineCommentStart">A parser to recognise a lexeme which starts a line comment</param>
        /// <returns>A parser which runs <paramref name="lineCommentStart"/>, then skips the rest of the line.</returns>
        public static Parser<char, Unit, Unit> SkipLineComment<T>(Parser<char, Unit, T> lineCommentStart)
            => CommentParser<Unit>.SkipLineComment(lineCommentStart);

        /// <summary>
        /// Creates a parser which runs <paramref name="blockCommentStart"/>,
        /// then skips everything until <paramref name="blockCommentEnd"/>.
        /// </summary>
        /// <param name="blockCommentStart">A parser to recognise a lexeme which starts a multi-line block comment</param>
        /// <param name="blockCommentEnd">A parser to recognise a lexeme which ends a multi-line block comment</param>
        /// <returns>
        /// A parser which runs <paramref name="blockCommentStart"/>, then skips everything until <paramref name="blockCommentEnd"/>.
        /// </returns>
        public static Parser<char, Unit, Unit> SkipBlockComment<T, U>(Parser<char, Unit, T> blockCommentStart, Parser<char, Unit, U> blockCommentEnd)
            => CommentParser<Unit>.SkipBlockComment(blockCommentStart, blockCommentEnd);

        /// <summary>
        /// Creates a parser which runs <paramref name="blockCommentStart"/>,
        /// then skips everything until <paramref name="blockCommentEnd"/>, accounting for nested comments.
        /// </summary>
        /// <param name="blockCommentStart">A parser to recognise a lexeme which starts a multi-line block comment</param>
        /// <param name="blockCommentEnd">A parser to recognise a lexeme which ends a multi-line block comment</param>
        /// <returns>
        /// A parser which runs <paramref name="blockCommentStart"/>,
        /// then skips everything until <paramref name="blockCommentEnd"/>, accounting for nested comments.
        /// </returns>
        public static Parser<char, Unit, Unit> SkipNestedBlockComment<T, U>(Parser<char, Unit, T> blockCommentStart, Parser<char, Unit, U> blockCommentEnd)
            => CommentParser<Unit>.SkipNestedBlockComment(blockCommentStart, blockCommentEnd);
    }
}