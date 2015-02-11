namespace Doctrine.Domain.Utils.SecuredPasswordHelper
{
    using Doctrine.Domain.Utils.SecuredPasswordHelper.Model;

    public interface ISecuredPasswordHelper
    {
        bool ArePasswordsEqual(string password, SecuredPassword securedPassword);

        SecuredPassword GetSecuredPassword(string plainPassword);
    }
}