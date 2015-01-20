using AutoMapper;
using JobScheduler.Common;
using JobScheduler.Contracts;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;

namespace JobScheduler.Mappers
{
    public class JobDetailToJobDetailModelMapper : MapperBase<JobDetailImpl, JobDetailModel>
    {
        public override void CreateMap(IMappingExpression<JobDetailImpl, JobDetailModel> mappingAction)
        {
            mappingAction.ForMember(t => t.JobType, e => e.MapFrom(d => d.JobType.ToString()));
        }
    }

    public interface IMapper
    {
        void CreateMap(IConfiguration expression);
    }
    public interface IMapper<TSource, TDestination>: IMapper
    {
        void CreateMap(IMappingExpression<TSource, TDestination> mappingAction);
    }

    public abstract class MapperBase<TSource, TDestination> : IMapper<TSource, TDestination>
    {
        public abstract void CreateMap(IMappingExpression<TSource, TDestination> mappingAction);

        public void CreateMap(IConfiguration configuration)
        {
            CreateMap(configuration.CreateMap<TSource, TDestination>());
        }
    }

    public class TimeOfDayToTimeOfDayModel : MapperBase<TimeOfDay, TimeOfDayModel>
    {
        public override void CreateMap(IMappingExpression<TimeOfDay, TimeOfDayModel> mappingAction)
        {
        }
    }

    public class TriggerToTriggerModelMapper: IMapper
    {

        public void CreateMap(IConfiguration expression)
        {
            expression.CreateMap<CronTriggerImpl, CronTriggerModel>()
                .ForMember(model => model.ExpressionSummary, ex => ex.MapFrom(impl => impl.GetExpressionSummary()));
            expression.CreateMap<SimpleTriggerImpl, SimpleTriggerModel>()
                .Include<DefaultTrigger, SimpleTriggerModel>()
                ;  
            expression.CreateMap<ITrigger, TriggerModel>()
                .Include<DefaultTrigger, SimpleTriggerModel>()
                .Include<SimpleTriggerImpl, SimpleTriggerModel>()
                .Include<DailyTimeIntervalTriggerImpl, DailyTimeIntervalTriggerModel>()
                .Include<CalendarIntervalTriggerImpl, CalendarIntervalTriggerModel>()
                .Include<CronTriggerImpl, CronTriggerModel>()
                .ForMember(t => t.NextFireTimeUtc, e => e.MapFrom(d => d.GetNextFireTimeUtc()))
                .ForMember(t => t.PreviousFireTimeUtc, e => e.MapFrom(d => d.GetPreviousFireTimeUtc()))
                ;                
        }
    }

    public class MappingProcessor : IMappingProcessor
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper[] _mapper;

        public MappingProcessor(IConfiguration configuration, IMapper[] mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public void Process()
        {
            foreach (var mapper in _mapper)
            {
                mapper.CreateMap(_configuration);
            }
        }
    }

    public interface IMappingProcessor
    {
        void Process();
    }
}