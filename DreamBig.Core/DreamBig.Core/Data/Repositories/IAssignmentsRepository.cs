using DreamBigManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public interface IAssignmentRepository
    {
        Task<List<ClassAssignment>> GetAssignmentsByClassAsync(int idClass);
        Task<bool> AddAssignmentAsync(ClassAssignment assignment);
        Task<bool> DeleteAssignmentAsync(int id);
        Task<List<WeeklyScheduleItem>> GetFullWeeklyScheduleAsync(int? idRoom = null);
        Task<int> GetTodayClassesCountAsync(int dayOfWeek);
        Task<List<WeeklyScheduleItem>> GetTodayClassesAsync(int dayOfWeek);
    }
}