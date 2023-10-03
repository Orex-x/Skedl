using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Skedl.DataCatcher.Services.Quartz;

public class QuartzService
{
    private StdSchedulerFactory? _factory;
    private IScheduler? _scheduler;

    private readonly IDictionary<string, IJobDetail> jobDictionary = new Dictionary<string, IJobDetail>();

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


    public async Task AddCatcherRepeatWithCron<T>(string cronExpression) where T : IJob
    {
        if (_scheduler == null) return;

        IJobDetail job = JobBuilder.Create<T>()
            .Build();

        jobDictionary.Add(typeof(T).Name, job);

        ITrigger trigger = TriggerBuilder.Create()
            .StartAt(SystemTime.UtcNow().AddDays(-1))
            .WithCronSchedule(cronExpression)
            .Build();

        await _scheduler.ScheduleJob(job, trigger);
    }

    public async Task AddCatcherRepeat<T>(int withIntervalInHours) where T : IJob
    {
        if (_scheduler == null) return;

        IJobDetail job = JobBuilder.Create<T>()
            .Build();

        jobDictionary.Add(typeof(T).Name, job);

        ITrigger trigger = TriggerBuilder.Create()
            .StartAt(SystemTime.UtcNow().AddDays(-1))
            .WithSimpleSchedule(x => x
                .WithIntervalInHours(withIntervalInHours)
                .RepeatForever())
            .Build();

        await _scheduler.ScheduleJob(job, trigger);
    }

    public async Task<bool> ExecuteJob(string jobClassName)
    {
        if (jobDictionary.TryGetValue(jobClassName, out var job))
        {
            var jobKey = job.Key;
            await _scheduler!.TriggerJob(jobKey);
            return true;
        }
        return false;
    }
}