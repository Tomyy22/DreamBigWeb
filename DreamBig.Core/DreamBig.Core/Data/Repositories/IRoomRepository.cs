using DreamBigManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public interface IRoomRepository
    {
        Task<List<Room>> GetAllRoomsAsync();
        Task<bool> AddRoomAsync(Room room);
        Task<bool> UpdateRoomAsync(Room room);
        Task<bool> DeleteRoomAsync(int id);
    }
}