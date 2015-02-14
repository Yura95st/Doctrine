namespace Doctrine.Domain.Validation.Concrete
{
    using System.Linq;
    using System.Text.RegularExpressions;

    using Doctrine.Domain.Enums;
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

        public PasswordStrength GetPasswordStrength(string password)
        {
            Guard.NotNullOrEmpty(password, "password");

            int score = 0;

            if (password.Length >= 5)
            {
                if (password.Length >= 8)
                {
                    score++;
                }

                if (Regex.Match(password, @"\d+", RegexOptions.ECMAScript)
                .Success && Regex.Match(password, @"[^\d]+", RegexOptions.ECMAScript)
                .Success)
                {
                    score++;
                }

                if (Regex.Match(password, @"[a-z]", RegexOptions.ECMAScript)
                .Success && Regex.Match(password, @"[A-Z]", RegexOptions.ECMAScript)
                .Success)
                {
                    score++;
                }

                if (Regex.Match(password, @".[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]", RegexOptions.ECMAScript)
                .Success)
                {
                    score++;
                }
            }

            return (PasswordStrength)score;
        }

        #endregion
    }
}