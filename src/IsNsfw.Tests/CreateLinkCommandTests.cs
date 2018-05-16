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
        private readonly ServiceStackHost _appHost;
        private LinkRepository _linkRepo;
        private LinkCommandHandlers _linkDomain;
        private IDbConnection _db;

        public CreateLinkCommandTests()
        {
            _appHost = new BasicAppHost().Init();
            _appHost.Container.AddTransient<MyServices>();
            
            _appHost.Container.Register<IDbConnectionFactory>(c => new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider));

            _db = _appHost.Container.Resolve<IDbConnectionFactory>().Open();

            _db.CreateTableIfNotExists<User>();
            _db.CreateTableIfNotExists<Link>();

            _linkRepo = new LinkRepository(_appHost.Container.TryResolve<IDbConnectionFactory>());
            _linkDomain = new LinkCommandHandlers(_appHost.Container.TryResolve<IDbConnectionFactory>());
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
            _db.Dispose();
            _appHost.Dispose();
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
                Url = "http://www.google.com"
            };

            sut.Handle(req);

            Assert.AreNotEqual(0, req.Id);
        }

        [Test]
        public void CreateLinkThrowsIfDuplicateKey()
        {
            var sut = GetCommandHandler();
            
            _db.Insert(new Link() { Key = "Hello", Url = "http://www.test.com" });

            var req = new CreateLinkCommand()
            {
                Key = "Hello",
                Url = "http://www.google.com"
            };

            Assert.Throws<ValidationException>(() => sut.Handle(req));
        }
    }
}
