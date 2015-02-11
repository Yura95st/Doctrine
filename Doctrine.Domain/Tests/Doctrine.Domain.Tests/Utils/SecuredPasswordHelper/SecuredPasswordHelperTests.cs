namespace Doctrine.Domain.Tests.Utils.SecuredPasswordHelper
{
    using System;

    using Doctrine.Domain.Utils.SecuredPasswordHelper;
    using Doctrine.Domain.Utils.SecuredPasswordHelper.Model;

    using NUnit.Framework;

    [TestFixture]
    public class SecuredPasswordHelperTests
    {
        private ISecuredPasswordHelper _securedPasswordHelper;

        [Test]
        public void ArePasswordsEqual_PasswordIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            SecuredPassword securedPassword = new SecuredPassword("password_hash", "password_salt");

            // Act and Assert
            Assert.Throws<ArgumentException>(() => this._securedPasswordHelper.ArePasswordsEqual("", securedPassword));
        }

        [Test]
        public void ArePasswordsEqual_PasswordIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            SecuredPassword securedPassword = new SecuredPassword("password_hash", "password_salt");

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => this._securedPasswordHelper.ArePasswordsEqual(null, securedPassword));
        }

        [Test]
        public void ArePasswordsEqual_PasswordsAreDifferent_ReturnsFalse()
        {
            // Arrange
            string password = "password";
            SecuredPassword securedPassword = this._securedPasswordHelper.GetSecuredPassword("PASSWORD");

            // Act
            bool result = this._securedPasswordHelper.ArePasswordsEqual(password, securedPassword);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ArePasswordsEqual_PasswordsAreMatching_ReturnsTrue()
        {
            // Arrange
            string password = "password";
            SecuredPassword securedPassword = this._securedPasswordHelper.GetSecuredPassword(password);

            // Act
            bool result = this._securedPasswordHelper.ArePasswordsEqual(password, securedPassword);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ArePasswordsEqual_SecuredPasswordIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            string password = "password";

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => this._securedPasswordHelper.ArePasswordsEqual(password, null));
        }

        [Test]
        public void GetSecuredPassword_MultipleGetForSamePassword_EachTimeReturnsSecuredPasswordWithUniqueSaltAndHash()
        {
            // Arrange
            string password = "password";

            // Act
            SecuredPassword securedPasswordOne = this._securedPasswordHelper.GetSecuredPassword(password);
            SecuredPassword securedPasswordTwo = this._securedPasswordHelper.GetSecuredPassword(password);

            // Assert
            Assert.IsNotNull(securedPasswordOne);
            Assert.IsNotNull(securedPasswordTwo);

            Assert.AreNotEqual(securedPasswordOne.Hash, securedPasswordTwo.Hash);
            Assert.AreNotEqual(securedPasswordOne.Salt, securedPasswordTwo.Salt);
        }

        [Test]
        public void GetSecuredPassword_PasswordIsEmpty_ThrowsArgumentException()
        {
            // Act and Assert
            Assert.Throws<ArgumentException>(() => this._securedPasswordHelper.GetSecuredPassword(""));
        }

        [Test]
        public void GetSecuredPassword_PasswordIsNull_ThrowsArgumentNullException()
        {
            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => this._securedPasswordHelper.GetSecuredPassword(null));
        }

        [Test]
        public void GetSecuredPassword_PasswordIsValid_ReturnsSecuredPassword()
        {
            // Arrange
            string password = "password";

            // Act
            SecuredPassword securedPassword = this._securedPasswordHelper.GetSecuredPassword(password);

            // Assert
            Assert.IsNotNull(securedPassword);
            Assert.AreNotEqual(password, securedPassword.Hash);
            Assert.AreEqual(32, SecuredPasswordHelper.SaltSize);
            Assert.AreEqual(4 * (int)Math.Ceiling(SecuredPasswordHelper.SaltSize / 3.0), securedPassword.Hash.Length);
        }

        [SetUp]
        public void Init()
        {
            this._securedPasswordHelper = new SecuredPasswordHelper();
        }
    }
}