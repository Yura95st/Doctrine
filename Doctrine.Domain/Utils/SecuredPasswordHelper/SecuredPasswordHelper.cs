namespace Doctrine.Domain.Utils.SecuredPasswordHelper
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;

    using Doctrine.Domain.Utils.SecuredPasswordHelper.Model;

    public class SecuredPasswordHelper : ISecuredPasswordHelper
    {
        public static readonly int SaltSize = 32;

        #region ISecuredPasswordHelper Members

        public bool ArePasswordsEqual(string password, SecuredPassword securedPassword)
        {
            Guard.NotNullOrEmpty(password, "password");
            Guard.NotNull(securedPassword, "securedPassword");

            using (
            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, Convert.FromBase64String(securedPassword.Salt))
            )
            {
                byte[] newKey = deriveBytes.GetBytes(SecuredPasswordHelper.SaltSize);

                return newKey.SequenceEqual(Convert.FromBase64String(securedPassword.Hash));
            }
        }

        public SecuredPassword GetSecuredPassword(string plainPassword)
        {
            Guard.NotNullOrEmpty(plainPassword, "plainPassword");

            using (Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(plainPassword, SecuredPasswordHelper.SaltSize))
            {
                string salt = Convert.ToBase64String(deriveBytes.Salt);
                string hash = Convert.ToBase64String(deriveBytes.GetBytes(SecuredPasswordHelper.SaltSize));

                return new SecuredPassword(hash, salt);
            }
        }

        #endregion
    }
}