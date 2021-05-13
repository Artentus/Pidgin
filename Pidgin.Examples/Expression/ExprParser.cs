using System;
using System.Collections.Immutable;
using Pidgin.Expression;
using static Pidgin.Parser;

namespace Pidgin.Examples.Expression
{
    public static class ExprParser
    {
        private static Parser<char, Unit, T> Tok<T>(Parser<char, Unit, T> token)
            => Try(token).Before(SkipWhitespaces);
        private static Parser<char, Unit, string> Tok(string token)
            => Tok(String(token));

        private static Parser<char, Unit, T> Parenthesised<T>(Parser<char, Unit, T> parser)
            => parser.Between(Tok("("), Tok(")"));

        private static Parser<char, Unit, Func<IExpr, IExpr, IExpr>> Binary(Parser<char, Unit, BinaryOperatorType> op)
            => op.Select<Func<IExpr, IExpr, IExpr>>(type => (l, r) => new BinaryOp(type, l, r));
        private static Parser<char, Unit, Func<IExpr, IExpr>> Unary(Parser<char, Unit, UnaryOperatorType> op)
            => op.Select<Func<IExpr, IExpr>>(type => o => new UnaryOp(type, o));

        private static readonly Parser<char, Unit, Func<IExpr, IExpr, IExpr>> Add
            = Binary(Tok("+").ThenReturn(BinaryOperatorType.Add));
        private static readonly Parser<char, Unit, Func<IExpr, IExpr, IExpr>> Mul
            = Binary(Tok("*").ThenReturn(BinaryOperatorType.Mul));
        private static readonly Parser<char, Unit, Func<IExpr, IExpr>> Neg
            = Unary(Tok("-").ThenReturn(UnaryOperatorType.Neg));
        private static readonly Parser<char, Unit, Func<IExpr, IExpr>> Complement
            = Unary(Tok("~").ThenReturn(UnaryOperatorType.Complement));

        private static readonly Parser<char, Unit, IExpr> Identifier
            = Tok(Letter.Then(LetterOrDigit.ManyString(), (h, t) => h + t))
                .Select<IExpr>(name => new Identifier(name))
                .Labelled("identifier");
        private static readonly Parser<char, Unit, IExpr> Literal
            = Tok(Num)
                .Select<IExpr>(value => new Literal(value))
                .Labelled("integer literal");

        private static Parser<char, Unit, Func<IExpr, IExpr>> Call(Parser<char, Unit, IExpr> subExpr)
            => Parenthesised(subExpr.Separated(Tok(",")))
                .Select<Func<IExpr, IExpr>>(args => method => new Call(method, args.ToImmutableArray()))
                .Labelled("function call");

        private static readonly Parser<char, Unit, IExpr> Expr = ExpressionParser.Build<char, IExpr>(
            expr => (
                OneOf(
                    Identifier,
                    Literal,
                    Parenthesised(expr).Labelled("parenthesised expression")
                ),
                new[]
                {
                    Operator.PostfixChainable(Call(expr)),
                    Operator.Prefix(Neg).And(Operator.Prefix(Complement)),
                    Operator.InfixL(Mul),
                    Operator.InfixL(Add)
                }
            )
        ).Labelled("expression");

        public static IExpr ParseOrThrow(string input)
            => Expr.ParseOrThrow(input);
    }
}