using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using IsNsfw.Command;
using IsNsfw.Command.Domains;
using IsNsfw.Command.Interface;
using IsNsfw.Model;
using IsNsfw.Repository;
using IsNsfw.Repository.Interface;
using IsNsfw.ServiceInterface;
using Moq;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.FluentValidation;
using ServiceStack.OrmLite;
using ServiceStack.Testing;

namespace IsNsfw.Tests
{
    public class CreateLinkCommandTests
    {
        private readonly LinkRepository _linkRepo;
        private readonly LinkCommandHandlers _linkDomain;
        private readonly IDbConnection _db;
        private readonly OrmLiteConnectionFactory _dbFactory;

        public CreateLinkCommandTests()
        {
            _dbFactory = new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider);

            _db = _dbFactory.Open();

            _db.CreateTableIfNotExists<User>();
            _db.CreateTableIfNotExists<Link>();

            _linkRepo = new LinkRepository(_dbFactory);
            _linkDomain = new LinkCommandHandlers(_dbFactory);
        }

        [TearDown]
        public void TearDown()
        {
            _db.DeleteAll<User>();
            _db.DeleteAll<Link>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _db.DeleteAll<User>();
            _db.DeleteAll<Link>();
            _db.Dispose();
        }

        public ICommandHandler<CreateLinkCommand> GetCommandHandler()
        {
            return new ValidationCommandHandlerDecorator<CreateLinkCommand>(_linkDomain, new CreateLinkCommandValidator(_linkRepo));
        }

        [Test]
        public void CanCreateLink()
        {
            var sut = GetCommandHandler();

            var req = new CreateLinkCommand()
            {
                Key = "Hello",
                Url = "http://www.google.com",
                SessionId = "test",
            };

            sut.Handle(req);

            Assert.AreNotEqual(0, req.Id);
        }

        [Test]
        public void LinkPersistedInDatabase()
        {
            var sut = GetCommandHandler();

            var req = new CreateLinkCommand()
            {
                Key = "Hello",
                Url = "http://www.google.com",
                SessionId = "test",
            };

            sut.Handle(req);

            Assert.IsNotNull(_db.Single<Link>(m => m.Id == req.Id));
        }

        [Test]
        public void CreatedLinkContainsKey()
        {
            var sut = GetCommandHandler();

            var req = new CreateLinkCommand()
            {
                Key = "Hello",
                Url = "http://www.google.com",
                SessionId = "test",
            };

            sut.Handle(req);

            Assert.AreEqual(req.Key, _db.Single<Link>(m => m.Id == req.Id).Key);
        }

        [Test]
        public void CreatedLinkContainsUrl()
        {
            var sut = GetCommandHandler();

            var req = new CreateLinkCommand()
            {
                Key = "Hello",
                Url = "http://www.google.com",
                SessionId = "test",
            };

            sut.Handle(req);

            Assert.AreEqual(req.Url, _db.Single<Link>(m => m.Id == req.Id).Url);
        }

        [Test]
        public void CreateLinkThrowsIfDuplicateKey()
        {
            var sut = GetCommandHandler();
            
            _db.Insert(new Link() { Key = "Hello", Url = "http://www.test.com", SessionId = "test" });

            var req = new CreateLinkCommand()
            {
                Key = "Hello",
                Url = "http://www.google.com"
            };

            Assert.Throws<ValidationException>(() => sut.Handle(req));
        }
    }
}
