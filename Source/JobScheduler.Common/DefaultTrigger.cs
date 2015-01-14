using System;
using Quartz;
using Quartz.Impl.Triggers;

namespace JobScheduler.Common
{
    public sealed class DefaultTrigger: SimpleTriggerImpl, IDefaultTrigger
    {
        public DefaultTrigger()
        {
            Group = "Default";
            Description = "Default Trigger";
            Name = "Default Trigger";
            RepeatInterval = TimeSpan.FromHours(1);
            RepeatCount = RepeatIndefinitely;
            StartTimeUtc = DateTime.UtcNow;
        }
    }
}