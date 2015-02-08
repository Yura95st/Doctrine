namespace Doctrine.Domain.Validation.Abstract
{
    public interface IUserValidation
    {
        /// <summary>
        /// Checks whether string value represents valid email.
        /// </summary>
        /// <param name="email">The email string.</param>
        /// <returns>True if email is valid, false - otherwise</returns>
        bool IsValidEmail(string email);
    }
}