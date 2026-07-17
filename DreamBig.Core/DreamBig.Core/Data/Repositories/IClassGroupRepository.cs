using DreamBigManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public interface IClassGroupRepository
    {
        Task<List<ClassGroup>> GetAllClassesAsync();
        Task<ClassGroup> GetClassByIdAsync(int id);
        Task<int> AddClassAsync(ClassGroup newClass);
        Task<bool> UpdateClassAsync(ClassGroup classObj);
        Task<bool> DeleteClassAsync(int id);
        Task<List<ClassGroup>> GetClassesByLevelAsync(int idLevel);
        Task<List<ClassGroup>> GetClassesByDayAsync(int dayNumber);
    }
}