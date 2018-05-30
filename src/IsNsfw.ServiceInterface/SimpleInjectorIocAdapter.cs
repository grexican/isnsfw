using ServiceStack.Configuration;
using SimpleInjector;

namespace IsNsfw.ServiceInterface
{
    public class SimpleInjectorIocAdapter : IContainerAdapter
    {
        private readonly Container _container;

        public SimpleInjectorIocAdapter(Container container)
        {
            this._container = container;
        }

        public T Resolve<T>() 
        {
            return (T)this._container.GetInstance(typeof(T));
        }

        public T TryResolve<T>()
        {
            var registration = this._container.GetRegistration(typeof(T));
            return (T) registration?.GetInstance();
        }
    }
}
