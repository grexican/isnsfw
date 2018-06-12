using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using IsNsfw.Model;
using IsNsfw.Repository;
using IsNsfw.Repository.Interface;
using IsNsfw.Service;
using IsNsfw.ServiceInterface;
using IsNsfw.ServiceInterface.EventHandlers;
using IsNsfw.ServiceModel;
using Moq;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.Host;
using ServiceStack.Logging;
using ServiceStack.Messaging;
using ServiceStack.OrmLite;
using ServiceStack.Testing;

namespace IsNsfw.Tests
{
    public class CreateLinkEventRequestTests
    {
        const string SessionId = "12345";

        private readonly ILinkRepository _linkRepo;
        private readonly ITagRepository _tagRepo;
        private readonly IEventBus _eventBus;
        private readonly IDbConnection _db;
        private readonly IDbConnectionFactory _dbFactory;
        private TestAppHost _appHost;
        private Link _link;

        public CreateLinkEventRequestTests()
        {
            LogManager.LogFactory = new DebugLogFactory(debugEnabled:true);

            _appHost = new TestAppHost();
            _appHost.Init();

            _dbFactory = _appHost.Container.Resolve<IDbConnectionFactory>();

            _db = _dbFactory.Open();

            _db.CreateTableIfNotExists<User>();
            _db.CreateTableIfNotExists<Link>();
            _db.CreateTableIfNotExists<Tag>();
            _db.CreateTableIfNotExists<LinkTag>();
            _db.CreateTableIfNotExists<LinkEvent>();

            _linkRepo = _appHost.Resolve<ILinkRepository>();
            _tagRepo  = _appHost.Resolve<ITagRepository>();
            _eventBus = _appHost.Resolve<IEventBus>();
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
            _db.DeleteAll<LinkTag>();
            _db.DeleteAll<Tag>();
            _db.DeleteAll<LinkEvent>();
            _db.DeleteAll<Link>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _db.DeleteAll<User>();
            _db.DeleteAll<LinkTag>();
            _db.DeleteAll<Tag>();
            _db.DeleteAll<LinkEvent>();
            _db.DeleteAll<Link>();
            _db.Dispose();

            _appHost.Dispose();
        }

        public LinkService GetService(string sessionId = SessionId)
        {
            var msgFactory = new Mock<IMessageFactory>();

            var ret = new LinkService(_linkRepo, _tagRepo, _eventBus);

            ret.Request = new BasicHttpRequest()
            {
                Items =
                {
                    [Keywords.Session] = new AuthUserSession() { Id = sessionId }
                }
            };
            
            return ret;
        }

        [Test]
        public void CreateLinkEventRequest_View_CreatesRecord()
        {
            var sut = GetService();

            var req = new CreateLinkEventRequest()
            {
                Key = _link.Key,
                LinkEventType = LinkEventType.View
            };

            sut.Post(req);

            Assert.IsTrue(_db.Exists<LinkEvent>(m => m.SessionId == SessionId && m.LinkEventType == req.LinkEventType));
        }

        [Test]
        public void CreateLinkEventRequest_View_IncrementsTotal()
        {
            var sut = GetService();

            var req = new CreateLinkEventRequest()
            {
                Key = _link.Key,
                LinkEventType = LinkEventType.View
            };

            sut.Post(req);

            _link = _db.SingleById<Link>(_link.Id);

            Assert.AreEqual(1, _link.TotalViews);
        }

        [Test]
        public void CreateLinkEventRequest_Preview_CreatesRecord()
        {
            var sut = GetService();

            var req = new CreateLinkEventRequest()
            {
                Key = _link.Key,
                LinkEventType = LinkEventType.Preview
            };

            sut.Post(req);

            Assert.IsTrue(_db.Exists<LinkEvent>(m => m.SessionId == SessionId && m.LinkEventType == req.LinkEventType));
        }

        [Test]
        public void CreateLinkEventRequest_Preview_IncrementsTotal()
        {
            var sut = GetService();

            var req = new CreateLinkEventRequest()
            {
                Key = _link.Key,
                LinkEventType = LinkEventType.Preview
            };

            sut.Post(req);

            _link = _db.SingleById<Link>(_link.Id);

            Assert.AreEqual(1, _link.TotalPreviews);
        }

        [Test]
        public void CreateLinkEventRequest_ClickThrough_CreatesRecord()
        {
            var sut = GetService();

            var req = new CreateLinkEventRequest()
            {
                Key = _link.Key,
                LinkEventType = LinkEventType.ClickThrough
            };

            sut.Post(req);

            Assert.IsTrue(_db.Exists<LinkEvent>(m => m.SessionId == SessionId && m.LinkEventType == req.LinkEventType));
        }

        [Test]
        public void CreateLinkEventRequest_ClickThrough_IncrementsTotal()
        {
            var sut = GetService();

            var req = new CreateLinkEventRequest()
            {
                Key = _link.Key,
                LinkEventType = LinkEventType.ClickThrough
            };

            sut.Post(req);

            _link = _db.SingleById<Link>(_link.Id);

            Assert.AreEqual(1, _link.TotalClickThroughs);
        }

        [Test]
        public void CreateLinkEventRequest_TurnBack_CreatesRecord()
        {
            var sut = GetService();

            var req = new CreateLinkEventRequest()
            {
                Key = _link.Key,
                LinkEventType = LinkEventType.TurnBack
            };

            sut.Post(req);

            Assert.IsTrue(_db.Exists<LinkEvent>(m => m.SessionId == SessionId && m.LinkEventType == req.LinkEventType));
        }

        [Test]
        public void CreateLinkEventRequest_TurnBack_IncrementsTotal()
        {
            var sut = GetService();

            var req = new CreateLinkEventRequest()
            {
                Key = _link.Key,
                LinkEventType = LinkEventType.TurnBack
            };

            sut.Post(req);

            _link = _db.SingleById<Link>(_link.Id);

            Assert.AreEqual(1, _link.TotalTurnBacks);
        }
    }
}
