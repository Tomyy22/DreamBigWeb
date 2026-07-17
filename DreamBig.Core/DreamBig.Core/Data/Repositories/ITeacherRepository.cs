using DreamBigManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public interface ITeacherRepository
    {
        Task<List<Teacher>> GetAllTeachersAsync();
        Task<Teacher> GetTeacherByIdAsync(int id);
        Task<int> AddTeacherAsync(Teacher teacher);
        Task<bool> UpdateTeacherAsync(Teacher teacher);
        Task<bool> DeleteTeacherAsync(int id);
    }
}