using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;

namespace Skedl.DataStorage.Services;

public class Huiznaetkaktebyanazvazz
{
    public async void Start()
    {
        StdSchedulerFactory factory = new StdSchedulerFactory();
        IScheduler scheduler = await factory.GetScheduler();
        
        IJobDetail job = JobBuilder.Create<HelloJob>()
            .Build();
    
        // Trigger the job to run now, and then repeat every 10 seconds
        ITrigger trigger = TriggerBuilder.Create()
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(40)
                .RepeatForever())
            .Build();
        
        await scheduler.ScheduleJob(job, trigger);
        
        await scheduler.Start(); // Начать планировщик
    }
}