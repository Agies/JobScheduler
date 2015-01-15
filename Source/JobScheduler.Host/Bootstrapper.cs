using System;
using System.Configuration;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using AutoMapper;
using Common.Logging;
using JobScheduler.Common;
using JobScheduler.Mappers;
using Microsoft.Practices.ServiceLocation;
using Microsoft.WindowsAzure.ServiceRuntime;
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
                                                           scanner.AddAllTypesOf<IMapper>();
                                                       });
                                              cfg.For<IScheduler>()
                                                  .Use(StdSchedulerFactory.GetDefaultScheduler())
                                                  .SetLifecycleTo<SingletonLifecycle>();
                                              cfg.For<IDefaultTrigger>()
                                                  .Use<DefaultTrigger>()
                                                  .SetLifecycleTo<SingletonLifecycle>();
                                              cfg.For<IJobFactory>().Use<ServiceLocatorJobFactory>();
                                              cfg.For<IMappingEngine>().Use(Mapper.Engine);
                                              cfg.For<IConfiguration>().Use(Mapper.Configuration);
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
            ServiceLocator.Current.GetInstance<MappingProcessor>().Process();
            return ServiceLocator.Current.GetInstance<SchedulerHost>();
        }

        public static ServiceHost StartRestService()
        {
            var service = ServiceLocator.Current.GetInstance<JobSchedulerService>();
            var logger = LogManager.GetLogger("Boostrapper");
            IPEndPoint endpoint = GetInstanceEndpoint("httpIn");
            var address = new Uri(string.Format("http://{0}/", endpoint));
            logger.Info("Service Endpoint: " + address);
            var host = new WebServiceHost(service, address);
            host.Open();
            logger.Info("Rest Service Started");
            return host;
        }

        private static IPEndPoint GetInstanceEndpoint(string endpointName)
        {
            if (RoleEnvironment.IsAvailable)
            {
                return RoleEnvironment.CurrentRoleInstance.InstanceEndpoints[endpointName].IPEndpoint;
            }
            return new IPEndPoint(IPAddress.Loopback, int.Parse(ConfigurationManager.AppSettings[endpointName]));
        }

    }
}