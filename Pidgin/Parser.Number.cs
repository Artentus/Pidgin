using System.Globalization;

namespace Pidgin
{
    public static partial class Parser<TUser>
    {
        private static readonly Parser<char, TUser, string> SignString
            = Char('-').ThenReturn("-")
                .Or(Char('+').ThenReturn("+"))
                .Or(Parser<char, TUser>.Return(""));
        private static readonly Parser<char, TUser, int> Sign
            = Char('+').ThenReturn(1)
                .Or(Char('-').ThenReturn(-1))
                .Or(Parser<char, TUser>.Return(1));

        /// <summary>
        /// A parser which parses a base-10 integer with an optional sign.
        /// The resulting <c>int</c> is not checked for overflow.
        /// </summary>
        /// <returns>A parser which parses a base-10 integer with an optional sign</returns>
        public static Parser<char, TUser, int> DecimalNum { get; } = Int(10).Labelled("number");

        /// <summary>
        /// A parser which parses a base-10 integer with an optional sign.
        /// The resulting <c>int</c> is not checked for overflow.
        /// </summary>
        /// <returns>A parser which parses a base-10 integer with an optional sign</returns>
        public static Parser<char, TUser, int> Num { get; } = DecimalNum;

        /// <summary>
        /// A parser which parses a base-10 long integer with an optional sign.
        /// </summary>
        /// <returns>A parser which parses a base-10 long integer with an optional sign</returns>
        public static Parser<char, TUser, long> LongNum { get; } = Long(10).Labelled("number");

        /// <summary>
        /// A parser which parses a base-8 (octal) integer with an optional sign.
        /// The resulting <c>int</c> is not checked for overflow.
        /// </summary>
        /// <returns>A parser which parses a base-8 (octal) integer with an optional sign</returns>
        public static Parser<char, TUser, int> OctalNum { get; } = Int(8).Labelled("octal number");

        /// <summary>
        /// A parser which parses a base-16 (hexadecimal) integer with an optional sign.
        /// The resulting <c>int</c> is not checked for overflow.
        /// </summary>
        /// <returns>A parser which parses a base-16 (hexadecimal) integer with an optional sign</returns>
        public static Parser<char, TUser, int> HexNum { get; } = Int(16).Labelled("hexadecimal number");

        /// <summary>
        /// A parser which parses an integer in the given base with an optional sign.
        /// The resulting <c>int</c> is not checked for overflow.
        /// </summary>
        /// <param name="base">The base in which the number is notated, between 1 and 36</param>
        /// <returns>A parser which parses an integer with an optional sign</returns>
        public static Parser<char, TUser, int> Int(int @base)
            => Map(
                (sign, num) => sign * num,
                Sign,
                UnsignedInt(@base)
            ).Labelled($"base-{@base} number");

        /// <summary>
        /// A parser which parses an integer in the given base without a sign.
        /// The resulting <c>int</c> is not checked for overflow.
        /// </summary>
        /// <param name="base">The base in which the number is notated, between 1 and 36</param>
        /// <returns>A parser which parses an integer without a sign.</returns>
        public static Parser<char, TUser, int> UnsignedInt(int @base)
            => DigitChar(@base)
                .ChainAtLeastOnce<int, IntChainer>(c => new IntChainer(@base))
                .Labelled($"base-{@base} number");

        private struct IntChainer : IChainer<int, int>
        {
            private readonly int _base;
            private int _result;

            public IntChainer(int @base)
            {
                _base = @base;
                _result = 0;
            }

            public void Apply(int value)
            {
                _result = _result * _base + value;
            }

            public int GetResult() => _result;

            public void OnError()
            {
            }
        }

        /// <summary>
        /// Creates a parser which parses a long integer in the given base with an optional sign.
        /// The resulting <see cref="System.Int64" /> is not checked for overflow.
        /// </summary>
        /// <param name="base">The base in which the number is notated, between 1 and 36</param>
        /// <returns>A parser which parses a long integer with an optional sign</returns>
        public static Parser<char, TUser, long> Long(int @base)
            => Map(
                (sign, num) => sign * num,
                Sign,
                UnsignedLong(@base)
            ).Labelled($"base-{@base} number");

        /// <summary>
        /// A parser which parses a long integer in the given base without a sign.
        /// The resulting <see cref="System.Int64" /> is not checked for overflow.
        /// </summary>
        /// <param name="base">The base in which the number is notated, between 1 and 36</param>
        /// <returns>A parser which parses a long integer without a sign.</returns>
        public static Parser<char, TUser, long> UnsignedLong(int @base)
            => DigitCharLong(@base)
                .ChainAtLeastOnce<long, LongChainer>(c => new LongChainer(@base))
                .Labelled($"base-{@base} number");

        private struct LongChainer : IChainer<long, long>
        {
            private readonly int _base;
            private long _result;

            public LongChainer(int @base)
            {
                _base = @base;
                _result = 0;
            }

            public void Apply(long value)
            {
                _result = _result * _base + value;
            }

            public long GetResult() => _result;

            public void OnError()
            {
            }
        }
        
        private static Parser<char, TUser, int> DigitChar(int @base)
            => @base <= 10
                ? Parser<char, TUser>.Token(c => c >= '0' && c < '0' + @base)
                    .Select(c => GetDigitValue(c))
                : Parser<char, TUser>
                    .Token(c =>
                        c >= '0' && c <= '9'
                        || c >= 'A' && c < 'A' + @base - 10
                        || c >= 'a' && c < 'a' + @base - 10
                    )
                    .Select(c => GetLetterOrDigitValue(c));
        
        private static Parser<char, TUser, long> DigitCharLong(int @base)
            => @base <= 10
                ? Parser<char, TUser>.Token(c => c >= '0' && c < '0' + @base)
                    .Select(c => GetDigitValueLong(c))
                : Parser<char, TUser>
                    .Token(c =>
                        c >= '0' && c <= '9'
                        || c >= 'A' && c < 'A' + @base - 10
                        || c >= 'a' && c < 'a' + @base - 10
                    )
                    .Select(c => GetLetterOrDigitValueLong(c));

        private static int GetDigitValue(char c) => c - '0';
        private static int GetLetterOrDigitValue(char c)
        {
            if (c >= '0' && c <= '9')
            {
                return GetDigitValue(c);
            }
            if (c >= 'A' && c <= 'Z')
            {
                return GetUpperLetterOffset(c) + 10;
            }
            return GetLowerLetterOffset(c) + 10;
        }
        private static int GetUpperLetterOffset(char c) => c - 'A';
        private static int GetLowerLetterOffset(char c) => c - 'a';

        private static long GetDigitValueLong(char c) => c - '0';
        private static long GetLetterOrDigitValueLong(char c)
        {
            if (c >= '0' && c <= '9')
            {
                return GetDigitValueLong(c);
            }
            if (c >= 'A' && c <= 'Z')
            {
                return GetUpperLetterOffsetLong(c) + 10;
            }
            return GetLowerLetterOffsetLong(c) + 10;
        }
        private static long GetUpperLetterOffsetLong(char c) => c - 'A';
        private static long GetLowerLetterOffsetLong(char c) => c - 'a';


        private static Parser<char, TUser, Unit> _fractionalPart
            = Char('.').Then(Digit.SkipAtLeastOnce());
        private static Parser<char, TUser, Unit> _optionalFractionalPart
            = _fractionalPart.Or(Parser<char, TUser>.Return(Unit.Value));
        /// <summary>
        /// A parser which parses a floating point number with an optional sign.
        /// </summary>
        /// <returns>A parser which parses a floating point number with an optional sign</returns>
        public static Parser<char, TUser, double> Real { get; }
            = SignString
                .Then(
                    // if we saw an integral part, the fractional part is optional
                    _fractionalPart
                        .Or(Digit.SkipAtLeastOnce().Then(_optionalFractionalPart))
                )
                .Then(
                    CIChar('e').Then(SignString).Then(Digit.SkipAtLeastOnce())
                        .Or(Parser<char, TUser>.Return(Unit.Value))
                )
                .MapWithInput((span, _) => {
                    var success = double.TryParse(span.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var result);
                    if (success)
                    {
                        return (double?)result;
                    }
                    return (double?)null;
                })
                .Assert(x => x.HasValue, "Couldn't parse a double")
                .Select(x => x!.Value)
                .Labelled($"real number");
    }



    public static partial class Parser
    {
        /// <summary>
        /// A parser which parses a base-10 integer with an optional sign.
        /// The resulting <c>int</c> is not checked for overflow.
        /// </summary>
        /// <returns>A parser which parses a base-10 integer with an optional sign</returns>
        public static Parser<char, Unit, int> DecimalNum { get; } = Parser<Unit>.DecimalNum;

        /// <summary>
        /// A parser which parses a base-10 integer with an optional sign.
        /// The resulting <c>int</c> is not checked for overflow.
        /// </summary>
        /// <returns>A parser which parses a base-10 integer with an optional sign</returns>
        public static Parser<char, Unit, int> Num { get; } = Parser<Unit>.Num;

        /// <summary>
        /// A parser which parses a base-10 long integer with an optional sign.
        /// </summary>
        /// <returns>A parser which parses a base-10 long integer with an optional sign</returns>
        public static Parser<char, Unit, long> LongNum { get; } = Parser<Unit>.LongNum;

        /// <summary>
        /// A parser which parses a base-8 (octal) integer with an optional sign.
        /// The resulting <c>int</c> is not checked for overflow.
        /// </summary>
        /// <returns>A parser which parses a base-8 (octal) integer with an optional sign</returns>
        public static Parser<char, Unit, int> OctalNum { get; } = Parser<Unit>.OctalNum;

        /// <summary>
        /// A parser which parses a base-16 (hexadecimal) integer with an optional sign.
        /// The resulting <c>int</c> is not checked for overflow.
        /// </summary>
        /// <returns>A parser which parses a base-16 (hexadecimal) integer with an optional sign</returns>
        public static Parser<char, Unit, int> HexNum { get; } = Parser<Unit>.HexNum;

        /// <summary>
        /// A parser which parses an integer in the given base with an optional sign.
        /// The resulting <c>int</c> is not checked for overflow.
        /// </summary>
        /// <param name="base">The base in which the number is notated, between 1 and 36</param>
        /// <returns>A parser which parses an integer with an optional sign</returns>
        public static Parser<char, Unit, int> Int(int @base) => Parser<Unit>.Int(@base);

        /// <summary>
        /// A parser which parses an integer in the given base without a sign.
        /// The resulting <c>int</c> is not checked for overflow.
        /// </summary>
        /// <param name="base">The base in which the number is notated, between 1 and 36</param>
        /// <returns>A parser which parses an integer without a sign.</returns>
        public static Parser<char, Unit, int> UnsignedInt(int @base) => Parser<Unit>.UnsignedInt(@base);

        /// <summary>
        /// Creates a parser which parses a long integer in the given base with an optional sign.
        /// The resulting <see cref="System.Int64" /> is not checked for overflow.
        /// </summary>
        /// <param name="base">The base in which the number is notated, between 1 and 36</param>
        /// <returns>A parser which parses a long integer with an optional sign</returns>
        public static Parser<char, Unit, long> Long(int @base) => Parser<Unit>.Long(@base);

        /// <summary>
        /// A parser which parses a long integer in the given base without a sign.
        /// The resulting <see cref="System.Int64" /> is not checked for overflow.
        /// </summary>
        /// <param name="base">The base in which the number is notated, between 1 and 36</param>
        /// <returns>A parser which parses a long integer without a sign.</returns>
        public static Parser<char, Unit, long> UnsignedLong(int @base) => Parser<Unit>.UnsignedLong(@base);

        /// <summary>
        /// A parser which parses a floating point number with an optional sign.
        /// </summary>
        /// <returns>A parser which parses a floating point number with an optional sign</returns>
        public static Parser<char, Unit, double> Real { get; } = Parser<Unit>.Real;
    }
}