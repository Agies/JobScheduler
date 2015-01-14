using Quartz;

namespace JobScheduler.Common
{
    public class JobRegistrationDefault<TJob>: IJobRegistration where TJob : IJob
    {
        private readonly string _identity;
        private readonly IDefaultTrigger _defaultTrigger;
        private readonly JobSettings _jobSettings;

        public JobRegistrationDefault(
            string identity, 
            IDefaultTrigger defaultTrigger,
            JobSettings jobSettings = new JobSettings())
        {
            _identity = identity;
            _defaultTrigger = defaultTrigger;
            _jobSettings = jobSettings;
        }

        public virtual IJobDetail CreateJobDetail(JobBuilder builder)
        {
            return builder
                .OfType<TJob>()
                .RequestRecovery(_jobSettings.RequestRecovery)
                .StoreDurably(_jobSettings.StoreDurably)
                .WithIdentity(_identity, _jobSettings.Group ?? "Default")
                .WithDescription(_jobSettings.Description ?? "Default Job Description")
                .Build();
        }

        public virtual ITrigger CreateTrigger(TriggerBuilder builder)
        {
            return _defaultTrigger;
        }
    }
}