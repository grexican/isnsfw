using System;
using System.Collections.Generic;
using IsNsfw.Model;
using IsNsfw.Repository;
using IsNsfw.Repository.Interface;
using IsNsfw.ServiceInterface;
using IsNsfw.ServiceInterface.EventHandlers;
using IsNsfw.ServiceInterface.Validators;
using Microsoft.AspNetCore.Http;
using ServiceStack;
using ServiceStack.Caching;
using ServiceStack.Data;
using ServiceStack.FluentValidation;
using ServiceStack.Messaging;
using ServiceStack.Messaging.Redis;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
using ServiceStack.Validation;
using SimpleInjector;

namespace IsNsfw.Service
{
    public class AppHost : AppHostBase
    {
        public SimpleInjector.Container SimpleContainer { get; protected set; }

        public AppHost() : base("IsNSFW", typeof(LinkService).Assembly) { }

        // Configure your AppHost with the necessary configuration and dependencies your App needs
        public override void Configure(Funq.Container container)
        {
            base.SetConfig(new HostConfig
            {
                DebugMode = AppSettings.Get(nameof(HostConfig.DebugMode), false),
                HandlerFactoryPath = "api"
            });

            // plugins
            Plugins.Add(new TemplatePagesFeature());
            Plugins.Add(new ValidationFeature());

            SimpleContainer = new Container();
            //SimpleContainer.Options.DefaultScopedLifestyle = ScopedLifestyle.Flowing;
            
            container.Adapter = new SimpleInjectorIocAdapter(SimpleContainer);

            InitializeCache();
            InitializeDbConnectionFactory();
            InitializeMessaging();


            // repositories
            SimpleContainer.Register<ILinkRepository, LinkRepository>();
            SimpleContainer.Register<ITagRepository, TagRepository>();

            // validators
            SimpleContainer.Register<ITagValidator, TagValidator>();
            SimpleContainer.Collection.Register(typeof(IValidator<>), this.ServiceAssemblies);
            SimpleContainer.Collection.Register(typeof(ICreateLinkScreenshotHandler), this.ServiceAssemblies);

            //SimpleContainer.RegisterDecorator(...); // https://cuttingedge.it/blogs/steven/pivot/entry.php?id=93

            // events
            SimpleContainer.RegisterInstance<IEventBus>(new DomainEventBus(SimpleContainer));
            SimpleContainer.Collection.Register(typeof(IEventHandler<>), this.ServiceAssemblies);
            
            // messaging
            SimpleContainer.Register<ICreateLinkScreenshotHandler, ScreenshotService>();

            // Done container registration!
            SimpleContainer.Verify(VerificationOption.VerifyOnly);

            // Messaging Service
            var mqHost = Container.Adapter.Resolve<IMessageService>();

            mqHost.RegisterHandler<CreateLinkScreenshotRequest>(msg => Container.Adapter.Resolve<ICreateLinkScreenshotHandler>().Handle(msg));

            mqHost.Start();
        }

        public virtual void InitializeMessaging()
        {
            SimpleContainer.RegisterSingleton<IMessageService>(() => Container.Adapter.Resolve<RedisMqServer>());
        }

        public virtual void InitializeDbConnectionFactory()
        {
            SimpleContainer.RegisterInstance<IDbConnectionFactory>(new OrmLiteConnectionFactory(AppSettings.GetString("ConnectionString"), PostgreSqlDialect.Provider));

            OrmLiteConfig.DialectProvider.NamingStrategy = new OrmLiteNamingStrategyBase();
        }

        public virtual void InitializeCache()
        {
            string redisServer = AppSettings.Get<string>("Redis.Server", (string)null);
            SimpleContainer.Register<MemoryCacheClient>(() => new MemoryCacheClient(), Lifestyle.Transient);

            if (redisServer != null)
            {
                SimpleContainer.Register<IRedisClientsManager>(() => new PooledRedisClientManager(new[] { redisServer }, new[] { redisServer }, AppSettings.Get("Redis.Instance", (long)1)), Lifestyle.Transient);
                SimpleContainer.Register<ICacheClient>(() => Container.Adapter.Resolve<IRedisClientsManager>().GetCacheClient(), Lifestyle.Transient);

                SimpleContainer.Register<IMessageFactory >(() => new RedisMessageFactory(Container.Adapter.Resolve<IRedisClientsManager>()), Lifestyle.Transient);
                SimpleContainer.Register<IMessageProducer>(() => Container.Adapter.Resolve<IMessageFactory>().CreateMessageProducer(), Lifestyle.Transient);
                SimpleContainer.Register<IMessageQueueClient>(() => Container.Adapter.Resolve<IMessageFactory>().CreateMessageQueueClient(), Lifestyle.Transient);
                SimpleContainer.Register<RedisMqServer>(() => new RedisMqServer(Container.Adapter.Resolve<IRedisClientsManager>(), 5), Lifestyle.Singleton);
            }
            else
            {
                throw new Exception("No Redis Server configured for environment."); // in prod, we need a real cache
            }
        }

        //public override RouteAttribute[] GetRouteAttributes(System.Type requestType)
        //{
        //    var routes = base.GetRouteAttributes(requestType);
        //    routes.Each(x => x.Path = "/api" + x.Path);
        //    return routes;
        //}
    }
}