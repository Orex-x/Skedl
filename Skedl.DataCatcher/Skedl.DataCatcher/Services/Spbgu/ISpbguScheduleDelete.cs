namespace Skedl.DataCatcher.Services.Spbgu
{
    public interface ISpbguScheduleDelete
    {
        Task DeleteLastWeek();
        Task DeleteWeek(int days);
    }
}
