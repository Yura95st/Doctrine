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
        ///     Checks whether string value represents valid name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if name is valid, false - otherwise</returns>
        bool IsValidName(string name);

        /// <summary>
        ///     Checks whether string value represents valid password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>True if password is valid, false - otherwise</returns>
        bool IsValidPassword(string password);
    }
}