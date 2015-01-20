using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using AutoMapper;
using JobScheduler.Contracts;
using Quartz;
using Quartz.Impl.Matchers;

namespace JobScheduler
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class JobSchedulerService : IJobSchedulerService
    {
        private readonly IScheduler _scheduler;
        private readonly IMappingEngine _engine;

        public JobSchedulerService(IScheduler scheduler, IMappingEngine engine)
        {
            _scheduler = scheduler;
            _engine = engine;
        }

        public string Ping()
        {
            return "pong";
        }

        public SchedulerModel GetScheduler()
        {
            var model = new SchedulerModel();
            model.Id = _scheduler.SchedulerInstanceId;
            model.Name = _scheduler.SchedulerName;
            foreach (var jobDetail in GetJobs(GroupMatcher<JobKey>.AnyGroup()))
            {
                model.JobDetails.Add(jobDetail);
            }
            return model;
        }

        public JobDetailModel GetJob(string name, string group)
        {
            var jobDetail = GetJobModel(name, group);
            return jobDetail;
        }

        public MessageBase Stop()
        {
            _scheduler.PauseAll();
            return new MessageBase();
        }

        public MessageBase Shutdown()
        {
            _scheduler.Shutdown();
            return new MessageBase();
        }

        public MessageBase Start()
        {
            _scheduler.Start();
            return new MessageBase();
        }

        private JobDetailModel CreateJobDetail(IJobDetail jobDetail)
        {
            if (jobDetail == null) return null;
            var jobModel = _engine.Map<JobDetailModel>(jobDetail);
            var running = _scheduler.GetCurrentlyExecutingJobs();
            foreach (var context in running)
            {
                if (context.JobDetail.Equals(jobDetail))
                {
                    jobModel.Running = true;
                    jobModel.Recovering = true;
                    jobModel.InstanceId = context.FireInstanceId;
                    jobModel.FireTime = context.FireTimeUtc;
                }
            }
            foreach (var trigger in _scheduler.GetTriggersOfJob(jobDetail.Key))
            {
                var triggerModel = _engine.Map<TriggerModel>(trigger);
                jobModel.Triggers.Add(triggerModel);
            }
            return jobModel;
        }

        private JobDetailModel GetJobModel(string name, string group)
        {
            return CreateJobDetail(_scheduler.GetJobDetail(new JobKey(name ?? "Default", group ?? "Default")));
        }

        private IEnumerable<JobDetailModel> GetJobs(GroupMatcher<JobKey> match)
        {
            var jobsNames = _scheduler.GetJobKeys(match);
            return jobsNames.Select(jobsName => CreateJobDetail(_scheduler.GetJobDetail(jobsName)));
        }
    }
}