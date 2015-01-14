using System.Net;
using Common.Logging;
using Common.Logging.Simple;
using JobScheduler;
using JobScheduler.Host;
using Microsoft.WindowsAzure.ServiceRuntime;
using LogLevel = Common.Logging.LogLevel;

namespace CloudHost
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly SchedulerHost _scheduler;

        public WorkerRole()
        {
            LogManager.Adapter = new TraceLoggerFactoryAdapter
            {
                Level = LogLevel.All
            };
            
            _scheduler = Bootstrapper.Initialize();
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;
            _scheduler.Start();
            return base.OnStart();
        }

        public override void OnStop()
        {
            _scheduler.Stop();
            base.OnStop();
        }
    }
}
