using Common.Logging;
using JobScheduler.Common;
using Quartz;
using Quartz.Spi;

namespace JobScheduler
{
    public class SchedulerHost
    {
        private readonly IScheduler _instance;

        public SchedulerHost(IScheduler scheduler, IJobFactory jobFactory, IJobRegistration[] jobRegistrations, ILog log)
        {
            _instance = scheduler;
            _instance.JobFactory = jobFactory;
            foreach (var jobRegistration in jobRegistrations)
            {
                var detail = jobRegistration.CreateJobDetail(JobBuilder.Create());
                var trigger = jobRegistration.CreateTrigger(TriggerBuilder.Create());
                var result = _instance.ScheduleJob(detail, trigger);
                log.InfoFormat("{0} has been scheduled with {1}, starting {2}", 
                    detail.Key,
                    trigger.Key,
                    result);
            }
        }

        public void Start()
        {
            _instance.Start();
        }

        public void Stop()
        {
            _instance.Shutdown(true);
        }

        public void Initialize()
        {
            
        }

        public void Shutdown()
        {
            _instance.Shutdown(false);
        }

        
    }
}