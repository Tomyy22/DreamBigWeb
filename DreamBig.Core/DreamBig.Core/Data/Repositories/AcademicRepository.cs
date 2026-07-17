using Dapper;
using DreamBigManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public class AcademicRepository : IAcademicRepository
    {
        private const string TableName = "academic_history";
        private const string LevelTable = "levels";

        public async Task<List<AcademicHistory>> GetHistoryByStudentAsync(int idStudent)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"SELECT h.*, l.level_name as LevelName 
                           FROM {TableName} h
                           INNER JOIN {LevelTable} l ON h.id_level_taken = l.id_level
                           WHERE h.id_student = @idStudent
                           ORDER BY h.school_year DESC, h.exam_date DESC";

            var result = await db.QueryAsync<AcademicHistory>(sql, new { idStudent });
            return result.ToList();
        }

        public async Task<bool> AddGradeAsync(AcademicHistory note)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"INSERT INTO {TableName} 
                           (id_student, school_year, id_level_taken, written_score, oral_score, intl_exam_info, exam_date)
                           VALUES 
                           (@id_student, @school_year, @id_level_taken, @written_score, @oral_score, @intl_exam_info, @exam_date)";

            return await db.ExecuteAsync(sql, note) > 0;
        }

        public async Task<bool> UpdateGradeAsync(AcademicHistory note)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"UPDATE {TableName} 
                           SET school_year = @school_year,
                               id_level_taken = @id_level_taken,
                               written_score = @written_score,
                               oral_score = @oral_score,
                               intl_exam_info = @intl_exam_info,
                               exam_date = @exam_date
                           WHERE id_grade = @id_grade";

            return await db.ExecuteAsync(sql, note) > 0;
        }

        public async Task<bool> DeleteGradeAsync(int idGrade)
        {
            using var db = DatabaseManager.CreateConnection();
            return await db.ExecuteAsync($"DELETE FROM {TableName} WHERE id_grade = @idGrade", new { idGrade }) > 0;
        }

        public async Task<double> GetGeneralAverageAsync(int idStudent)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"SELECT COALESCE(AVG(((written_score + oral_score) / 2.0)::float), 0)
                           FROM {TableName} 
                           WHERE id_student = @idStudent";

            return await db.ExecuteScalarAsync<double>(sql, new { idStudent });
        }
    }
}