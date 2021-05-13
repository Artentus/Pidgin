using Xunit;
using static Pidgin.Parser<char, Pidgin.Tests.AUserState>;

namespace Pidgin.Tests
{
    internal class AUserState
    {
        public int Value { get; set; }
    }

    public class UserStateTests
    {
        [Fact]
        public void TestPropagateUserState()
        {
            var userState = new AUserState() { Value = 0 };

            var parser = Return(Unit.Value).WithUserState().Map(t =>
            {
                t.Item2.Value = 1;
                return t.Item1;
            });
            var result = parser.Parse("", userState);

            Assert.True(result.Success);
            Assert.Equal(userState, result.UserState);
            Assert.Equal(1, result.UserState.Value);
        }

        [Fact]
        public void TestGetUserState()
        {
            var userState = new AUserState();

            var parser = UserState;
            var result = parser.Parse("", userState);

            Assert.True(result.Success);
            Assert.Equal(userState, result.Value);
        }

        [Fact]
        public void TestMapUserState()
        {
            var userStateA = new AUserState();
            var userStateB = new AUserState();

            var parser = Return(Unit.Value).MapUserState((_, _) => userStateB);
            var result = parser.Parse("", userStateA);

            Assert.True(result.Success);
            Assert.Equal(userStateB, result.UserState);
        }
    }
}
