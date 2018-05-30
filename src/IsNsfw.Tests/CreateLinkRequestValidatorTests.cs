using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using IsNsfw.Model;
using IsNsfw.Repository;
using IsNsfw.Repository.Interface;
using IsNsfw.ServiceInterface.Validators;
using IsNsfw.ServiceModel;
using Moq;
using NUnit.Framework;
using ServiceStack.OrmLite;

namespace IsNsfw.Tests
{
    public class CreateLinkRequestValidatorTests
    {
        private readonly TagRepository _tagRepo;
        private readonly IDbConnection _db;
        private readonly OrmLiteConnectionFactory _dbFactory;

        public CreateLinkRequestValidatorTests()
        {
            _dbFactory = new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider);

            _db = _dbFactory.Open();

            _db.CreateTableIfNotExists<Tag>();

            _tagRepo  = new TagRepository(_dbFactory);
        }

        [TearDown]
        public void TearDown()
        {
            _db.DeleteAll<Tag>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _db.DeleteAll<Tag>();
            _db.Dispose();
        }

        [Test]
        public void ErrorIfNullUrl()
        {
            var sut     = new CreateLinkRequestValidator(null, new TagValidator(_tagRepo));
            var results = sut.Validate(new CreateLinkRequest() { });

            Assert.IsFalse(results.IsValid);
            Assert.IsTrue(results.Errors.Any(m => m.PropertyName == nameof(CreateLinkRequest.Url)));
        }

        [Test]
        public void ErrorIfNullKey()
        {
            var sut     = new CreateLinkRequestValidator(null, new TagValidator(_tagRepo));
            var results = sut.Validate(new CreateLinkRequest() { });

            Assert.IsFalse(results.IsValid);
            Assert.IsTrue(results.Errors.Any(m => m.PropertyName == nameof(CreateLinkRequest.Key)));
        }

        //[Test]
        //public void ErrorIfNullSessionId()
        //{
        //    var sut     = new CreateLinkRequestValidator(null, new TagValidator(_tagRepo));
        //    var results = sut.Validate(new CreateLinkRequest() { });

        //    Assert.IsFalse(results.IsValid);
        //    Assert.IsTrue(results.Errors.Any(m => m.PropertyName == nameof(CreateLinkRequest.SessionId)));
        //}

        [Test]
        public void ErrorIfBlankUrl()
        {
            var sut     = new CreateLinkRequestValidator(null, new TagValidator(_tagRepo));
            var results = sut.Validate(new CreateLinkRequest() { Url = " " });

            Assert.IsFalse(results.IsValid);
            Assert.IsTrue(results.Errors.Any(m => m.PropertyName == nameof(CreateLinkRequest.Url)));
        }

        [Test]
        public void ErrorIfBlankKey()
        {
            var sut     = new CreateLinkRequestValidator(null, new TagValidator(_tagRepo));
            var results = sut.Validate(new CreateLinkRequest() { Key = " " });

            Assert.IsFalse(results.IsValid);
            Assert.IsTrue(results.Errors.Any(m => m.PropertyName == nameof(CreateLinkRequest.Key)));
        }

        //[Test]
        //public void ErrorIfBlankSessionId()
        //{
        //    var sut     = new CreateLinkRequestValidator(null, new TagValidator(_tagRepo));
        //    var results = sut.Validate(new CreateLinkRequest() { SessionId = " " });

        //    Assert.IsFalse(results.IsValid);
        //    Assert.IsTrue(results.Errors.Any(m => m.PropertyName == nameof(CreateLinkRequest.SessionId)));
        //}

        [Test]
        public void RequiresValidUrl()
        {
            var linkRepo = new Mock<ILinkRepository>();
            linkRepo.Setup(m => m.KeyExists(It.IsAny<string>())).Returns(false);

            var sut     = new CreateLinkRequestValidator(linkRepo.Object, new TagValidator(_tagRepo));
            var results = sut.Validate(new CreateLinkRequest() { Key = "Test", Url = "bad url" });

            Assert.IsFalse(results.IsValid);
            Assert.IsTrue(results.Errors.Any(m => m.PropertyName == nameof(CreateLinkRequest.Url)));
        }

        [Test]
        public void RequiresAtLeastOneTag()
        {
            var linkRepo = new Mock<ILinkRepository>();
            linkRepo.Setup(m => m.KeyExists(It.IsAny<string>())).Returns(false);

            var sut = new CreateLinkRequestValidator(linkRepo.Object, new TagValidator(_tagRepo));
            var results = sut.Validate(new CreateLinkRequest() { Key = "Test", Url = "bad url", Tags = new HashSet<string>() { } });

            Assert.IsFalse(results.IsValid);
            Assert.IsTrue(results.Errors.Any(m => m.PropertyName == nameof(CreateLinkRequest.Tags)));
        }

        [Test]
        public void RequiresTagsThatExist()
        {
            var linkRepo = new Mock<ILinkRepository>();
            linkRepo.Setup(m => m.KeyExists(It.IsAny<string>())).Returns(false);

            var sut     = new CreateLinkRequestValidator(linkRepo.Object, new TagValidator(_tagRepo));
            var results = sut.Validate(new CreateLinkRequest() { Key = "Test", Url = "bad url", Tags = new HashSet<string>() { "T1" } });

            Assert.IsFalse(results.IsValid);
            Assert.IsTrue(results.Errors.Any(m => m.PropertyName.Contains(nameof(CreateLinkRequest.Tags))));
        }
        
        [Test]
        public void RequiresUniqueKey()
        {
            var linkRepo = new Mock<ILinkRepository>();
            linkRepo.Setup(m => m.KeyExists(It.IsAny<string>())).Returns(true);

            _db.Insert(new Tag() { Key = "T1", Name = "Tag 1" });

            var sut     = new CreateLinkRequestValidator(linkRepo.Object, new TagValidator(_tagRepo));
            var results = sut.Validate(new CreateLinkRequest() { Url = "http://www.google.com", Key = "werd", Tags = new HashSet<string>() { "T1" } });

            Assert.IsFalse(results.IsValid);
            Assert.IsTrue(results.Errors.Any(m => m.PropertyName == nameof(CreateLinkRequest.Key)));
        }
    }
}
