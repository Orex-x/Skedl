using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Skedl.DataCatcher.Services.Quartz;

public class QuartzService
{
    private StdSchedulerFactory? _factory;
    private IScheduler? _scheduler;
    
    public QuartzService(IJobFactory jobFactory)
    {
        Build(jobFactory);
    }

    private async void Build(IJobFactory jobFactory)
    {
        _factory = new StdSchedulerFactory();
        _scheduler = await _factory.GetScheduler();
        _scheduler.JobFactory = jobFactory;
        await _scheduler.Start(); 
    }
    
    public async Task AddCatcherRepeat<T>(DateTimeOffset startTimeUtc, int withIntervalInHours) where T : IJob
    {
        if(_scheduler == null) return;
        
        IJobDetail job = JobBuilder.Create<T>()
            .Build();
        
        ITrigger trigger = TriggerBuilder.Create()
            .StartAt(startTimeUtc)
            .WithSimpleSchedule(x => x
                .WithIntervalInHours(withIntervalInHours)
                .RepeatForever())
            .Build();
        
        await _scheduler.ScheduleJob(job, trigger);
    }
}