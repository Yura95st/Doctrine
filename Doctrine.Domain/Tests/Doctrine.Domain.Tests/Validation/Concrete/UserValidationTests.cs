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
                "invalid_email@somewhere.", "invalid email with spaces@somewhere.else", " email@somewhere.else",
                "email@somewhere.else ", " email@somewhere.else "
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

        [Test]
        public void IsValidName_NameContainsOnlyWhiteSpaces_ReturnFalse()
        {
            // Arrange
            string name = "  ";

            // Act
            bool result = this._target.IsValidName(name);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValidName_NameIsEmpty_ThrowsArgumentException()
        {
            // Act and Assert
            Assert.Throws<ArgumentException>(() => this._target.IsValidName(""));
        }

        [Test]
        public void IsValidName_NameIsInvalid_ReturnFalse()
        {
            // Arrange
            string[] invalidNames =
            {
                "some_name", "some1234Name", "-someName", ".someName", "'someName",
                "someN@me", "some. name", "some - name", "some ' name", "some..name", "some.-name", "some--name",
                "some''name", "some  name"
            };

            foreach (string firstName in invalidNames)
            {
                // Act
                bool result = this._target.IsValidName(firstName);

                // Assert
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void IsValidName_NameIsNull_ThrowsArgumentNullException()
        {
            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => this._target.IsValidName(null));
        }

        [Test]
        public void IsValidName_NameIsValid_ReturnTrue()
        {
            // Arrange
            string[] validNames = { "Some Name", "some name", "SOME NAME", "some-name", "some.name", "some'name" };

            foreach (string name in validNames)
            {
                // Act
                bool result = this._target.IsValidName(name);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void IsValidName_NameStartsOrEndsWithWhiteSpace_ReturnFalse()
        {
            // Arrange
            string[] names = { " someName", "someName ", " someName " };

            foreach (string name in names)
            {
                // Act
                bool result = this._target.IsValidName(name);

                // Assert
                Assert.IsFalse(result);
            }
        }
    }
}