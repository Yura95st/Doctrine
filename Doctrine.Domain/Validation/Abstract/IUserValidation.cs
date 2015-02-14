namespace Doctrine.Domain.Validation.Abstract
{
    using Doctrine.Domain.Enums;

    public interface IUserValidation
    {
        /// <summary>Gets the password's strength.</summary>
        /// <param name="password">The password.</param>
        /// <returns>The password's strength.</returns>
        PasswordStrength GetPasswordStrength(string password);

        /// <summary>
        ///     Checks whether string value represents valid email.
        /// </summary>
        /// <param name="email">The email string.</param>
        /// <returns>True if email is valid, false - otherwise</returns>
        bool IsValidEmail(string email);

        /// <summary>
        ///     Checks whether string value represents valid name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if name is valid, false - otherwise</returns>
        bool IsValidName(string name);
    }
}