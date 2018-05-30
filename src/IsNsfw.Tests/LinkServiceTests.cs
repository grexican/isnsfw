using System.Data;
using System.Diagnostics;
using Funq;
using IsNsfw.Model;
using IsNsfw.Repository;
using IsNsfw.Repository.Interface;
using ServiceStack;
using NUnit.Framework;
using IsNsfw.ServiceInterface;
using IsNsfw.ServiceInterface.Validators;
using IsNsfw.ServiceModel;
using ServiceStack.Data;
using ServiceStack.Host;
using ServiceStack.FluentValidation;
using ServiceStack.Logging;
using ServiceStack.OrmLite;
using ServiceStack.Testing;
using ServiceStack.Validation;

namespace IsNsfw.Tests
{
    public class LinkServiceTests
    {
        const string BaseUri = "http://localhost:2000/";
        private readonly AppHost _appHost;

        public LinkServiceTests()
        {
            _appHost = new AppHost();

            _appHost
                .Init()
                .Start(BaseUri);
        }

        class AppHost : AppSelfHostBase
        {
            public IDbConnection Db;
            private OrmLiteConnectionFactory _dbFactory;

            public AppHost() : base(nameof(LinkServiceTests), typeof(MyServices).Assembly) { }

            public override void Configure(Container container)
            {
                SetConfig(new HostConfig { DebugMode = true });

                Plugins.Add(new ValidationFeature());

                LogManager.LogFactory = new DebugLogFactory(debugEnabled:true);

                _dbFactory                     = new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider);
                OrmLiteConfig.BeforeExecFilter = cmd => Debug.WriteLine(cmd.GetDebugString());

                Db = _dbFactory.Open();

                Db.CreateTableIfNotExists<User>();
                Db.CreateTableIfNotExists<Link>();
                Db.CreateTableIfNotExists<Tag>();
                Db.CreateTableIfNotExists<LinkTag>();

                container.Register<IDbConnectionFactory>(_dbFactory);
                container.RegisterAs<LinkRepository, ILinkRepository>();
                container.RegisterAs<TagRepository, ITagRepository>();
                container.RegisterAs<TagValidator, ITagValidator>();
                container.RegisterValidators(typeof(CreateLinkRequestValidator).Assembly);
            }
        }


        [TearDown]
        public void TearDown()
        {
            _appHost.Db.DeleteAll<User>();
            _appHost.Db.DeleteAll<LinkTag>();
            _appHost.Db.DeleteAll<Tag>();
            _appHost.Db.DeleteAll<Link>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _appHost.Db.DeleteAll<User>();
            _appHost.Db.DeleteAll<LinkTag>();
            _appHost.Db.DeleteAll<Tag>();
            _appHost.Db.DeleteAll<Link>();
            _appHost.Db.Dispose();

            _appHost.Dispose();
        }

        public IServiceClient CreateClient() => new JsonServiceClient(BaseUri);

        [Test]
        public void CreateLinkThrowsIfDuplicateKey()
        {
            var client = CreateClient();
            
            _appHost.Db.Insert(new Link() { Key = "Hello", Url = "http://www.test.com", SessionId = "test" });

            var req = new CreateLinkRequest()
            {
                Key = "Hello",
                Url = "http://www.google.com"
            };

            Assert.Throws<WebServiceException>(() => client.Post(req));
        }

        [Test]
        public void CreateLinkThrowsIfBannedKey()
        {
            var client = CreateClient();
            
            var req = new CreateLinkRequest()
            {
                Key = KeyValidator.BannedTags[0],
                Url = "http://www.google.com"
            };

            Assert.Throws<WebServiceException>(() => client.Post(req));
        }
    }
}