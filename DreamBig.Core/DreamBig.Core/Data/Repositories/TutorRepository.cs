using Dapper;
using DreamBigManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public class TutorRepository : ITutorRepository
    {
        private const string TableTutors = "tutors";
        private const string TableStudentTutor = "student_tutor";

        public async Task<List<Tutor>> GetAllTutorsAsync()
        {
            using var db = DatabaseManager.CreateConnection();
            return (await db.QueryAsync<Tutor>($@"SELECT * FROM {TableTutors} ORDER BY last_name, first_name")).ToList();
        }

        public async Task<Tutor> GetTutorByIdAsync(int id)
        {
            using var db = DatabaseManager.CreateConnection();
            return await db.QueryFirstOrDefaultAsync<Tutor>($@"SELECT * FROM {TableTutors} WHERE id_tutor = @id", new { id });
        }

        public async Task<int> AddTutorAsync(Tutor tutor)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"INSERT INTO {TableTutors} (first_name, last_name, phone_number, address) 
                           VALUES (@first_name, @last_name, @phone_number, @address)
                           RETURNING id_tutor;";
            return await db.ExecuteScalarAsync<int>(sql, tutor);
        }

        public async Task<bool> UpdateTutorAsync(Tutor tutor)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"UPDATE {TableTutors} SET first_name = @first_name, last_name = @last_name, 
                           phone_number = @phone_number, address = @address WHERE id_tutor = @id_tutor";
            return await db.ExecuteAsync(sql, tutor) > 0;
        }

        public async Task<bool> DeleteTutorAsync(int id)
        {
            using var db = DatabaseManager.CreateConnection();
            await db.OpenAsync();
            using var transaction = db.BeginTransaction();
            try
            {
                await db.ExecuteAsync($@"DELETE FROM {TableStudentTutor} WHERE id_tutor = @id", new { id }, transaction);
                int rows = await db.ExecuteAsync($@"DELETE FROM {TableTutors} WHERE id_tutor = @id", new { id }, transaction);
                await transaction.CommitAsync();
                return rows > 0;
            }
            catch { await transaction.RollbackAsync(); return false; }
        }

        public async Task<bool> LinkTutorToStudentAsync(int idStudent, int idTutor)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"INSERT INTO {TableStudentTutor} (id_student, id_tutor)
                           VALUES (@idStudent, @idTutor)
                           ON CONFLICT DO NOTHING";
            return await db.ExecuteAsync(sql, new { idStudent, idTutor }) > 0;
        }

        public async Task UnassignTutorAsync(int idStudent, int idTutor)
        {
            using var db = DatabaseManager.CreateConnection();
            await db.ExecuteAsync($@"DELETE FROM {TableStudentTutor} WHERE id_student = @idStudent AND id_tutor = @idTutor", new { idStudent, idTutor });
        }

        public async Task<List<Tutor>> GetTutorsByStudentAsync(int idStudent)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"SELECT t.* FROM {TableTutors} t
                           INNER JOIN {TableStudentTutor} st ON t.id_tutor = st.id_tutor
                           WHERE st.id_student = @idStudent";
            return (await db.QueryAsync<Tutor>(sql, new { idStudent })).ToList();
        }

        public async Task<bool> RemoveAllLinksAsync(int idStudent)
        {
            using var db = DatabaseManager.CreateConnection();
            return await db.ExecuteAsync($@"DELETE FROM {TableStudentTutor} WHERE id_student = @idStudent", new { idStudent }) >= 0;
        }
    }
}