using System;
using System.Collections.Generic;
using System.Linq;

namespace Pidgin.Expression
{
    /// <summary>
    /// Methods to create <see cref="OperatorTableRow{TToken, TUser, T}"/> values.
    /// </summary>
    public static class Operator<TUser>
    {
        /// <summary>
        /// Creates a row in a table of operators which contains a single binary infix operator with the specified associativity.
        /// Can be combined with other <see cref="OperatorTableRow{TToken, TUser, T}"/>s using
        /// <see cref="OperatorTableRow{TToken, TUser, T}.And(OperatorTableRow{TToken, TUser, T})"/> to build a larger row.
        /// </summary>
        /// <param name="type">The associativity of the infix operator</param>
        /// <param name="opParser">A parser for an infix operator</param>
        /// <returns>A row in a table of operators which contains a single infix operator.</returns>
        public static OperatorTableRow<TToken, TUser, T> Binary<TToken, T>(
            BinaryOperatorType type,
            Parser<TToken, TUser, Func<T, T, T>> opParser
        )
        {
            if (opParser == null)
            {
                throw new ArgumentNullException(nameof(opParser));
            }
            switch (type)
            {
                case BinaryOperatorType.NonAssociative:
                    return InfixN(opParser);
                case BinaryOperatorType.LeftAssociative:
                    return InfixL(opParser);
                case BinaryOperatorType.RightAssociative:
                    return InfixR(opParser);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        /// <summary>
        /// Creates a row in a table of operators which contains a single unary operator - either a prefix operator or a postfix operator.
        /// Can be combined with other <see cref="OperatorTableRow{TToken, TUser, T}"/>s using
        /// <see cref="OperatorTableRow{TToken, TUser, T}.And(OperatorTableRow{TToken, TUser, T})"/> to build a larger row.
        /// </summary>
        /// <param name="type">The type of the unary operator</param>
        /// <param name="opParser">A parser for a unary operator</param>
        /// <returns>A row in a table of operators which contains a single unary operator.</returns>
        public static OperatorTableRow<TToken, TUser, T> Unary<TToken, T>(
            UnaryOperatorType type,
            Parser<TToken, TUser, Func<T, T>> opParser
        )
        {
            if (opParser == null)
            {
                throw new ArgumentNullException(nameof(opParser));
            }
            switch (type)
            {
                case UnaryOperatorType.Prefix:
                    return Prefix(opParser);
                case UnaryOperatorType.Postfix:
                    return Postfix(opParser);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        /// <summary>
        /// Creates a row in a table of operators which contains a single non-associative infix operator.
        /// Can be combined with other <see cref="OperatorTableRow{TToken, TUser, T}"/>s using
        /// <see cref="OperatorTableRow{TToken, TUser, T}.And(OperatorTableRow{TToken, TUser, T})"/> to build a larger row.
        /// </summary>
        /// <param name="opParser">A parser for an infix operator</param>
        /// <returns>A row in a table of operators which contains a single infix operator.</returns>
        public static OperatorTableRow<TToken, TUser, T> InfixN<TToken, T>(Parser<TToken, TUser, Func<T, T, T>> opParser)
        {
            if (opParser == null)
            {
                throw new ArgumentNullException(nameof(opParser));
            }
            return new OperatorTableRow<TToken, TUser, T>(new[]{ opParser }, null, null, null, null );
        }

        /// <summary>
        /// Creates a row in a table of operators which contains a single left-associative infix operator.
        /// Can be combined with other <see cref="OperatorTableRow{TToken, TUser, T}"/>s using
        /// <see cref="OperatorTableRow{TToken, TUser, T}.And(OperatorTableRow{TToken, TUser, T})"/> to build a larger row.
        /// </summary>
        /// <param name="opParser">A parser for an infix operator</param>
        /// <returns>A row in a table of operators which contains a single infix operator.</returns>
        public static OperatorTableRow<TToken, TUser, T> InfixL<TToken, T>(Parser<TToken, TUser, Func<T, T, T>> opParser)
        {
            if (opParser == null)
            {
                throw new ArgumentNullException(nameof(opParser));
            }
            return new OperatorTableRow<TToken, TUser, T>(null, new[]{ opParser }, null, null, null);
        }

        /// <summary>
        /// Creates a row in a table of operators which contains a single right-associative infix operator.
        /// Can be combined with other <see cref="OperatorTableRow{TToken, TUser, T}"/>s using
        /// <see cref="OperatorTableRow{TToken, TUser, T}.And(OperatorTableRow{TToken, TUser, T})"/> to build a larger row.
        /// </summary>
        /// <param name="opParser">A parser for an infix operator</param>
        /// <returns>A row in a table of operators which contains a single infix operator.</returns>
        public static OperatorTableRow<TToken, TUser, T> InfixR<TToken, T>(Parser<TToken, TUser, Func<T, T, T>> opParser)
        {
            if (opParser == null)
            {
                throw new ArgumentNullException(nameof(opParser));
            }
            return new OperatorTableRow<TToken, TUser, T>(null, null, new[]{ opParser }, null, null);
        }

        /// <summary>
        /// Creates a row in a table of operators which contains a single prefix operator.
        /// Can be combined with other <see cref="OperatorTableRow{TToken, TUser, T}"/>s using
        /// <see cref="OperatorTableRow{TToken, TUser, T}.And(OperatorTableRow{TToken, TUser, T})"/> to build a larger row.
        /// </summary>
        /// <param name="opParser">A parser for an prefix operator</param>
        /// <returns>A row in a table of operators which contains a single prefix operator.</returns>
        public static OperatorTableRow<TToken, TUser, T> Prefix<TToken, T>(Parser<TToken, TUser, Func<T, T>> opParser)
        {
            if (opParser == null)
            {
                throw new ArgumentNullException(nameof(opParser));
            }
            return new OperatorTableRow<TToken, TUser, T>(null, null, null, new[]{ opParser }, null);
        }

        /// <summary>
        /// Creates a row in a table of operators which contains a single postfix operator.
        /// Can be combined with other <see cref="OperatorTableRow{TToken, TUser, T}"/>s using
        /// <see cref="OperatorTableRow{TToken, TUser, T}.And(OperatorTableRow{TToken, TUser, T})"/> to build a larger row.
        /// </summary>
        /// <param name="opParser">A parser for an postfix operator</param>
        /// <returns>A row in a table of operators which contains a single postfix operator.</returns>
        public static OperatorTableRow<TToken, TUser, T> Postfix<TToken, T>(Parser<TToken, TUser, Func<T, T>> opParser)
        {
            if (opParser == null)
            {
                throw new ArgumentNullException(nameof(opParser));
            }
            return new OperatorTableRow<TToken, TUser, T>(null, null, null, null, new[]{ opParser });
        }
        
        /// <summary>
        /// Creates a row in a table of operators which contains a chainable collection of prefix operators.
        /// By default <see cref="Prefix"/> operators can only appear onc, so <c>- - 1</c> would not be parsed as "minus minus 1".
        /// 
        /// This method is equivalent to:
        /// <code>
        /// Prefix(
        ///     OneOf(opParsers)
        ///         .AtLeastOnce()
        ///         .Select&lt;Func&lt;T, T&gt;&gt;(fs => z => fs.AggregateR(z, (f, x) => f(x)))
        /// )
        /// </code>
        /// </summary>
        /// <param name="opParsers">A collection of parsers for individual prefix operators</param>
        /// <returns>A row in a table of operators which contains a chainable collection of prefix operators</returns>
        public static OperatorTableRow<TToken, TUser, T> PrefixChainable<TToken, T>(IEnumerable<Parser<TToken, TUser, Func<T, T>>> opParsers)
            => Prefix(
                Parser<TUser>.OneOf(opParsers)
                    .AtLeastOncePooled()
                    .Select<Func<T, T>>(fs =>
                        z =>
                        {
                            for (var i = fs.Count - 1; i >= 0; i--)
                            {
                                z = fs[i](z);
                            }
                            fs.Dispose();
                            return z;
                        }
                    )
            );
        /// <summary>
        /// Creates a row in a table of operators which contains a chainable collection of prefix operators.
        /// By default <see cref="Prefix"/> operators can only appear onc, so <c>- - 1</c> would not be parsed as "minus minus 1".
        /// 
        /// This method is equivalent to:
        /// <code>
        /// Prefix(
        ///     OneOf(opParsers)
        ///         .AtLeastOnce()
        ///         .Select&lt;Func&lt;T, T&gt;&gt;(fs => z => fs.AggregateR(z, (f, x) => f(x)))
        /// )
        /// </code>
        /// </summary>
        /// <param name="opParsers">A collection of parsers for individual prefix operators</param>
        /// <returns>A row in a table of operators which contains a chainable collection of prefix operators</returns>
        public static OperatorTableRow<TToken, TUser, T> PrefixChainable<TToken, T>(params Parser<TToken, TUser, Func<T, T>>[] opParsers)
            => PrefixChainable(opParsers.AsEnumerable());
        
        /// <summary>
        /// Creates a row in a table of operators which contains a chainable collection of postfix operators.
        /// By default <see cref="Postfix"/> operators can only appear onc, so <c>foo()()</c> would not be parsed as "call(call(foo))".
        /// 
        /// This method is equivalent to:
        /// <code>
        /// Postfix(
        ///     OneOf(opParsers)
        ///         .AtLeastOnce()
        ///         .Select&lt;Func&lt;T, T&gt;&gt;(fs => z => fs.Aggregate(z, (x, f) => f(x)))
        /// )
        /// </code>
        /// </summary>
        /// <param name="opParsers">A collection of parsers for individual postfix operators</param>
        /// <returns>A row in a table of operators which contains a chainable collection of postfix operators</returns>
        public static OperatorTableRow<TToken, TUser, T> PostfixChainable<TToken, T>(IEnumerable<Parser<TToken, TUser, Func<T, T>>> opParsers)
            => Postfix(
                Parser<TUser>.OneOf(opParsers)
                    .AtLeastOncePooled()
                    .Select<Func<T, T>>(fs =>
                        z =>
                        {
                            for (var i = 0; i < fs.Count; i++)
                            {
                                z = fs[i](z);
                            }
                            fs.Dispose();
                            return z;
                        }
                    )
            );
        /// <summary>
        /// Creates a row in a table of operators which contains a chainable collection of postfix operators.
        /// By default <see cref="Postfix"/> operators can only appear onc, so <c>foo()()</c> would not be parsed as "call(call(foo))".
        /// 
        /// This method is equivalent to:
        /// <code>
        /// Postfix(
        ///     OneOf(opParsers)
        ///         .AtLeastOnce()
        ///         .Select&lt;Func&lt;T, T&gt;&gt;(fs => z => fs.Aggregate(z, (x, f) => f(x)))
        /// )
        /// </code>
        /// </summary>
        /// <param name="opParsers">A collection of parsers for individual postfix operators</param>
        /// <returns>A row in a table of operators which contains a chainable collection of postfix operators</returns>
        public static OperatorTableRow<TToken, TUser, T> PostfixChainable<TToken, T>(params Parser<TToken, TUser, Func<T, T>>[] opParsers)
            => PostfixChainable(opParsers.AsEnumerable());
    }



    /// <summary>
    /// Methods to create <see cref="OperatorTableRow{TToken, TUser, T}"/> values.
    /// </summary>
    public static class Operator
    {
        /// <summary>
        /// Creates a row in a table of operators which contains a single binary infix operator with the specified associativity.
        /// Can be combined with other <see cref="OperatorTableRow{TToken, TUser, T}"/>s using
        /// <see cref="OperatorTableRow{TToken, TUser, T}.And(OperatorTableRow{TToken, TUser, T})"/> to build a larger row.
        /// </summary>
        /// <param name="type">The associativity of the infix operator</param>
        /// <param name="opParser">A parser for an infix operator</param>
        /// <returns>A row in a table of operators which contains a single infix operator.</returns>
        public static OperatorTableRow<TToken, Unit, T> Binary<TToken, T>(
            BinaryOperatorType type,
            Parser<TToken, Unit, Func<T, T, T>> opParser)
            => Operator<Unit>.Binary(type, opParser);

        /// <summary>
        /// Creates a row in a table of operators which contains a single unary operator - either a prefix operator or a postfix operator.
        /// Can be combined with other <see cref="OperatorTableRow{TToken, TUser, T}"/>s using
        /// <see cref="OperatorTableRow{TToken, TUser, T}.And(OperatorTableRow{TToken, TUser, T})"/> to build a larger row.
        /// </summary>
        /// <param name="type">The type of the unary operator</param>
        /// <param name="opParser">A parser for a unary operator</param>
        /// <returns>A row in a table of operators which contains a single unary operator.</returns>
        public static OperatorTableRow<TToken, Unit, T> Unary<TToken, T>(
            UnaryOperatorType type,
            Parser<TToken, Unit, Func<T, T>> opParser)
            => Operator<Unit>.Unary(type, opParser);

        /// <summary>
        /// Creates a row in a table of operators which contains a single non-associative infix operator.
        /// Can be combined with other <see cref="OperatorTableRow{TToken, TUser, T}"/>s using
        /// <see cref="OperatorTableRow{TToken, TUser, T}.And(OperatorTableRow{TToken, TUser, T})"/> to build a larger row.
        /// </summary>
        /// <param name="opParser">A parser for an infix operator</param>
        /// <returns>A row in a table of operators which contains a single infix operator.</returns>
        public static OperatorTableRow<TToken, Unit, T> InfixN<TToken, T>(Parser<TToken, Unit, Func<T, T, T>> opParser)
            => Operator<Unit>.InfixN(opParser);

        /// <summary>
        /// Creates a row in a table of operators which contains a single left-associative infix operator.
        /// Can be combined with other <see cref="OperatorTableRow{TToken, TUser, T}"/>s using
        /// <see cref="OperatorTableRow{TToken, TUser, T}.And(OperatorTableRow{TToken, TUser, T})"/> to build a larger row.
        /// </summary>
        /// <param name="opParser">A parser for an infix operator</param>
        /// <returns>A row in a table of operators which contains a single infix operator.</returns>
        public static OperatorTableRow<TToken, Unit, T> InfixL<TToken, T>(Parser<TToken, Unit, Func<T, T, T>> opParser)
            => Operator<Unit>.InfixL(opParser);

        /// <summary>
        /// Creates a row in a table of operators which contains a single right-associative infix operator.
        /// Can be combined with other <see cref="OperatorTableRow{TToken, TUser, T}"/>s using
        /// <see cref="OperatorTableRow{TToken, TUser, T}.And(OperatorTableRow{TToken, TUser, T})"/> to build a larger row.
        /// </summary>
        /// <param name="opParser">A parser for an infix operator</param>
        /// <returns>A row in a table of operators which contains a single infix operator.</returns>
        public static OperatorTableRow<TToken, Unit, T> InfixR<TToken, T>(Parser<TToken, Unit, Func<T, T, T>> opParser)
            => Operator<Unit>.InfixR(opParser);

        /// <summary>
        /// Creates a row in a table of operators which contains a single prefix operator.
        /// Can be combined with other <see cref="OperatorTableRow{TToken, TUser, T}"/>s using
        /// <see cref="OperatorTableRow{TToken, TUser, T}.And(OperatorTableRow{TToken, TUser, T})"/> to build a larger row.
        /// </summary>
        /// <param name="opParser">A parser for an prefix operator</param>
        /// <returns>A row in a table of operators which contains a single prefix operator.</returns>
        public static OperatorTableRow<TToken, Unit, T> Prefix<TToken, T>(Parser<TToken, Unit, Func<T, T>> opParser)
            => Operator<Unit>.Prefix(opParser);

        /// <summary>
        /// Creates a row in a table of operators which contains a single postfix operator.
        /// Can be combined with other <see cref="OperatorTableRow{TToken, TUser, T}"/>s using
        /// <see cref="OperatorTableRow{TToken, TUser, T}.And(OperatorTableRow{TToken, TUser, T})"/> to build a larger row.
        /// </summary>
        /// <param name="opParser">A parser for an postfix operator</param>
        /// <returns>A row in a table of operators which contains a single postfix operator.</returns>
        public static OperatorTableRow<TToken, Unit, T> Postfix<TToken, T>(Parser<TToken, Unit, Func<T, T>> opParser)
            => Operator<Unit>.Postfix(opParser);

        /// <summary>
        /// Creates a row in a table of operators which contains a chainable collection of prefix operators.
        /// By default <see cref="Prefix"/> operators can only appear onc, so <c>- - 1</c> would not be parsed as "minus minus 1".
        /// 
        /// This method is equivalent to:
        /// <code>
        /// Prefix(
        ///     OneOf(opParsers)
        ///         .AtLeastOnce()
        ///         .Select&lt;Func&lt;T, T&gt;&gt;(fs => z => fs.AggregateR(z, (f, x) => f(x)))
        /// )
        /// </code>
        /// </summary>
        /// <param name="opParsers">A collection of parsers for individual prefix operators</param>
        /// <returns>A row in a table of operators which contains a chainable collection of prefix operators</returns>
        public static OperatorTableRow<TToken, Unit, T> PrefixChainable<TToken, T>(IEnumerable<Parser<TToken, Unit, Func<T, T>>> opParsers)
            => Operator<Unit>.PrefixChainable(opParsers);

        /// <summary>
        /// Creates a row in a table of operators which contains a chainable collection of prefix operators.
        /// By default <see cref="Prefix"/> operators can only appear onc, so <c>- - 1</c> would not be parsed as "minus minus 1".
        /// 
        /// This method is equivalent to:
        /// <code>
        /// Prefix(
        ///     OneOf(opParsers)
        ///         .AtLeastOnce()
        ///         .Select&lt;Func&lt;T, T&gt;&gt;(fs => z => fs.AggregateR(z, (f, x) => f(x)))
        /// )
        /// </code>
        /// </summary>
        /// <param name="opParsers">A collection of parsers for individual prefix operators</param>
        /// <returns>A row in a table of operators which contains a chainable collection of prefix operators</returns>
        public static OperatorTableRow<TToken, Unit, T> PrefixChainable<TToken, T>(params Parser<TToken, Unit, Func<T, T>>[] opParsers)
            => Operator<Unit>.PrefixChainable(opParsers);

        /// <summary>
        /// Creates a row in a table of operators which contains a chainable collection of postfix operators.
        /// By default <see cref="Postfix"/> operators can only appear onc, so <c>foo()()</c> would not be parsed as "call(call(foo))".
        /// 
        /// This method is equivalent to:
        /// <code>
        /// Postfix(
        ///     OneOf(opParsers)
        ///         .AtLeastOnce()
        ///         .Select&lt;Func&lt;T, T&gt;&gt;(fs => z => fs.Aggregate(z, (x, f) => f(x)))
        /// )
        /// </code>
        /// </summary>
        /// <param name="opParsers">A collection of parsers for individual postfix operators</param>
        /// <returns>A row in a table of operators which contains a chainable collection of postfix operators</returns>
        public static OperatorTableRow<TToken, Unit, T> PostfixChainable<TToken, T>(IEnumerable<Parser<TToken, Unit, Func<T, T>>> opParsers)
            => Operator<Unit>.PostfixChainable(opParsers);

        /// <summary>
        /// Creates a row in a table of operators which contains a chainable collection of postfix operators.
        /// By default <see cref="Postfix"/> operators can only appear onc, so <c>foo()()</c> would not be parsed as "call(call(foo))".
        /// 
        /// This method is equivalent to:
        /// <code>
        /// Postfix(
        ///     OneOf(opParsers)
        ///         .AtLeastOnce()
        ///         .Select&lt;Func&lt;T, T&gt;&gt;(fs => z => fs.Aggregate(z, (x, f) => f(x)))
        /// )
        /// </code>
        /// </summary>
        /// <param name="opParsers">A collection of parsers for individual postfix operators</param>
        /// <returns>A row in a table of operators which contains a chainable collection of postfix operators</returns>
        public static OperatorTableRow<TToken, Unit, T> PostfixChainable<TToken, T>(params Parser<TToken, Unit, Func<T, T>>[] opParsers)
            => Operator<Unit>.PostfixChainable(opParsers);
    }
}