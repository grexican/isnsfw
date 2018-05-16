using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsNsfw.Command;
using IsNsfw.Repository.Interface;
using Moq;
using NUnit.Framework;

namespace IsNsfw.Tests
{
    public class CreateLinkCommandValidatorTests
    {
        [Test]
        public void ErrorIfNullUrl()
        {
            var sut     = new CreateLinkCommandValidator(null);
            var results = sut.Validate(new CreateLinkCommand() { });

            Assert.IsFalse(results.IsValid);
            Assert.IsTrue(results.Errors.Any(m => m.PropertyName == nameof(CreateLinkCommand.Url)));
        }

        [Test]
        public void ErrorIfNullKey()
        {
            var sut     = new CreateLinkCommandValidator(null);
            var results = sut.Validate(new CreateLinkCommand() { });

            Assert.IsFalse(results.IsValid);
            Assert.IsTrue(results.Errors.Any(m => m.PropertyName == nameof(CreateLinkCommand.Key)));
        }

        [Test]
        public void ErrorIfBlankUrl()
        {
            var sut     = new CreateLinkCommandValidator(null);
            var results = sut.Validate(new CreateLinkCommand() { Url = "" });

            Assert.IsFalse(results.IsValid);
            Assert.IsTrue(results.Errors.Any(m => m.PropertyName == nameof(CreateLinkCommand.Url)));
        }

        [Test]
        public void ErrorIfBlankKey()
        {
            var sut     = new CreateLinkCommandValidator(null);
            var results = sut.Validate(new CreateLinkCommand() { Key = "" });

            Assert.IsFalse(results.IsValid);
            Assert.IsTrue(results.Errors.Any(m => m.PropertyName == nameof(CreateLinkCommand.Key)));
        }

        [Test]
        public void RequiresValidUrl()
        {
            var linkRepo = new Mock<ILinkRepository>();
            linkRepo.Setup(m => m.KeyExists(It.IsAny<string>())).Returns(false);

            var sut     = new CreateLinkCommandValidator(linkRepo.Object);
            var results = sut.Validate(new CreateLinkCommand() { Key = "Test", Url = "bad url" });

            Assert.IsFalse(results.IsValid);
            Assert.IsTrue(results.Errors.Any(m => m.PropertyName == nameof(CreateLinkCommand.Url)));
        }

        [Test]
        public void RequiresUniqueKey()
        {
            var linkRepo = new Mock<ILinkRepository>();
            linkRepo.Setup(m => m.KeyExists(It.IsAny<string>())).Returns(true);

            var sut     = new CreateLinkCommandValidator(linkRepo.Object);
            var results = sut.Validate(new CreateLinkCommand() { Url = "http://www.google.com", Key = "werd" });

            Assert.IsFalse(results.IsValid);
            Assert.IsTrue(results.Errors.Any(m => m.PropertyName == nameof(CreateLinkCommand.Key)));
        }
    }
}
