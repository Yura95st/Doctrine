namespace Doctrine.Domain.Utils.SecuredPasswordHelper.Model
{
    public class SecuredPassword
    {
        private readonly string _hash;

        private readonly string _salt;

        public SecuredPassword(string hash, string salt)
        {
            Guard.NotNullOrEmpty(hash, "hash");
            Guard.NotNullOrEmpty(salt, "salt");

            this._hash = hash;
            this._salt = salt;
        }

        public string Hash
        {
            get
            {
                return this._hash;
            }
        }

        public string Salt
        {
            get
            {
                return this._salt;
            }
        }
    }
}