namespace Skedl.DataCatcher.Services.Spbgu
{
    public interface ISpbguScheduleCatch
    {
        Task CatchScheduleAsync(int countWeek);
    }
}
