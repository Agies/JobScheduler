using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace JobScheduler.Contracts
{
    [ServiceContract]
    public interface IJobSchedulerService
    {
        [OperationContract]
        [WebGet]
        string Ping();

        [OperationContract]
        [WebGet]
        SchedulerModel GetScheduler();
        
        [OperationContract]
        [WebGet]
        JobDetailModel GetJob(string name, string group);

        [OperationContract]
        [WebGet]
        MessageBase Stop();

        [OperationContract]
        [WebGet]
        MessageBase Start();
    }

    public class MessageBase
    {
    }

    [DataContract]
    public class SchedulerModel
    {
        public SchedulerModel()
        {
            JobDetails = new List<JobDetailModel>();
        }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public List<JobDetailModel> JobDetails { get; set; }
    }

    [DataContract]
    public class JobDetailModel
    {
        public JobDetailModel()
        {
            Triggers = new List<TriggerModel>();
        }
        [DataMember]
        public string JobType { get; set; }
        [DataMember]
        public List<TriggerModel> Triggers { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Group { get; set; }
        [DataMember]
        public string FullName { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public bool Durable { get; set; }
        [DataMember]
        public bool PersistJobDataAfterExecution { get; set; }
        [DataMember]
        public bool RequestsRecovery { get; set; }
        [DataMember]
        public bool Running { get; set; }
        [DataMember]
        public bool Recovering { get; set; }
        [DataMember]
        public string InstanceId { get; set; }
        [DataMember]
        public DateTimeOffset? FireTime { get; set; }
    }

    [DataContract]
    [KnownType(typeof(SimpleTriggerModel))]
    [KnownType(typeof(DailyTimeIntervalTriggerModel))]
    [KnownType(typeof(CalendarIntervalTriggerModel))]
    [KnownType(typeof(CronTriggerModel))]
    public class TriggerModel
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Group { get; set; }
        [DataMember]
        public DateTimeOffset? FinalFireTimeUtc { get; set; }
        [DataMember]
        public int MisfireInstruction { get; set; }
        [DataMember]
        public DateTimeOffset? EndTimeUtc { get; set; }
        [DataMember]
        public DateTimeOffset? StartTimeUtc { get; set; }
        [DataMember]
        public int Priority { get; set; }
        [DataMember]
        public bool HasMillisecondPrecision { get; set; }
        [DataMember]
        public DateTimeOffset? NextFireTimeUtc { get; set; }
        [DataMember]
        public DateTimeOffset? PreviousFireTimeUtc { get; set; }
        [DataMember]
        public int RepeatCount { get; set; }
        [DataMember]
        public int TimesTriggered { get; set; }
        [DataMember]
        public bool Complete { get; set; }
    }

    [DataContract]
    public class SimpleTriggerModel : TriggerModel
    {
        [DataMember]
        public TimeSpan RepeatInterval { get; set; }
    }

    [DataContract]
    public class CronTriggerModel : TriggerModel
    {
        [DataMember] 
        public string CronExpressionString { get; set; }
        [DataMember]
        public string ExpressionSummary { get; set; }
    }

    [DataContract]
    public class CalendarIntervalTriggerModel : TriggerModel
    {

    }
    
    [DataContract]
    public class DailyTimeIntervalTriggerModel : TriggerModel
    {
        [DataMember]
        public int RepeatInterval { get; set; }
        [DataMember]
        public List<DayOfWeek> DaysOfWeek { get; set; }
        [DataMember]
        public TimeOfDayModel StartTimeOfDay { get; set; }
        [DataMember]
        public TimeOfDayModel EndTimeOfDay { get; set; }
    }

    [DataContract]
    public class TimeOfDayModel
    {
        [DataMember]
        public int Hour { get; set; }
        [DataMember]
        public int Minute { get; set; }
        [DataMember]
        public int Second { get; set; }
    }
}