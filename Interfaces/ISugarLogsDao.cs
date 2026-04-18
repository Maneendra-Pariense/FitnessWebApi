using FitnessApi.Models;

namespace FitnessApi.Interfaces
{
    public interface ISugarLogsDao
    {
        Task<IList<SugarLogDto>> GetAllSugarLogs(string userId);
        Task AddSugarLog(string userId, SugarLogDto logDto);
    }
}
