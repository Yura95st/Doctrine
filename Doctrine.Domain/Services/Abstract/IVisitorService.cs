namespace Doctrine.Domain.Services.Abstract
{
    using Doctrine.Domain.Models;

    public interface IVisitorService
    {
        /// <summary>Registers the ip address (if not registered yet) and returns registered visitor info.</summary>
        /// <param name="ipAddress">The ip address.</param>
        /// <returns>Registered visitor info</returns>
        Visitor RegisterIpAddress(string ipAddress);
    }
}