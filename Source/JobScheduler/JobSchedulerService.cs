using System.Collections;
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
        private IScheduler _scheduler;
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

        public SchedulerModel Get()
        {
            var model = new SchedulerModel();
            model.Id = _scheduler.SchedulerInstanceId;
            model.Name = _scheduler.SchedulerName;
            foreach (var jobDetail in GetJobs())
            {
                var jobModel = _engine.Map<JobDetailModel>(jobDetail);
                model.JobDetails.Add(jobModel);
                foreach (var trigger in _scheduler.GetTriggersOfJob(jobDetail.Key))
                {
                    var triggerModel = _engine.Map<TriggerModel>(trigger);
                    jobModel.Triggers.Add(triggerModel);
                }
            }
            return model;
        }

        private IEnumerable<IJobDetail> GetJobs()
        {
            var jobsNames = _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
            return jobsNames.Select(jobsName => _scheduler.GetJobDetail(jobsName));
        }
    }
}