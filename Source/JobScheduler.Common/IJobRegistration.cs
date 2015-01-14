using Quartz;

namespace JobScheduler.Common
{
    public interface IJobRegistration
    {
        IJobDetail CreateJobDetail(JobBuilder builder);
        ITrigger CreateTrigger(TriggerBuilder builder);
    }
}