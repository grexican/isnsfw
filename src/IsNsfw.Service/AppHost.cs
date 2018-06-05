using System;
using System.Collections.Generic;
using IsNsfw.Model;
using IsNsfw.Repository;
using IsNsfw.Repository.Interface;
using IsNsfw.ServiceInterface;
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
        public AppHost() : base("IsNSFW", typeof(MyServices).Assembly) { }

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

            var simpleContainer = new Container();
            //simpleContainer.Options.DefaultScopedLifestyle = ScopedLifestyle.Flowing;
            
            container.Adapter = new SimpleInjectorIocAdapter(simpleContainer);

            string redisServer = AppSettings.Get<string>("Redis.Server", (string)null);
            simpleContainer.Register<MemoryCacheClient>(() => new MemoryCacheClient(), Lifestyle.Transient);

            if (redisServer != null)
            {
                simpleContainer.Register<IRedisClientsManager>(() => new PooledRedisClientManager(new[] { redisServer }, new[] { redisServer }, AppSettings.Get("Redis.Instance", (long)1)), Lifestyle.Transient);
                simpleContainer.Register<ICacheClient>(() => container.Adapter.Resolve<IRedisClientsManager>().GetCacheClient(), Lifestyle.Transient);

                simpleContainer.Register<IMessageFactory >(() => new RedisMessageFactory(container.Adapter.Resolve<IRedisClientsManager>()), Lifestyle.Transient);
                simpleContainer.Register<IMessageProducer>(() => container.Adapter.Resolve<IMessageFactory>().CreateMessageProducer(), Lifestyle.Transient);
                simpleContainer.Register<IMessageQueueClient>(() => container.Adapter.Resolve<IMessageFactory>().CreateMessageQueueClient(), Lifestyle.Transient);
                simpleContainer.Register<RedisMqServer>(() => new RedisMqServer(container.Adapter.Resolve<IRedisClientsManager>(), 5), Lifestyle.Singleton);
            }
            else
            {
                throw new Exception("No Redis Server configured for environment."); // in prod, we need a real cache
            }

            simpleContainer.RegisterInstance<IDbConnectionFactory>(new OrmLiteConnectionFactory(AppSettings.GetString("ConnectionString"), PostgreSqlDialect.Provider));

            OrmLiteConfig.DialectProvider.NamingStrategy = new OrmLiteNamingStrategyBase();

            // repositories
            simpleContainer.Register<ILinkRepository, LinkRepository>();
            simpleContainer.Register<ITagRepository, TagRepository>();

            // validators
            simpleContainer.Register<ITagValidator, TagValidator>();
            simpleContainer.Collection.Register(typeof(IValidator<>), this.ServiceAssemblies);
            simpleContainer.Collection.Register(typeof(ICreateLinkScreenshotHandler), this.ServiceAssemblies);

            //simpleContainer.RegisterDecorator(...); // https://cuttingedge.it/blogs/steven/pivot/entry.php?id=93

            // messaging
            simpleContainer.Register<ICreateLinkScreenshotHandler, ScreenshotService>();

            // done!
            simpleContainer.Verify(VerificationOption.VerifyOnly);

            // Messaging Service
            var mqHost = container.Adapter.Resolve<RedisMqServer>();

            mqHost.RegisterHandler<CreateLinkScreenshotRequest>(msg => container.Adapter.Resolve<ICreateLinkScreenshotHandler>().Handle(msg));

            mqHost.Start();
        }

        //public override RouteAttribute[] GetRouteAttributes(System.Type requestType)
        //{
        //    var routes = base.GetRouteAttributes(requestType);
        //    routes.Each(x => x.Path = "/api" + x.Path);
        //    return routes;
        //}
    }
}