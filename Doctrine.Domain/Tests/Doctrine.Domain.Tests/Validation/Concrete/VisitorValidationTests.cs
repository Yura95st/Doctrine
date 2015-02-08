namespace Doctrine.Domain.Tests.Validation.Concrete
{
    using System;

    using Doctrine.Domain.Validation.Abstract;
    using Doctrine.Domain.Validation.Concrete;

    using NUnit.Framework;

    [TestFixture]
    public class VisitorValidationTests
    {
        private IVisitorValidation _target;

        [SetUp]
        public void Init()
        {
            this._target = new VisitorValidation();
        }

        [Test]
        public void IsValidIpAddress_IpAddressIsInvalid_ReturnsFalse()
        {
            // Arrange
            string[] invalidIpAddresses =
            {
                "999.999.999.999", "127.", "127.0.", "127.0.0.", "-1.2.3.4", "invalid_ip_address"
            };

            foreach (string ipAddress in invalidIpAddresses)
            {
                // Act
                bool result = this._target.IsValidIpAddress(ipAddress);

                // Assert
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void IsValidIpAddress_IpAddressIsNull_ThrowsArgumentNullException()
        {
            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => this._target.IsValidIpAddress(null));
        }

        [Test]
        public void IsValidIpAddress_IpAddressIsValid_ReturnsTrue()
        {
            // Arrange
            string[] validIpAddresses = { "255.255.255.255", "127.0.0.1", "192.168.1.1" };

            foreach (string ipAddress in validIpAddresses)
            {
                // Act
                bool result = this._target.IsValidIpAddress(ipAddress);

                // Assert
                Assert.IsTrue(result);
            }
        }
    }
}