using Common.Logging;
using JobScheduler.Common;
using Microsoft.Practices.ServiceLocation;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace JobScheduler.Host
{
    public class Bootstrapper
    {
        public static SchedulerHost Initialize()
        {
            var container = new Container(cfg =>
                                          {
                                              cfg.Scan(scanner =>
                                                       {
                                                           scanner.AssembliesFromApplicationBaseDirectory();
                                                           scanner.WithDefaultConventions();
                                                           scanner.AddAllTypesOf<IJobRegistration>();
                                                       });
                                              cfg.For<IScheduler>()
                                                  .Use(StdSchedulerFactory.GetDefaultScheduler())
                                                  .SetLifecycleTo<SingletonLifecycle>();
                                              cfg.For<IDefaultTrigger>()
                                                  .Use<DefaultTrigger>()
                                                  .SetLifecycleTo<SingletonLifecycle>();
                                              cfg.For<IJobFactory>().Use<ServiceLocatorJobFactory>();
                                              cfg.For<ILog>().Use(context => LogManager.GetLogger(context.ParentType));
                                          });
            var serviceLocator = new StructureMapServiceLocator(container);
            var serviceLocatorProvider = new ServiceLocatorProvider(() => serviceLocator);
            ServiceLocator.SetLocatorProvider(serviceLocatorProvider);
            container.Configure(c =>
                                {
                                    c.For<ServiceLocatorProvider>().Use(serviceLocatorProvider);
                                    c.For<IServiceLocator>().Use(serviceLocator);
                                });
            return ServiceLocator.Current.GetInstance<SchedulerHost>();
        }
    }
}