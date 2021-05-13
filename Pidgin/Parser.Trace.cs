using System;

namespace Pidgin
{
    public partial class Parser<TToken, TUser, T>
    {
        /// <summary>
        /// For debugging use.
        /// 
        /// Creates a new parser which runs the current parser and prints the given message to the console.
        /// </summary>
        /// <returns>A parser which runs the current parser and prints the given message to the console.</returns>
        public Parser<TToken, TUser, T> Trace(Func<T, string> message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            return this.Select(x =>
            {
                Console.WriteLine(message(x));
                return x;
            });
        }

        /// <summary>
        /// For debugging use.
        /// 
        /// Creates a new parser which runs the current parser and prints the given message to the console.
        /// </summary>
        /// <returns>A parser which runs the current parser and prints the given message to the console.</returns>
        public Parser<TToken, TUser, T> Trace(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            return this.Trace(_ => message);
        }

        /// <summary>
        /// For debugging use.
        /// 
        /// Creates a new parser which runs the current parser and prints the result to the console.
        /// </summary>
        /// <returns>A parser which runs the current parser and prints the result to the console.</returns>
        public Parser<TToken, TUser, T> TraceResult() => this.Trace(x => x!.ToString()!);
    }
}