namespace Doctrine.Domain.Tests.Validation.Concrete
{
    using System;

    using Doctrine.Domain.Validation.Abstract;
    using Doctrine.Domain.Validation.Concrete;

    using NUnit.Framework;

    [TestFixture]
    public class UserValidationTests
    {
        private IUserValidation _target;

        [SetUp]
        public void Init()
        {
            this._target = new UserValidation();
        }

        [Test]
        public void IsValidEmail_EmailIsInvalid_ReturnsFalse()
        {
            // Arrange
            string[] invalidEmails =
            {
                "", "invalid_email", "invalid_email@", "invalid_email@somewhere",
                "invalid_email@somewhere.", "invalid email with spaces@somewhere.else"
            };

            foreach (string email in invalidEmails)
            {
                // Act
                bool result = this._target.IsValidEmail(email);

                // Assert
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void IsValidEmail_EmailIsNull_ThrowsArgumentNullException()
        {
            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => this._target.IsValidEmail(null));
        }

        [Test]
        public void IsValidEmail_EmailIsValid_ReturnsTrue()
        {
            // Arrange
            string[] validEmails =
            {
                "someone@somewhere.com", "someone@somewhere.co.uk", "someone+tag@somewhere.net",
                "futureTLD@somewhere.else"
            };

            foreach (string email in validEmails)
            {
                // Act
                bool result = this._target.IsValidEmail(email);

                // Assert
                Assert.IsTrue(result);
            }
        }
    }
}