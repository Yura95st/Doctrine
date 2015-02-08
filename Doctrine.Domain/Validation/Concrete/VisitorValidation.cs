namespace Doctrine.Domain.Validation.Concrete
{
    using System.Net;

    using Doctrine.Domain.Utils;
    using Doctrine.Domain.Validation.Abstract;

    public class VisitorValidation : IVisitorValidation
    {
        #region IVisitorValidation Members

        public bool IsValidIpAddress(string ipAddress)
        {
            Guard.NotNull(ipAddress, "ipAddress");

            IPAddress address;
            if (!IPAddress.TryParse(ipAddress, out address))
            {
                return false;
            }

            return ipAddress == address.ToString();
        }

        #endregion
    }
}