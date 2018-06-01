using IsNsfw.Model;
using IsNsfw.Repository;
using IsNsfw.Repository.Interface;
using IsNsfw.ServiceInterface;
using IsNsfw.ServiceInterface.Validators;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.FluentValidation;
using ServiceStack.OrmLite;
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
            container.Adapter = new SimpleInjectorIocAdapter(simpleContainer);

            simpleContainer.RegisterInstance<IDbConnectionFactory>(new OrmLiteConnectionFactory(AppSettings.GetString("ConnectionString"), PostgreSqlDialect.Provider));

            // repositories
            simpleContainer.Register<ILinkRepository, LinkRepository>();
            simpleContainer.Register<ITagRepository, TagRepository>();

            // validators
            simpleContainer.Register<ITagValidator, TagValidator>();
            simpleContainer.Collection.Register(typeof(IValidator<>), this.ServiceAssemblies);

            // done!
            simpleContainer.Verify(VerificationOption.VerifyAndDiagnose);

            //simpleContainer.RegisterDecorator(...); // https://cuttingedge.it/blogs/steven/pivot/entry.php?id=93

            if(Config.DebugMode)
            {
                using(var db = simpleContainer.GetInstance<IDbConnectionFactory>().OpenDbConnection())
                {
                    using(var trans = db.OpenTransaction())
                    {
                        db.CreateTableIfNotExists<User>();
                        db.CreateTableIfNotExists<Link>();
                        db.CreateTableIfNotExists<Tag>();
                        db.CreateTableIfNotExists<LinkTag>();
                        db.CreateTableIfNotExists<LinkEvent>();

                        trans.Commit();
                    }
                }
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