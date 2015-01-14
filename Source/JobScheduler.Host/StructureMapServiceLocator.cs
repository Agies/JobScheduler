using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using StructureMap;

namespace JobScheduler.Host
{
    public class StructureMapServiceLocator : ServiceLocatorImplBase
    {
        private readonly IContainer _container;

        public StructureMapServiceLocator(IContainer container)
        {
            _container = container;
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return Enumerable.Cast<object>(_container.GetAllInstances(serviceType));
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return serviceType.IsAbstract || serviceType.IsInterface
                    ? _container.TryGetInstance(serviceType)
                    : _container.GetInstance(serviceType);
            }

            return _container.GetInstance(serviceType, key);
        }
    }
}