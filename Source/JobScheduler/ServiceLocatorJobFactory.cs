using Microsoft.Practices.ServiceLocation;
using Quartz;
using Quartz.Spi;

namespace JobScheduler
{
    public class ServiceLocatorJobFactory : IJobFactory
    {
        private readonly IServiceLocator _serviceLocator;

        public ServiceLocatorJobFactory(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceLocator.GetInstance(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
        }
    }
}