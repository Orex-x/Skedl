using Quartz;
using Skedl.DataCatcher.Services.Spbgu;

namespace Skedl.DataCatcher.Services.Quartz.Spbgu;

public class SpbguScheduleCatchJob : IJob
{
    private readonly ISpbguScheduleCatch _spbguScheduleCatch;


    public SpbguScheduleCatchJob(ISpbguScheduleCatch spbguScheduleCatch)
    {
        _spbguScheduleCatch = spbguScheduleCatch;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _spbguScheduleCatch.CatchScheduleAsync(1);
    }
}