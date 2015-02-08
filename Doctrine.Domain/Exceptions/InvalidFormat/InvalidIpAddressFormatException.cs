namespace Doctrine.Domain.Exceptions.InvalidFormat
{
    using System;

    public class InvalidIpAddressFormatException : Exception
    {
        #region Constructors

        public InvalidIpAddressFormatException()
        {
        }

        public InvalidIpAddressFormatException(string message)
        : base(message)
        {
        }

        public InvalidIpAddressFormatException(string message, Exception inner)
        : base(message, inner)
        {
        }

        #endregion
    }
}