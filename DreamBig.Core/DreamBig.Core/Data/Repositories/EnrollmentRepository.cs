using Dapper;
using DreamBigManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private const string TableEnrollments = "enrollments";
        private const string TableStudents = "students";

        public async Task<List<Enrollment>> GetStudentsInClassAsync(int idClass)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"SELECT e.id_enrollment, e.id_student, e.id_class, e.school_year,
                                   (s.last_name || ', ' || s.first_name) AS student_name
                            FROM {TableEnrollments} e
                            INNER JOIN {TableStudents} s ON e.id_student = s.id_student
                            WHERE e.id_class = @idClass";

            var enrollments = await db.QueryAsync<Enrollment>(sql, new { idClass });
            return enrollments.ToList();
        }

        public async Task<bool> AddEnrollmentAsync(Enrollment enrollment)
        {
            using var db = DatabaseManager.CreateConnection();

            string sqlCheck = $@"SELECT COUNT(1) FROM {TableEnrollments} 
                                WHERE id_class = @id_class AND id_student = @id_student";

            int existe = await db.ExecuteScalarAsync<int>(sqlCheck, enrollment);
            if (existe > 0)
            {
                throw new Exception("El alumno ya se encuentra inscripto en esta clase.");
            }

            string sql = $@"INSERT INTO {TableEnrollments} (id_class, id_student, school_year) 
                            VALUES (@id_class, @id_student, @school_year)";

            return await db.ExecuteAsync(sql, enrollment) > 0;
        }

        public async Task<bool> DeleteEnrollmentAsync(int id)
        {
            using var db = DatabaseManager.CreateConnection();
            return await db.ExecuteAsync($"DELETE FROM {TableEnrollments} WHERE id_enrollment = @id", new { id }) > 0;
        }
    }
}