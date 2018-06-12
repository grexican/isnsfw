using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using IsNsfw.Repository.Interface;
using IsNsfw.Service;
using IsNsfw.ServiceInterface;
using Moq;
using ServiceStack.Caching;
using ServiceStack.Data;
using ServiceStack.Messaging;
using ServiceStack.OrmLite;
using SimpleInjector;

namespace IsNsfw.Tests
{
    public class TestAppHost : AppHost
    {
        public const string ScreenshotUrl = "http://screen.shot/image.png";

        public override void InitializeCache()
        {
            var mcc = new MemoryCacheClient();

            SimpleContainer.RegisterInstance<MemoryCacheClient>(mcc);
            SimpleContainer.RegisterInstance<ICacheClient>(mcc);
        }

        public override void InitializeDbConnectionFactory()
        {
            SimpleContainer.RegisterInstance<IDbConnectionFactory>(new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider));
            OrmLiteConfig.DialectProvider.NamingStrategy = new OrmLiteNamingStrategyBase();
            OrmLiteConfig.BeforeExecFilter = cmd => Debug.WriteLine(cmd.GetDebugString());
        }

        public override void InitializeMessaging()
        {
            SimpleContainer.RegisterSingleton<IMessageService>(() => new InMemoryTransientMessageService());
        }

        public override void InitializeIntegrations()
        {
            var scGen = new Mock<IScreenshotGenerator>();
            scGen.Setup(m => m.Process(It.IsAny<string>())).Returns(() => ScreenshotUrl);

            SimpleContainer.RegisterSingleton<IScreenshotGenerator>(() => scGen.Object);
        }
    }
}
