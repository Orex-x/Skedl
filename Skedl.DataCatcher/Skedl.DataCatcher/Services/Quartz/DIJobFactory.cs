using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using Skedl.DataCatcher.Services.HttpServices;
using Skedl.DataCatcher.Services.RabbitMqServices;

namespace Skedl.DataCatcher.Services.Quartz;

public class DiJobFactory : IJobFactory
{
    protected readonly IServiceProvider ServiceProvider;

    protected readonly ConcurrentDictionary<IJob, IServiceScope> Scopes = new ConcurrentDictionary<IJob, IServiceScope>();

    public DiJobFactory(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        var scope = ServiceProvider.CreateScope();
        IJob job;

        try
        {
            job = scope.ServiceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
        }
        catch
        {
            // Failed to create the job -> ensure scope gets disposed
            scope.Dispose();
            throw;
        }

        // Add scope to dictionary so we can dispose it once the job finishes
        if (!Scopes.TryAdd(job, scope))
        {
            // Failed to track DI scope -> ensure scope gets disposed
            scope.Dispose();
            throw new Exception("Failed to track DI scope");
        }

        return job;
    }

    public void ReturnJob(IJob job)
    {
        if (Scopes.TryRemove(job, out var scope))
        {
            // Manually dispose of scoped services except for DbContext
            foreach (var service in scope.ServiceProvider.GetServices<object>())
            {
                if (service is DbContext)
                {
                    // Skip disposing of DbContext instances
                    continue;
                }

                if (service is IDisposable disposableService)
                {
                     disposableService.Dispose();
                }
            }
        }
    }
}