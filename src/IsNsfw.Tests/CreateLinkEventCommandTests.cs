using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using IsNsfw.Command;
using IsNsfw.Command.Domains;
using IsNsfw.Command.Interface;
using IsNsfw.Model;
using IsNsfw.Repository;
using NUnit.Framework;
using ServiceStack.OrmLite;

namespace IsNsfw.Tests
{
    public class CreateLinkEventCommandTests
    {
        private readonly LinkRepository _linkRepo;
        private readonly LinkCommandHandlers _linkDomain;
        private readonly IDbConnection _db;
        private readonly OrmLiteConnectionFactory _dbFactory;
        private Link _link;

        public CreateLinkEventCommandTests()
        {
            _dbFactory = new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider);

            _db = _dbFactory.Open();

            _db.CreateTableIfNotExists<User>();
            _db.CreateTableIfNotExists<Link>();
            _db.CreateTableIfNotExists<LinkEvent>();

            _linkDomain = new LinkCommandHandlers(_dbFactory);
        }

        public ICommandHandler<CreateLinkEventCommand> GetCommandHandler()
        {
            return new ValidationCommandHandlerDecorator<CreateLinkEventCommand>(_linkDomain, new CreateLinkEventCommandValidator());
        }

        [SetUp]
        public void SetUp()
        {
            _link = new Link() { Key = "test", Url = "http://www.google.com", SessionId = "test", };
            _db.Save(_link);
        }

        [TearDown]
        public void TearDown()
        {
            _db.DeleteAll<User>();
            _db.DeleteAll<LinkEvent>();
            _db.DeleteAll<Link>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _db.DeleteAll<User>();
            _db.DeleteAll<LinkEvent>();
            _db.DeleteAll<Link>();
            _db.Dispose();
        }

        [Test]
        public void CreateLinkEventCommand_View_CreatesRecord()
        {
            var sut = GetCommandHandler();

            var req = new CreateLinkEventCommand()
            {
                LinkId = _link.Id,
                SessionId = "12345",
                LinkEventType = LinkEventType.View
            };

            sut.Handle(req);

            Assert.IsTrue(_db.Exists<LinkEvent>(m => m.SessionId == req.SessionId && m.LinkEventType == req.LinkEventType));
        }

        [Test]
        public void CreateLinkEventCommand_View_IncrementsTotal()
        {
            var sut = GetCommandHandler();

            var req = new CreateLinkEventCommand()
            {
                LinkId        = _link.Id,
                SessionId     = "12345",
                LinkEventType = LinkEventType.View
            };

            sut.Handle(req);

            _link = _db.SingleById<Link>(_link.Id);

            Assert.AreEqual(1, _link.TotalViews);
        }

        [Test]
        public void CreateLinkEventCommand_Preview_CreatesRecord()
        {
            var sut = GetCommandHandler();

            var req = new CreateLinkEventCommand()
            {
                LinkId        = _link.Id,
                SessionId     = "12345",
                LinkEventType = LinkEventType.Preview
            };

            sut.Handle(req);

            Assert.IsTrue(_db.Exists<LinkEvent>(m => m.SessionId == req.SessionId && m.LinkEventType == req.LinkEventType));
        }

        [Test]
        public void CreateLinkEventCommand_Preview_IncrementsTotal()
        {
            var sut = GetCommandHandler();

            var req = new CreateLinkEventCommand()
            {
                LinkId        = _link.Id,
                SessionId     = "12345",
                LinkEventType = LinkEventType.Preview
            };

            sut.Handle(req);

            _link = _db.SingleById<Link>(_link.Id);

            Assert.AreEqual(1, _link.TotalPreviews);
        }

        [Test]
        public void CreateLinkEventCommand_ClickThrough_CreatesRecord()
        {
            var sut = GetCommandHandler();

            var req = new CreateLinkEventCommand()
            {
                LinkId        = _link.Id,
                SessionId     = "12345",
                LinkEventType = LinkEventType.ClickThrough
            };

            sut.Handle(req);

            Assert.IsTrue(_db.Exists<LinkEvent>(m => m.SessionId == req.SessionId && m.LinkEventType == req.LinkEventType));
        }

        [Test]
        public void CreateLinkEventCommand_ClickThrough_IncrementsTotal()
        {
            var sut = GetCommandHandler();

            var req = new CreateLinkEventCommand()
            {
                LinkId        = _link.Id,
                SessionId     = "12345",
                LinkEventType = LinkEventType.ClickThrough
            };

            sut.Handle(req);

            _link = _db.SingleById<Link>(_link.Id);

            Assert.AreEqual(1, _link.TotalClickThroughs);
        }

        [Test]
        public void CreateLinkEventCommand_TurnBack_CreatesRecord()
        {
            var sut = GetCommandHandler();

            var req = new CreateLinkEventCommand()
            {
                LinkId        = _link.Id,
                SessionId     = "12345",
                LinkEventType = LinkEventType.TurnBack
            };

            sut.Handle(req);

            Assert.IsTrue(_db.Exists<LinkEvent>(m => m.SessionId == req.SessionId && m.LinkEventType == req.LinkEventType));
        }

        [Test]
        public void CreateLinkEventCommand_TurnBack_IncrementsTotal()
        {
            var sut = GetCommandHandler();

            var req = new CreateLinkEventCommand()
            {
                LinkId        = _link.Id,
                SessionId     = "12345",
                LinkEventType = LinkEventType.TurnBack
            };

            sut.Handle(req);

            _link = _db.SingleById<Link>(_link.Id);

            Assert.AreEqual(1, _link.TotalTurnBacks);
        }
    }
}
