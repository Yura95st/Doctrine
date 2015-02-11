namespace Doctrine.Domain.Utils.SecuredPasswordHelper
{
    using Doctrine.Domain.Utils.SecuredPasswordHelper.Model;

    public interface ISecuredPasswordHelper
    {
        /// <summary>Checks whether the password and the securedPassword are equal.</summary>
        /// <param name="password">The password.</param>
        /// <param name="securedPassword">The secured password.</param>
        /// <returns>True if the password and the securedPassword are equal, otherwise - false.</returns>
        bool ArePasswordsEqual(string password, SecuredPassword securedPassword);

        /// <summary>Gets the secured password from the plain password.</summary>
        /// <param name="plainPassword">The plain password.</param>
        /// <returns>The secured password.</returns>
        SecuredPassword GetSecuredPassword(string plainPassword);
    }
}