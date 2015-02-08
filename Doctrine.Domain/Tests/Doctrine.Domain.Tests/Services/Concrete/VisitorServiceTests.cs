﻿namespace Doctrine.Domain.Tests.Services.Concrete
{
    using System;

    using Doctrine.Domain.Dal;
    using Doctrine.Domain.Dal.Repositories.Abstract;
    using Doctrine.Domain.Exceptions;
    using Doctrine.Domain.Exceptions.InvalidFormat;
    using Doctrine.Domain.Models;
    using Doctrine.Domain.Services.Abstract;
    using Doctrine.Domain.Services.Concrete;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class VisitorServiceTests
    {
        [SetUp]
        public void Init()
        {
            //mockRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Visitor, bool>>>(), null, ""))
            //.Returns(
            //(Expression<Func<Visitor, bool>> predicate, Func<IQueryable<Visitor>, IOrderedQueryable<Visitor>> orderBy,
            // string includeProperties) => new[] { visitor }.Where(predicate.Compile()));
        }

        [Test]
        public void RegisterIpAddress_IpAddressFormatIsInvalid_ThrowsInvalidIpAddressFormatException()
        {
            IVisitorService target = new VisitorService(new Mock<IUnitOfWork>().Object);

            Assert.Throws<InvalidIpAddressFormatException>(() => target.RegisterIpAddress(""));

            Assert.Throws<InvalidIpAddressFormatException>(() => target.RegisterIpAddress("1.2.3.999"));

            Assert.Throws<InvalidIpAddressFormatException>(() => target.RegisterIpAddress("-1.2.3.4"));

            Assert.Throws<InvalidIpAddressFormatException>(() => target.RegisterIpAddress("invalid_ip_address"));
        }

        [Test]
        public void RegisterIpAddress_IpAddressIsAlreadyRegistered_GetsVisitorByIpAddress()
        {
            // Arrange
            Visitor visitor = new Visitor { VisitorId = 1, IpAddress = "127.0.0.1" };

            // Arrange - mock visitorRepository
            Mock<IVisitorRepository> visitorRepositoryMock = new Mock<IVisitorRepository>();
            visitorRepositoryMock.Setup(r => r.GetByIpAddress(visitor.IpAddress))
            .Returns(visitor);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(unitOfWork => unitOfWork.VisitorRepository)
            .Returns(visitorRepositoryMock.Object);

            // Act
            IVisitorService target = new VisitorService(unitOfWorkMock.Object);

            Visitor registeredVisitor = target.RegisterIpAddress(visitor.IpAddress);

            // Assert
            Assert.AreSame(visitor, registeredVisitor);

            visitorRepositoryMock.Verify(r => r.GetByIpAddress(visitor.IpAddress), Times.Once);
            visitorRepositoryMock.Verify(r => r.Insert(It.IsAny<Visitor>()), Times.Never);
            unitOfWorkMock.Verify(r => r.Save(), Times.Never);
        }

        [Test]
        public void RegisterIpAddress_IpAddressIsNotRegistered_RegistersIpAddressAndGetsVisitorByIpAddress()
        {
            // Arrange
            string ipAddress = "127.0.0.1";
            int newVisitorId = 1;

            // Arrange - mock visitorRepository
            Mock<IVisitorRepository> visitorRepositoryMock = new Mock<IVisitorRepository>();
            visitorRepositoryMock.Setup(r => r.GetByIpAddress(ipAddress))
            .Returns((Visitor)null);

            Visitor newVisitor = null;

            visitorRepositoryMock.Setup(r => r.Insert(It.IsAny<Visitor>()))
            .Callback((Visitor v) => newVisitor = v);

            // Arrange - mock unitOfWork
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(u => u.VisitorRepository)
            .Returns(visitorRepositoryMock.Object);

            unitOfWorkMock.Setup(u => u.Save())
            .Callback(() => newVisitor.VisitorId = newVisitorId);

            // Act
            IVisitorService target = new VisitorService(unitOfWorkMock.Object);

            Visitor registeredVisitor = target.RegisterIpAddress(ipAddress);

            // Assert
            Assert.IsNotNull(registeredVisitor);
            Assert.AreEqual(ipAddress, registeredVisitor.IpAddress);
            Assert.AreEqual(newVisitorId, registeredVisitor.VisitorId);

            visitorRepositoryMock.Verify(r => r.GetByIpAddress(ipAddress), Times.Once);
            visitorRepositoryMock.Verify(r => r.Insert(It.Is<Visitor>(v => v.IpAddress == ipAddress)), Times.Once);
            unitOfWorkMock.Verify(r => r.Save(), Times.Once);
        }

        [Test]
        public void RegisterIpAddress_IpAddressIsNull_ThrowsArgumentNullException()
        {
            IVisitorService target = new VisitorService(new Mock<IUnitOfWork>().Object);

            Assert.Throws<ArgumentNullException>(() => target.RegisterIpAddress(null));
        }
    }
}