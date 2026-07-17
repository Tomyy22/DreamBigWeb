using Dapper;
using DreamBigManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private const string TableName = "teachers";

        public async Task<List<Teacher>> GetAllTeachersAsync()
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"SELECT * FROM {TableName} ORDER BY last_name ASC";
            var teachers = await db.QueryAsync<Teacher>(sql);
            return teachers.ToList();
        }

        public async Task<Teacher> GetTeacherByIdAsync(int id)
        {
            using var db = DatabaseManager.CreateConnection();
            return await db.QueryFirstOrDefaultAsync<Teacher>(
                $"SELECT * FROM {TableName} WHERE id_teacher = @id", new { id });
        }

        public async Task<int> AddTeacherAsync(Teacher teacher)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"INSERT INTO {TableName} (first_name, last_name, phone) 
                           VALUES (@first_name, @last_name, @phone)
                           RETURNING id_teacher;";
            return await db.ExecuteScalarAsync<int>(sql, teacher);
        }

        public async Task<bool> UpdateTeacherAsync(Teacher teacher)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"UPDATE {TableName} 
                           SET first_name = @first_name, 
                               last_name = @last_name, 
                               phone = @phone 
                           WHERE id_teacher = @id_teacher";
            return await db.ExecuteAsync(sql, teacher) > 0;
        }

        public async Task<bool> DeleteTeacherAsync(int id)
        {
            using var db = DatabaseManager.CreateConnection();
            return await db.ExecuteAsync($"DELETE FROM {TableName} WHERE id_teacher = @id", new { id }) > 0;
        }
    }
}