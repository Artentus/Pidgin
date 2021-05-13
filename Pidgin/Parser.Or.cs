using System;

namespace Pidgin
{
    public abstract partial class Parser<TToken, TUser, T>
    {
        /// <summary>
        /// Creates a parser which tries to apply the current parser, applying the specified parser if the current parser fails without consuming any input.
        /// The resulting parser fails if both the current parser and the alternative parser fail, or if the current parser fails after consuming input.
        /// </summary>
        /// <param name="parser">The alternative parser to apply if the current parser fails without consuming any input</param>
        /// <returns>A parser which tries to apply the current parser, and applies <paramref name="parser"/> if the current parser fails without consuming any input.</returns>
        public Parser<TToken, TUser, T> Or(Parser<TToken, TUser, T> parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException(nameof(parser));
            }
            return Parser<TUser>.OneOf(this, parser);
        }
    }
}