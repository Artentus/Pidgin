using System.Diagnostics.CodeAnalysis;
using Pidgin.Configuration;

namespace Pidgin
{
    public static partial class Parser<TToken, TUser>
    {
        /// <summary>A parser which returns the current <see cref="IConfiguration{TToken}"/>.</summary>
        public static Parser<TToken, TUser, IConfiguration<TToken>> Configuration { get; } = new ConfigurationParser<TToken, TUser>();
    }

    internal class ConfigurationParser<TToken, TUser> : Parser<TToken, TUser, IConfiguration<TToken>>
    {
        public override bool TryParse(ref ParseState<TToken, TUser> state, ref PooledList<Expected<TToken>> expecteds, [MaybeNullWhen(false)] out IConfiguration<TToken> result)
        {
            result = state.Configuration;
            return true;
        }
    }
}