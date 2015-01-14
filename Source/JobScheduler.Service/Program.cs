using Common.Logging;
using Common.Logging.Simple;
using JobScheduler.Host;
using Topshelf;

namespace JobScheduler.Service
{
    class Program
    {
        static void Main()
        {
            LogManager.Adapter = new ConsoleOutLoggerFactoryAdapter
            {
                Level = LogLevel.All
            };
            RunService();
        }

        private static void RunService()
        {
            HostFactory.Run(
                x =>
                {
                    x.Service<SchedulerHost>(
                        s =>
                        {
                            s.ConstructUsing(name =>
                                             {
                                                 var t = Bootstrapper.Initialize();
                                                 t.Initialize();
                                                 return t;
                                             });
                            s.WhenStarted(t => t.Start());
                            s.WhenStopped(t => t.Stop());
                            s.WhenShutdown(t => t.Shutdown());
                        });
                    //TODO: Make Run as configurable
                    x.RunAsNetworkService();
                    x.EnableShutdown();
                    x.SetDescription("QSI Job Scheduler");
                    x.SetDisplayName("QSI Job Scheduler");
                    x.SetServiceName("QSIJS");

                    x.EnableServiceRecovery(
                        rc => { rc.RestartService(1); });
                });
        }
    }
}
