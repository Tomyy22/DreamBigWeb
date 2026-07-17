using DreamBigManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public interface ILevelRepository
    {
        Task<List<Level>> GetAllLevelsAsync();
        Task<Level> GetByIdAsync(int id);
        Task<int> AddLevelAsync(Level level);
        Task<bool> UpdateLevelAsync(Level level);
        Task<bool> DeleteLevelAsync(int id);
    }
}