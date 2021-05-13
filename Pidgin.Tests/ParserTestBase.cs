using System.Collections.Generic;
using Xunit;

namespace Pidgin.Tests
{
    public class ParserTestBase
    {
        protected void AssertSuccess<TToken, TUser, T>(Result<TToken, TUser, T> result, T expected, bool consumedInput)
        {
            Assert.True(result.Success);
            Assert.Equal(expected, result.Value);
            Assert.Equal(consumedInput, result.ConsumedInput);
        }

        protected void AssertFailure<TToken, TUser, T>(Result<TToken, TUser, T> result, ParseError<TToken> expectedError, bool consumedInput)
        {
            Assert.False(result.Success);
            Assert.Equal(expectedError, result.Error);
            Assert.Equal(consumedInput, result.ConsumedInput);
        }
    }
}
