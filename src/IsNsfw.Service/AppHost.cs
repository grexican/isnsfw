using IsNsfw.Repository;
using IsNsfw.Repository.Interface;
using ServiceStack;
using SimpleInjector;

namespace IsNsfw.ServiceInterface
{
    public class AppHost : AppHostBase
    {
        public AppHost() : base("IsNSFW", typeof(MyServices).Assembly) { }

        // Configure your AppHost with the necessary configuration and dependencies your App needs
        public override void Configure(Funq.Container container)
        {
            base.SetConfig(new HostConfig
            {
                DebugMode = AppSettings.Get(nameof(HostConfig.DebugMode), false)
            });

            Plugins.Add(new TemplatePagesFeature());

            var simpleContainer = new Container();
            container.Adapter = new SimpleInjectorIocAdapter (simpleContainer);

            simpleContainer.Register(typeof(IRepository<,>), typeof(IRepository<,>).Assembly, typeof(LinkRepository).Assembly);
            //simpleContainer.RegisterDecorator(...); // https://cuttingedge.it/blogs/steven/pivot/entry.php?id=93
        }
    }
}