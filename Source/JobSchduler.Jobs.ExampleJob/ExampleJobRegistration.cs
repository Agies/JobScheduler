using Common.Logging;
using JobScheduler.Common;
using Quartz;

namespace JobScheduler.Jobs.ExampleJob
{
    public class ExampleJobRegistration: JobRegistrationDefault<ExampleJob>
    {
        public ExampleJobRegistration(IDefaultTrigger defaultTrigger) : 
            base("Example Job", defaultTrigger)
        {
        }
    }

    public class ExampleJob: IJob
    {
        private readonly ILog _log;

        public ExampleJob(ILog log)
        {
            _log = log;
        }

        public void Execute(IJobExecutionContext context)
        {
            _log.Info("Example Job Ran!!");
        }
    }
}
