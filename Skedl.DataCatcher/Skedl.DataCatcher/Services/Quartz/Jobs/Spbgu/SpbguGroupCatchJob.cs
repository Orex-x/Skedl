using Quartz;
using Skedl.DataCatcher.Services.Spbgu;

namespace Skedl.DataCatcher.Services.Quartz.Spbgu;

public class SpbguGroupCatchJob : IJob
{
    private readonly SpbguGroupCatch _spbguGroupCatch;

    public SpbguGroupCatchJob(SpbguGroupCatch spbguGroupCatch)
    {
        _spbguGroupCatch = spbguGroupCatch;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        await _spbguGroupCatch.CatchGroups();
    }
}