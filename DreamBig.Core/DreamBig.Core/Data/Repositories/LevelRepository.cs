using Dapper;
using DreamBigManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public class LevelRepository : ILevelRepository
    {
        private const string TableName = "levels";

        public async Task<List<Level>> GetAllLevelsAsync()
        {
            using var db = DatabaseManager.CreateConnection();
            var levels = await db.QueryAsync<Level>($"SELECT * FROM {TableName}");
            return levels.ToList();
        }

        public async Task<Level> GetByIdAsync(int id)
        {
            using var db = DatabaseManager.CreateConnection();
            return await db.QueryFirstOrDefaultAsync<Level>(
                $"SELECT * FROM {TableName} WHERE id_level = @id", new { id });
        }

        public async Task<int> AddLevelAsync(Level level)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"INSERT INTO {TableName} (level_name, price) 
                           VALUES (@level_name, @price)
                           RETURNING id_level;";
            return await db.ExecuteScalarAsync<int>(sql, level);
        }

        public async Task<bool> UpdateLevelAsync(Level level)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"UPDATE {TableName} 
                           SET level_name = @level_name, price = @price 
                           WHERE id_level = @id_level";
            return await db.ExecuteAsync(sql, level) > 0;
        }

        public async Task<bool> DeleteLevelAsync(int id)
        {
            using var db = DatabaseManager.CreateConnection();
            return await db.ExecuteAsync($"DELETE FROM {TableName} WHERE id_level = @id", new { id }) > 0;
        }
    }
}