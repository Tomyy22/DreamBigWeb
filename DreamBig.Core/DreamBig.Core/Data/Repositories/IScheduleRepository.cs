using DreamBigManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public interface IScheduleRepository
    {
        Task<List<Schedule>> GetAllSchedulesAsync();
        Task<Schedule> GetScheduleByIdAsync(int id);
        Task<bool> AddScheduleAsync(Schedule schedule);
        Task<bool> UpdateScheduleAsync(Schedule schedule);
        Task<bool> DeleteScheduleAsync(int id);
    }
}