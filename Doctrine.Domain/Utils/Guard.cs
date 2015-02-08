namespace Doctrine.Domain.Utils
{
    using System;

    /// <summary>
    ///     A static helper class that includes various parameter checking routines.
    /// </summary>
    public static class Guard
    {
        #region Public Methods

        /// <summary>
        ///     Throws <see cref="ArgumentNullException" /> if the given argument is null.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value is null.</exception>
        /// <param name="argumentValue">The argument value to test.</param>
        /// <param name="argumentName">The name of the argument to test.</param>
        public static void NotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        ///     Throws an exception if the tested string argument is null or an empty string.
        /// </summary>
        /// <exception cref="ArgumentNullException">The string value is null.</exception>
        /// <exception cref="ArgumentException">The string is empty.</exception>
        /// <param name="argumentValue">The argument value to test.</param>
        /// <param name="argumentName">The name of the argument to test.</param>
        public static void NotNullOrEmpty(string argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            if (argumentValue.Length == 0)
            {
                throw new ArgumentException("The argument cannot be empty.", argumentName);
            }
        }

        /// <summary>
        ///     Throws <see cref="ArgumentOutOfRangeException" /> if the given argument is not greater than 0.
        /// </summary>
        /// <param name="argumentIntValue">The argument value to test.</param>
        /// <param name="argumentName">The name of the argument to test.</param>
        public static void IntMoreThanZero(int argumentIntValue, string argumentName)
        {
            if (argumentIntValue <= 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, "The argument must be greater than 0.");
            }
        }

        #endregion
    }
}