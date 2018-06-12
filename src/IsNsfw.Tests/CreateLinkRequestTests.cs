using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using IsNsfw.Model;
using IsNsfw.Repository;
using IsNsfw.Repository.Interface;
using IsNsfw.ServiceInterface;
using IsNsfw.ServiceInterface.EventHandlers;
using IsNsfw.ServiceModel;
using IsNsfw.ServiceModel.Types;
using Moq;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.FluentValidation;
using ServiceStack.Host;
using ServiceStack.Logging;
using ServiceStack.Messaging;
using ServiceStack.OrmLite;
using ServiceStack.Testing;

namespace IsNsfw.Tests
{
    public class CreateLinkRequestTests
    {
        private readonly ILinkRepository _linkRepo;
        private readonly ITagRepository _tagRepo;
        private readonly IDbConnection _db;
        private readonly IDbConnectionFactory _dbFactory;
        private TestAppHost _appHost;
        private IEventBus _eventBus;
        private IMessageFactory _msgFactory;

        public CreateLinkRequestTests()
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

            _linkRepo = _appHost.Resolve<ILinkRepository>();
            _tagRepo = _appHost.Resolve<ITagRepository>();
            _eventBus = _appHost.Resolve<IEventBus>();
            _msgFactory = _appHost.Resolve<IMessageFactory>();
        }

        [TearDown]
        public void TearDown()
        {
            _db.DeleteAll<User>();
            _db.DeleteAll<LinkTag>();
            _db.DeleteAll<Tag>();
            _db.DeleteAll<Link>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _db.DeleteAll<User>();
            _db.DeleteAll<LinkTag>();
            _db.DeleteAll<Tag>();
            _db.DeleteAll<Link>();
            _db.Dispose();

            _appHost.Dispose();
        }

        public LinkService GetService(string sessionId = "12345")
        {
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
        public void CanCreateLink()
        {
            var sut = GetService();

            var req = new CreateLinkRequest()
            {
                Key = "Hello",
                Url = "http://www.google.com",
            };

            var res = (LinkResponse)sut.Post(req);

            Assert.AreNotEqual(0, res.Id);
        }

        [Test]
        public void CanCreateLinkWithoutKey()
        {
            var sut = GetService();

            var req = new CreateLinkRequest()
            {
                Url = "http://www.google.com",
            };

            var res = (LinkResponse)sut.Post(req);

            Assert.AreNotEqual(0, res.Id);
            Assert.IsNotNull(res.Key);
        }

        [Test]
        public void CanCreateLinkWithTags()
        {
            var sut = GetService();

            var tags = new []
            {
                new Tag() { Key = "T1" },
                new Tag() { Key = "T2" },
                new Tag() { Key = "T3" },
            };

            _db.SaveAll(tags);

            var req = new CreateLinkRequest()
            {
                Key       = "Hello",
                Url       = "http://www.google.com",
                Tags      = new HashSet<string>() { tags[0].Key, tags[1].Key }
            };

            var res = (LinkResponse)sut.Post(req);

            Assert.AreNotEqual(0, res.Id);
            Assert.AreEqual(2, res.Tags.Count);
            Assert.Contains(tags[0].Key, res.Tags.ToList());
            Assert.Contains(tags[1].Key, res.Tags.ToList());
        }

        [Test]
        public void LinkPersistedInDatabase()
        {
            var sut = GetService();

            var req = new CreateLinkRequest()
            {
                Key = "Hello",
                Url = "http://www.google.com",
            };

            var res = (LinkResponse)sut.Post(req);

            Assert.IsNotNull(_db.Single<Link>(m => m.Id == res.Id));
        }

        [Test]
        public void CreateLinkWithTagsPersistedInDatabase()
        {
            var sut = GetService();

            var tags = new []
            {
                new Tag() { Key = "T1" },
                new Tag() { Key = "T2" },
                new Tag() { Key = "T3" },
            };
            _db.SaveAll(tags);

            var req = new CreateLinkRequest()
            {
                Key       = "Hello",
                Url       = "http://www.google.com",
                Tags      = new HashSet<string>() { tags[0].Key, tags[1].Key }
            };

            var res = (LinkResponse)sut.Post(req);

            Assert.AreEqual(2, _db.Count<LinkTag>(m => m.LinkId == res.Id));
        }

        [Test]
        public void CreatedLinkContainsKey()
        {
            var sut = GetService();

            var req = new CreateLinkRequest()
            {
                Key = "Hello",
                Url = "http://www.google.com"
            };

            var res = (LinkResponse)sut.Post(req);

            Assert.AreEqual(req.Key, _db.Single<Link>(m => m.Id == res.Id).Key);
        }

        [Test]
        public void CreatedLinkContainsUrl()
        {
            var sut = GetService();

            var req = new CreateLinkRequest()
            {
                Key = "Hello",
                Url = "http://www.google.com"
            };

            var res = (LinkResponse)sut.Post(req);

            Assert.AreEqual(req.Url, _db.Single<Link>(m => m.Id == res.Id).Url);
        }

        [Test]
        public void CreateLinkAlsoGeneratesScreenshot()
        {
            var sut = GetService();

            var req = new CreateLinkRequest()
            {
                Key = "Hello",
                Url = "http://www.google.com",
            };

            var res = (LinkResponse)sut.Post(req);

            var link = _db.Single<Link>(m => m.Id == res.Id);

            Assert.IsNotNull(link);
            Assert.IsNotNull(link.ScreenshotUrl);
        }
    }
}
