namespace Doctrine.Domain.Validation.Concrete
{
    using System.Linq;
    using System.Text.RegularExpressions;

    using Doctrine.Domain.Utils;
    using Doctrine.Domain.Validation.Abstract;

    public class UserValidation : IUserValidation
    {
        #region IUserValidation Members

        public bool IsValidEmail(string email)
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                             + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            return regex.IsMatch(email);
        }

        public bool IsValidName(string name)
        {
            Guard.NotNullOrEmpty(name, "firstName");

            char[] allowedChars = { '.', '-', '\'', ' ' };

            string[] nameParts = name.Split(allowedChars);

            foreach (string part in nameParts)
            {
                if (part.Length == 0 || part.Any(c => !char.IsLetter(c)))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsValidPassword(string password)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}