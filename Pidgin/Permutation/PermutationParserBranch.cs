using System;

namespace Pidgin.Permutation
{
    internal abstract class PermutationParserBranch<TToken, TUser, T>
    {
        public abstract PermutationParserBranch<TToken, TUser, R> Add<U, R>(Parser<TToken, TUser, U> parser, Func<T, U, R> resultSelector);

        public abstract PermutationParserBranch<TToken, TUser, R> AddOptional<U, R>(Parser<TToken, TUser, U> parser, Func<U> defaultValueFactory, Func<T, U, R> resultSelector);

        public abstract Parser<TToken, TUser, T> Build();
    }
    internal sealed class PermutationParserBranchImpl<TToken, TUser, U, T, R> : PermutationParserBranch<TToken, TUser, R>
    {
        private readonly Parser<TToken, TUser, U> _parser;
        private readonly PermutationParser<TToken, TUser, T> _perm;
        private readonly Func<T, U, R> _func;

        public PermutationParserBranchImpl(Parser<TToken, TUser, U> parser, PermutationParser<TToken, TUser, T> perm, Func<T, U, R> func)
        {
            _parser = parser;
            _perm = perm;
            _func = func;
        }

        public override PermutationParserBranch<TToken, TUser, W> Add<V, W>(Parser<TToken, TUser, V> parser, Func<R, V, W> resultSelector)
            => Add(p => p.Add(parser), resultSelector);

        public override PermutationParserBranch<TToken, TUser, W> AddOptional<V, W>(Parser<TToken, TUser, V> parser, Func<V> defaultValueFactory, Func<R, V, W> resultSelector)
            => Add(p => p.AddOptional(parser, defaultValueFactory), resultSelector);

        private PermutationParserBranch<TToken, TUser, W> Add<V, W>(Func<PermutationParser<TToken, TUser, T>, PermutationParser<TToken, TUser, (T, V)>> addPerm, Func<R, V, W> resultSelector)
        {
            var this_func = _func;
            return new PermutationParserBranchImpl<TToken, TUser, U, (T, V), W>(
                _parser,
                addPerm(_perm),
                (tv, u) => resultSelector(this_func(tv.Item1, u), tv.Item2)
            );
        }

        public override Parser<TToken, TUser, R> Build()
        {
            var this_func = _func;
            return Parser<TUser>.Map((x, y) => this_func(y, x), _parser, _perm.Build());
        }
    }
}