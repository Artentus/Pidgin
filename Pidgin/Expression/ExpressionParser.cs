using System;
using System.Collections.Generic;
using System.Linq;

namespace Pidgin.Expression
{
    /// <summary>
    /// Contains tools for parsing expression languages with associative infix operators.
    /// </summary>
    public static class ExpressionParser<TUser>
    {
        /// <summary>
        /// Builds a parser for expressions built from the operators in <paramref name="operatorTable"/>.
        /// <paramref name="operatorTable"/> is a sequence of operators in precedence order:
        /// the operators in the first row have the highest precedence and operators in later rows have lower precedence.
        /// </summary>
        /// <param name="term">A parser for a single term in an expression language</param>
        /// <param name="operatorTable">A table of operators</param>
        /// <returns>A parser for expressions built from the operators in <paramref name="operatorTable"/>.</returns>
        public static Parser<TToken, TUser, T> Build<TToken, T>(
            Parser<TToken, TUser, T> term,
            IEnumerable<OperatorTableRow<TToken, TUser, T>> operatorTable
        ) => operatorTable.Aggregate(term, Build);

        /// <summary>
        /// Builds a parser for expressions built from the operators in <paramref name="operatorTable"/>.
        /// <paramref name="operatorTable"/> is a sequence of operators in precedence order:
        /// the operators in the first row have the highest precedence and operators in later rows have lower precedence.
        /// </summary>
        /// <param name="term">A parser for a single term in an expression language</param>
        /// <param name="operatorTable">A table of operators</param>
        /// <returns>A parser for expressions built from the operators in <paramref name="operatorTable"/>.</returns>
        public static Parser<TToken, TUser, T> Build<TToken, T>(
            Parser<TToken, TUser, T> term,
            IEnumerable<IEnumerable<OperatorTableRow<TToken, TUser, T>>> operatorTable
        ) => Build(term, Flatten(operatorTable));

        /// <summary>
        /// Builds a parser for expressions built from the operators in <paramref name="operatorTable"/>.
        /// The operator table is a sequence of operators in precedence order:
        /// the operators in the first row have the highest precedence and operators in later rows have lower precedence.
        /// 
        /// This overload is useful for recursive expressions (for example, languages with parenthesised subexpressions).
        /// <paramref name="termFactory"/>'s argument will be a parser which parses a full subexpression.
        /// </summary>
        /// <param name="termFactory">A function which produces a parser for a single term</param>
        /// <param name="operatorTable">A table of operators</param>
        /// <returns>A parser for expressions built from the operators in <paramref name="operatorTable"/>.</returns>
        public static Parser<TToken, TUser, T> Build<TToken, T>(
            Func<Parser<TToken, TUser, T>, Parser<TToken, TUser, T>> termFactory,
            IEnumerable<OperatorTableRow<TToken, TUser, T>> operatorTable
        )
        {
            Parser<TToken, TUser, T>? expr = null;
            var term = termFactory(Parser<TUser>.Rec(() => expr!));
            expr = Build(term, operatorTable);
            return expr;
        }
        
        /// <summary>
        /// Builds a parser for expressions built from the operators in <paramref name="operatorTable"/>.
        /// The operator table is a sequence of operators in precedence order:
        /// the operators in the first row have the highest precedence and operators in later rows have lower precedence.
        /// 
        /// This overload is useful for recursive expressions (for example, languages with parenthesised subexpressions).
        /// <paramref name="termFactory"/>'s argument will be a parser which parses a full subexpression.
        /// </summary>
        /// <param name="termFactory">A function which produces a parser for a single term</param>
        /// <param name="operatorTable">A table of operators</param>
        /// <returns>A parser for expressions built from the operators in <paramref name="operatorTable"/>.</returns>
        public static Parser<TToken, TUser, T> Build<TToken, T>(
            Func<Parser<TToken, TUser, T>, Parser<TToken, TUser, T>> termFactory,
            IEnumerable<IEnumerable<OperatorTableRow<TToken, TUser, T>>> operatorTable
        ) => Build(termFactory, Flatten(operatorTable));

        /// <summary>
        /// Builds a parser for expressions built from the operators in <paramref name="operatorTableFactory"/>'s result.
        /// The operator table is a sequence of operators in precedence order:
        /// the operators in the first row have the highest precedence and operators in later rows have lower precedence.
        /// 
        /// This overload is useful for recursive expressions (for example, languages with parenthesised subexpressions).
        /// <paramref name="operatorTableFactory"/>'s argument will be a parser which parses a full subexpression.
        /// </summary>
        /// <param name="term">A parser for a single term in an expression language</param>
        /// <param name="operatorTableFactory">A function which produces a table of operators</param>
        /// <returns>A parser for expressions built from the operators in the operator table</returns>
        public static Parser<TToken, TUser, T> Build<TToken, T>(
            Parser<TToken, TUser, T> term,
            Func<Parser<TToken, TUser, T>, IEnumerable<OperatorTableRow<TToken, TUser, T>>> operatorTableFactory
        )
        {
            Parser<TToken, TUser, T>? expr = null;
            var operatorTable = operatorTableFactory(Parser<TUser>.Rec(() => expr!));
            expr = Build(term, operatorTable);
            return expr;
        }

        /// <summary>
        /// Builds a parser for expressions built from the operators in <paramref name="operatorTableFactory"/>'s result.
        /// The operator table is a sequence of operators in precedence order:
        /// the operators in the first row have the highest precedence and operators in later rows have lower precedence.
        /// 
        /// This overload is useful for recursive expressions (for example, languages with parenthesised subexpressions).
        /// <paramref name="operatorTableFactory"/>'s argument will be a parser which parses a full subexpression.
        /// </summary>
        /// <param name="term">A parser for a single term in an expression language</param>
        /// <param name="operatorTableFactory">A function which produces a table of operators</param>
        /// <returns>A parser for expressions built from the operators in the operator table</returns>
        public static Parser<TToken, TUser, T> Build<TToken, T>(
            Parser<TToken, TUser, T> term,
            Func<Parser<TToken, TUser, T>, IEnumerable<IEnumerable<OperatorTableRow<TToken, TUser, T>>>> operatorTableFactory
        )
        {
            Parser<TToken, TUser, T>? expr = null;
            var operatorTable = operatorTableFactory(Parser<TUser>.Rec(() => expr!));
            expr = Build(term, operatorTable);
            return expr;
        }

        /// <summary>
        /// Builds a parser for expressions built from the operators in <paramref name="termAndOperatorTableFactory"/>'s second result.
        /// The operator table is a sequence of operators in precedence order:
        /// the operators in the first row have the highest precedence and operators in later rows have lower precedence.
        /// 
        /// This overload is useful for recursive expressions (for example, languages with parenthesised subexpressions).
        /// <paramref name="termAndOperatorTableFactory"/>'s argument will be a parser which parses a full subexpression.
        /// </summary>
        /// <param name="termAndOperatorTableFactory">A function which produces a parser for a single term and a table of operators</param>
        /// <returns>A parser for expressions built from the operators in the operator table</returns>
        public static Parser<TToken, TUser, T> Build<TToken, T>(
            Func<Parser<TToken, TUser, T>, (Parser<TToken, TUser, T> term, IEnumerable<OperatorTableRow<TToken, TUser, T>> operatorTable)> termAndOperatorTableFactory
        )
        {
            Parser<TToken, TUser, T>? expr = null;
            var (term, operatorTable) = termAndOperatorTableFactory(Parser<TUser>.Rec(() => expr!));
            expr = Build(term, operatorTable);
            return expr;
        }

        /// <summary>
        /// Builds a parser for expressions built from the operators in <paramref name="termAndOperatorTableFactory"/>'s second result.
        /// The operator table is a sequence of operators in precedence order:
        /// the operators in the first row have the highest precedence and operators in later rows have lower precedence.
        /// 
        /// This overload is useful for recursive expressions (for example, languages with parenthesised subexpressions).
        /// <paramref name="termAndOperatorTableFactory"/>'s argument will be a parser which parses a full subexpression.
        /// </summary>
        /// <param name="termAndOperatorTableFactory">A function which produces a parser for a single term and a table of operators</param>
        /// <returns>A parser for expressions built from the operators in the operator table</returns>
        public static Parser<TToken, TUser, T> Build<TToken, T>(
            Func<Parser<TToken, TUser, T>, (Parser<TToken, TUser, T> term, IEnumerable<IEnumerable<OperatorTableRow<TToken, TUser, T>>> operatorTable)> termAndOperatorTableFactory
        )
        {
            Parser<TToken, TUser, T>? expr = null;
            var (term, operatorTable) = termAndOperatorTableFactory(Parser<TUser>.Rec(() => expr!));
            expr = Build(term, operatorTable);
            return expr;
        }


        private static Parser<TToken, TUser, T> Build<TToken, T>(Parser<TToken, TUser, T> term, OperatorTableRow<TToken, TUser, T> row)
        {
            var returnIdentity = Parser<TToken, TUser>.Return<Func<T,T>>(x => x);
            var returnIdentityArray = new[]{ returnIdentity };

            var pTerm = Parser<TUser>.Map(
                (pre, tm, post) => post(pre(tm)),
                Parser<TUser>.OneOf(row.PrefixOps.Concat(returnIdentityArray)),
                term,
                Parser<TUser>.OneOf(row.PostfixOps.Concat(returnIdentityArray))
            );

            var infixN = Op(pTerm, row.InfixNOps).Select<Func<T, T>>(p => z => p.ApplyL(z));
            var infixL = Op(pTerm, row.InfixLOps)
                .AtLeastOncePooled()
                .Select<Func<T, T>>(fxs =>
                    z =>
                    {
                        for (var i = 0; i < fxs.Count; i++)
                        {
                            z = fxs[i].ApplyL(z);
                        }
                        fxs.Dispose();
                        return z;
                    }
                );
            var infixR = Op(pTerm, row.InfixROps)
                .AtLeastOncePooled()
                .Select<Func<T, T>>(fxs =>
                    {
                        // reassociate the parsed operators:
                        // move the right-hand term of each operator to the
                        // left-hand side of the next operator on the right,
                        // leaving a hole at the left
                        var partial = new Partial<T>((y, _) => y, default(T)!);
                        for (var i = fxs.Count - 1; i >= 0; i--)
                        {
                            var fx = fxs[i];
                            partial = new Partial<T>(fx.Func, partial.ApplyL(fx.Arg));
                        }
                        fxs.Dispose();
                        return z => partial.ApplyL(z);
                    }
                );
            
            var op = Parser<TUser>.OneOf(
                infixN,
                infixL,
                infixR,
                returnIdentity
            );
            
            return Parser<TUser>.Map(
                (x, f) => f(x),
                pTerm,
                op
            );
        }

        private static Parser<TToken, TUser, Partial<T>> Op<TToken, T>(Parser<TToken, TUser, T> pTerm, IEnumerable<Parser<TToken, TUser, Func<T, T, T>>> ops)
            => Parser<TUser>.Map(
                (f, y) => new Partial<T>(f, y),
                Parser<TUser>.OneOf(ops),
                pTerm
            );

        private static IEnumerable<OperatorTableRow<TToken, TUser, T>> Flatten<TToken, T>(IEnumerable<IEnumerable<OperatorTableRow<TToken, TUser, T>>> operatorTable)
            => operatorTable.Select(r => r.Aggregate(OperatorTableRow<TToken, TUser, T>.Empty, (p, q) => p.And(q)));

        private struct Partial<T>
        {
            public Func<T, T, T> Func { get; }
            public T Arg { get; }
            public Partial(Func<T, T, T> func, T arg)
            {
                Func = func;
                Arg = arg;
            }
            public T ApplyL(T arg) => Func(arg, Arg);
        }
    }



    /// <summary>
    /// Contains tools for parsing expression languages with associative infix operators.
    /// </summary>
    public static class ExpressionParser
    {
        /// <summary>
        /// Builds a parser for expressions built from the operators in <paramref name="operatorTable"/>.
        /// <paramref name="operatorTable"/> is a sequence of operators in precedence order:
        /// the operators in the first row have the highest precedence and operators in later rows have lower precedence.
        /// </summary>
        /// <param name="term">A parser for a single term in an expression language</param>
        /// <param name="operatorTable">A table of operators</param>
        /// <returns>A parser for expressions built from the operators in <paramref name="operatorTable"/>.</returns>
        public static Parser<TToken, Unit, T> Build<TToken, T>(
            Parser<TToken, Unit, T> term,
            IEnumerable<OperatorTableRow<TToken, Unit, T>> operatorTable)
            => ExpressionParser<Unit>.Build(term, operatorTable);

        /// <summary>
        /// Builds a parser for expressions built from the operators in <paramref name="operatorTable"/>.
        /// <paramref name="operatorTable"/> is a sequence of operators in precedence order:
        /// the operators in the first row have the highest precedence and operators in later rows have lower precedence.
        /// </summary>
        /// <param name="term">A parser for a single term in an expression language</param>
        /// <param name="operatorTable">A table of operators</param>
        /// <returns>A parser for expressions built from the operators in <paramref name="operatorTable"/>.</returns>
        public static Parser<TToken, Unit, T> Build<TToken, T>(
            Parser<TToken, Unit, T> term,
            IEnumerable<IEnumerable<OperatorTableRow<TToken, Unit, T>>> operatorTable)
            => ExpressionParser<Unit>.Build(term, operatorTable);

        /// <summary>
        /// Builds a parser for expressions built from the operators in <paramref name="operatorTable"/>.
        /// The operator table is a sequence of operators in precedence order:
        /// the operators in the first row have the highest precedence and operators in later rows have lower precedence.
        /// 
        /// This overload is useful for recursive expressions (for example, languages with parenthesised subexpressions).
        /// <paramref name="termFactory"/>'s argument will be a parser which parses a full subexpression.
        /// </summary>
        /// <param name="termFactory">A function which produces a parser for a single term</param>
        /// <param name="operatorTable">A table of operators</param>
        /// <returns>A parser for expressions built from the operators in <paramref name="operatorTable"/>.</returns>
        public static Parser<TToken, Unit, T> Build<TToken, T>(
            Func<Parser<TToken, Unit, T>, Parser<TToken, Unit, T>> termFactory,
            IEnumerable<OperatorTableRow<TToken, Unit, T>> operatorTable)
            => ExpressionParser<Unit>.Build(termFactory, operatorTable);

        /// <summary>
        /// Builds a parser for expressions built from the operators in <paramref name="operatorTable"/>.
        /// The operator table is a sequence of operators in precedence order:
        /// the operators in the first row have the highest precedence and operators in later rows have lower precedence.
        /// 
        /// This overload is useful for recursive expressions (for example, languages with parenthesised subexpressions).
        /// <paramref name="termFactory"/>'s argument will be a parser which parses a full subexpression.
        /// </summary>
        /// <param name="termFactory">A function which produces a parser for a single term</param>
        /// <param name="operatorTable">A table of operators</param>
        /// <returns>A parser for expressions built from the operators in <paramref name="operatorTable"/>.</returns>
        public static Parser<TToken, Unit, T> Build<TToken, T>(
            Func<Parser<TToken, Unit, T>, Parser<TToken, Unit, T>> termFactory,
            IEnumerable<IEnumerable<OperatorTableRow<TToken, Unit, T>>> operatorTable)
            => ExpressionParser<Unit>.Build(termFactory, operatorTable);

        /// <summary>
        /// Builds a parser for expressions built from the operators in <paramref name="operatorTableFactory"/>'s result.
        /// The operator table is a sequence of operators in precedence order:
        /// the operators in the first row have the highest precedence and operators in later rows have lower precedence.
        /// 
        /// This overload is useful for recursive expressions (for example, languages with parenthesised subexpressions).
        /// <paramref name="operatorTableFactory"/>'s argument will be a parser which parses a full subexpression.
        /// </summary>
        /// <param name="term">A parser for a single term in an expression language</param>
        /// <param name="operatorTableFactory">A function which produces a table of operators</param>
        /// <returns>A parser for expressions built from the operators in the operator table</returns>
        public static Parser<TToken, Unit, T> Build<TToken, T>(
            Parser<TToken, Unit, T> term,
            Func<Parser<TToken, Unit, T>, IEnumerable<OperatorTableRow<TToken, Unit, T>>> operatorTableFactory)
            => ExpressionParser<Unit>.Build(term, operatorTableFactory);

        /// <summary>
        /// Builds a parser for expressions built from the operators in <paramref name="operatorTableFactory"/>'s result.
        /// The operator table is a sequence of operators in precedence order:
        /// the operators in the first row have the highest precedence and operators in later rows have lower precedence.
        /// 
        /// This overload is useful for recursive expressions (for example, languages with parenthesised subexpressions).
        /// <paramref name="operatorTableFactory"/>'s argument will be a parser which parses a full subexpression.
        /// </summary>
        /// <param name="term">A parser for a single term in an expression language</param>
        /// <param name="operatorTableFactory">A function which produces a table of operators</param>
        /// <returns>A parser for expressions built from the operators in the operator table</returns>
        public static Parser<TToken, Unit, T> Build<TToken, T>(
            Parser<TToken, Unit, T> term,
            Func<Parser<TToken, Unit, T>, IEnumerable<IEnumerable<OperatorTableRow<TToken, Unit, T>>>> operatorTableFactory)
            => ExpressionParser<Unit>.Build(term, operatorTableFactory);

        /// <summary>
        /// Builds a parser for expressions built from the operators in <paramref name="termAndOperatorTableFactory"/>'s second result.
        /// The operator table is a sequence of operators in precedence order:
        /// the operators in the first row have the highest precedence and operators in later rows have lower precedence.
        /// 
        /// This overload is useful for recursive expressions (for example, languages with parenthesised subexpressions).
        /// <paramref name="termAndOperatorTableFactory"/>'s argument will be a parser which parses a full subexpression.
        /// </summary>
        /// <param name="termAndOperatorTableFactory">A function which produces a parser for a single term and a table of operators</param>
        /// <returns>A parser for expressions built from the operators in the operator table</returns>
        public static Parser<TToken, Unit, T> Build<TToken, T>(
            Func<Parser<TToken, Unit, T>, (Parser<TToken, Unit, T>, IEnumerable<OperatorTableRow<TToken, Unit, T>>)> termAndOperatorTableFactory)
            => ExpressionParser<Unit>.Build(termAndOperatorTableFactory);

        /// <summary>
        /// Builds a parser for expressions built from the operators in <paramref name="termAndOperatorTableFactory"/>'s second result.
        /// The operator table is a sequence of operators in precedence order:
        /// the operators in the first row have the highest precedence and operators in later rows have lower precedence.
        /// 
        /// This overload is useful for recursive expressions (for example, languages with parenthesised subexpressions).
        /// <paramref name="termAndOperatorTableFactory"/>'s argument will be a parser which parses a full subexpression.
        /// </summary>
        /// <param name="termAndOperatorTableFactory">A function which produces a parser for a single term and a table of operators</param>
        /// <returns>A parser for expressions built from the operators in the operator table</returns>
        public static Parser<TToken, Unit, T> Build<TToken, T>(
            Func<Parser<TToken, Unit, T>, (Parser<TToken, Unit, T>, IEnumerable<IEnumerable<OperatorTableRow<TToken, Unit, T>>>)> termAndOperatorTableFactory)
            => ExpressionParser<Unit>.Build(termAndOperatorTableFactory);
    }
}