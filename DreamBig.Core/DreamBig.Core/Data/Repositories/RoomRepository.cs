using Dapper;
using DreamBigManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private const string TableName = "rooms";

        public async Task<List<Room>> GetAllRoomsAsync()
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"SELECT * FROM {TableName} ORDER BY room_name ASC";
            var rooms = await db.QueryAsync<Room>(sql);
            return rooms.ToList();
        }

        public async Task<bool> AddRoomAsync(Room room)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"INSERT INTO {TableName} (room_name) VALUES (@room_name)";
            return await db.ExecuteAsync(sql, room) > 0;
        }

        public async Task<bool> UpdateRoomAsync(Room room)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"UPDATE {TableName} SET room_name = @room_name WHERE id_room = @id_room";
            return await db.ExecuteAsync(sql, room) > 0;
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"DELETE FROM {TableName} WHERE id_room = @id";
            return await db.ExecuteAsync(sql, new { id }) > 0;
        }
    }
}