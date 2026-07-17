using Dapper;
using DreamBigManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private const string TableStudents = "students";
        private const string TableEnrollments = "enrollments";
        private const string TableClasses = "classes";
        private const string TableLevels = "levels";

        public async Task<List<Student>> GetAllStudentsAsync(bool includeInactive = false)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"SELECT s.*, l.level_name as current_level_name 
                           FROM {TableStudents} s
                           LEFT JOIN {TableEnrollments} e ON s.id_student = e.id_student
                           LEFT JOIN {TableClasses} c ON e.id_class = c.id_class
                           LEFT JOIN {TableLevels} l ON c.id_level = l.id_level";

            if (!includeInactive) sql += " WHERE s.status = 1";

            var result = await db.QueryAsync<Student>(sql);
            return result.GroupBy(x => x.id_student).Select(g => g.First()).ToList();
        }

        public async Task<Student> GetStudentByDNIAsync(string dni)
        {
            using var db = DatabaseManager.CreateConnection();
            return await db.QueryFirstOrDefaultAsync<Student>(
                $"SELECT * FROM {TableStudents} WHERE dni = @dni", new { dni });
        }

        public async Task<Student> GetStudentByIDAsync(int id_student)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"SELECT s.*, l.price, l.level_name as current_level_name
                           FROM {TableStudents} s
                           LEFT JOIN {TableEnrollments} e ON s.id_student = e.id_student
                           LEFT JOIN {TableClasses} c ON e.id_class = c.id_class
                           LEFT JOIN {TableLevels} l ON c.id_level = l.id_level
                           WHERE s.id_student = @id_student";

            return await db.QueryFirstOrDefaultAsync<Student>(sql, new { id_student });
        }

        public async Task<int> AddStudentAsync(Student student)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"INSERT INTO {TableStudents} (dni, first_name, last_name, phone, status) 
                           VALUES (@DNI, @first_name, @last_name, @phone, @status)
                           RETURNING id_student;";
            return await db.ExecuteScalarAsync<int>(sql, student);
        }

        public async Task<bool> UpdateStudentAsync(Student student)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"UPDATE {TableStudents} 
                           SET dni = @DNI, first_name = @first_name, 
                           last_name = @last_name, phone = @phone, status = @status 
                           WHERE id_student = @id_student";
            return await db.ExecuteAsync(sql, student) > 0;
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            using var db = DatabaseManager.CreateConnection();
            return await db.ExecuteAsync($"UPDATE {TableStudents} SET status = 0 WHERE id_student = @id", new { id }) > 0;
        }
    }
}