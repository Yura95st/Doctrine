namespace Doctrine.Domain.Validation.Abstract
{
    public interface IVisitorValidation
    {
        /// <summary>
        ///     Checks whether string value represents valid ip address.
        /// </summary>
        /// <param name="ipAddress">The ip address string.</param>
        /// <returns>True if ip address is valid, false - otherwise</returns>
        bool IsValidIpAddress(string ipAddress);
    }
}