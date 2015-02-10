namespace Doctrine.Domain.Validation.Concrete
{
    using System.Text.RegularExpressions;

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

        public bool IsValidFirstName(string firstName)
        {
            throw new System.NotImplementedException();
        }

        public bool IsValidLastName(string lastName)
        {
            throw new System.NotImplementedException();
        }

        public bool IsValidPassword(string password)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}