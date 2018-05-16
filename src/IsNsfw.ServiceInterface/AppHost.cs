using Funq;
using ServiceStack;

namespace IsNsfw.ServiceInterface
{
    public class AppHost : AppHostBase
    {
        public AppHost() : base("IsNSFW", typeof(MyServices).Assembly) { }

        // Configure your AppHost with the necessary configuration and dependencies your App needs
        public override void Configure(Container container)
        {
            base.SetConfig(new HostConfig
            {
                DebugMode = AppSettings.Get(nameof(HostConfig.DebugMode), false)
            });

            Plugins.Add(new TemplatePagesFeature());
        }
    }
}