namespace Doctrine.Domain.Tests.Utils
{
    using System;

    using NUnit.Framework;

    [TestFixture]
    public class GuardTests
    {
        [Test]
        public void IntMoreThanZero_ValueIsEqualToZero_ThrowsArgumentOutOfRangeExceptionWithInfo()
        {
            // Arrange
            int someValue = 0;
            string someValueName = "someValue";

            // Act and Assert
            ArgumentOutOfRangeException exception =
            Assert.Throws<ArgumentOutOfRangeException>(() => Domain.Utils.Guard.IntMoreThanZero(someValue, someValueName));

            Assert.AreEqual(someValueName, exception.ParamName);
        }

        [Test]
        public void IntMoreThanZero_ValueIsGreaterThanZero_DoNothing()
        {
            // Arrange
            int someValue = 1;
            string someValueName = "someValue";

            // Act
            Domain.Utils.Guard.IntMoreThanZero(someValue, someValueName);
        }

        [Test]
        public void IntMoreThanZero_ValueIsLessThanZero_ThrowsArgumentOutOfRangeExceptionWithInfo()
        {
            // Arrange
            int someValue = -1;
            string someValueName = "someValue";

            // Act and Assert
            ArgumentOutOfRangeException exception =
            Assert.Throws<ArgumentOutOfRangeException>(() => Domain.Utils.Guard.IntMoreThanZero(someValue, someValueName));

            Assert.AreEqual(someValueName, exception.ParamName);
        }

        [Test]
        public void IntMoreOrEqualToZero_ValueIsEqualToZero_DoNothing()
        {
            // Arrange
            int someValue = 0;
            string someValueName = "someValue";

            // Act
            Domain.Utils.Guard.IntMoreOrEqualToZero(someValue, someValueName);
        }

        [Test]
        public void IntMoreOrEqualToZero_ValueIsGreaterThanZero_DoNothing()
        {
            // Arrange
            int someValue = 1;
            string someValueName = "someValue";

            // Act
            Domain.Utils.Guard.IntMoreOrEqualToZero(someValue, someValueName);
        }

        [Test]
        public void IntMoreOrEqualToZero_ValueIsLessThanZero_ThrowsArgumentOutOfRangeExceptionWithInfo()
        {
            // Arrange
            int someValue = -1;
            string someValueName = "someValue";

            // Act and Assert
            ArgumentOutOfRangeException exception =
            Assert.Throws<ArgumentOutOfRangeException>(() => Domain.Utils.Guard.IntMoreOrEqualToZero(someValue, someValueName));

            Assert.AreEqual(someValueName, exception.ParamName);
        }

        [Test]
        public void NotNull_ArgumentIsNotNull_DoNothing()
        {
            // Arrange
            object someArgument = new object();
            string someArgumentName = "someArgument";

            // Act
            Domain.Utils.Guard.NotNull(someArgument, someArgumentName);
        }

        [Test]
        public void NotNull_ArgumentIsNull_ThrowsArgumentNullExceptionWithInfo()
        {
            // Arrange
            object someArgument = null;
            string someArgumentName = "someArgument";

            // Act and Assert
            ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(() => Domain.Utils.Guard.NotNull(someArgument, someArgumentName));

            Assert.AreEqual(someArgumentName, exception.ParamName);
        }

        [Test]
        public void NotNullOrEmpty_StringIsEmpty_ThrowsArgumentExceptionWithInfo()
        {
            // Arrange
            string someString = String.Empty;
            string someStringName = "someString";

            // Act and Assert
            ArgumentException exception =
            Assert.Throws<ArgumentException>(() => Domain.Utils.Guard.NotNullOrEmpty(someString, someStringName));

            Assert.AreEqual(someStringName, exception.ParamName);
        }

        [Test]
        public void NotNullOrEmpty_StringIsNotEmpty_DoNothing()
        {
            // Arrange
            string someString = "This is string.";
            string someStringName = "someString";

            // Act
            Domain.Utils.Guard.NotNullOrEmpty(someString, someStringName);
        }

        [Test]
        public void NotNullOrEmpty_StringIsNull_ThrowsArgumentNullExceptionWithInfo()
        {
            // Arrange
            string someString = null;
            string someStringName = "someString";

            // Act and Assert
            ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(() => Domain.Utils.Guard.NotNullOrEmpty(someString, someStringName));

            Assert.AreEqual(someStringName, exception.ParamName);
        }
    }
}