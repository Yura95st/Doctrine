namespace Doctrine.Domain.Validation.Abstract
{
    public interface IUserValidation
    {
        /// <summary>
        ///     Checks whether string value represents valid email.
        /// </summary>
        /// <param name="email">The email string.</param>
        /// <returns>True if email is valid, false - otherwise</returns>
        bool IsValidEmail(string email);

        /// <summary>
        ///     Checks whether string value represents valid first name.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <returns>True if first name is valid, false - otherwise</returns>
        bool IsValidFirstName(string firstName);

        /// <summary>
        ///     Checks whether string value represents valid last name.
        /// </summary>
        /// <param name="lastName">The last name.</param>
        /// <returns>True if last name is valid, false - otherwise</returns>
        bool IsValidLastName(string lastName);

        /// <summary>
        ///     Checks whether string value represents valid password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>True if password is valid, false - otherwise</returns>
        bool IsValidPassword(string password);
    }
}