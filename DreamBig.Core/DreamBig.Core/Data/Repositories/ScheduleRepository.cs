using Dapper;
using DreamBigManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private const string TableName = "schedules";

        public async Task<List<Schedule>> GetAllSchedulesAsync()
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"SELECT * FROM {TableName} ORDER BY start_time ASC";
            var schedules = await db.QueryAsync<Schedule>(sql);
            return schedules.ToList();
        }

        public async Task<Schedule> GetScheduleByIdAsync(int id)
        {
            using var db = DatabaseManager.CreateConnection();
            return await db.QueryFirstOrDefaultAsync<Schedule>(
                $"SELECT * FROM {TableName} WHERE id_schedule = @id", new { id });
        }

        public async Task<bool> AddScheduleAsync(Schedule schedule)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"INSERT INTO {TableName} (start_time, end_time) VALUES (@start_time, @end_time)";
            return await db.ExecuteAsync(sql, schedule) > 0;
        }

        public async Task<bool> UpdateScheduleAsync(Schedule schedule)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"UPDATE {TableName} 
                           SET start_time = @start_time, end_time = @end_time 
                           WHERE id_schedule = @id_schedule";
            return await db.ExecuteAsync(sql, schedule) > 0;
        }

        public async Task<bool> DeleteScheduleAsync(int id)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"DELETE FROM {TableName} WHERE id_schedule = @id";
            return await db.ExecuteAsync(sql, new { id }) > 0;
        }
    }
}