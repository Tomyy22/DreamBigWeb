using Dapper;
using DreamBigManagement.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public class ClassGroupRepository : IClassGroupRepository
    {
        private const string TableClasses = "classes";
        private const string TableLevels = "levels";
        private const string TableTeachers = "teachers";
        private const string TableAssignments = "class_assignments";
        private const string TableSchedules = "schedules";
        private const string TableRooms = "rooms";
        private const string TableEnrollments = "enrollments";

        public async Task<List<ClassGroup>> GetAllClassesAsync()
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"SELECT c.id_class, c.id_level, c.id_teacher, c.modality,
                                   l.level_name, (t.first_name || ' ' || t.last_name) AS teacher_name
                            FROM {TableClasses} c
                            INNER JOIN {TableLevels} l ON c.id_level = l.id_level
                            INNER JOIN {TableTeachers} t ON c.id_teacher = t.id_teacher";
            return (await db.QueryAsync<ClassGroup>(sql)).ToList();
        }

        public async Task<ClassGroup> GetClassByIdAsync(int id)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"SELECT c.id_class, c.id_level, c.id_teacher, c.modality, 
                                   l.level_name, 
                                   t.first_name || ' ' || t.last_name as teacher_name
                            FROM {TableClasses} c
                            JOIN {TableLevels} l ON c.id_level = l.id_level
                            JOIN {TableTeachers} t ON c.id_teacher = t.id_teacher
                            WHERE c.id_class = @id";

            return await db.QueryFirstOrDefaultAsync<ClassGroup>(sql, new { id });
        }

        public async Task<int> AddClassAsync(ClassGroup newClass)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"INSERT INTO {TableClasses} (id_level, id_teacher, modality) 
                            VALUES (@id_level, @id_teacher, @modality)
                            RETURNING id_class;";
            return await db.ExecuteScalarAsync<int>(sql, newClass);
        }

        public async Task<bool> UpdateClassAsync(ClassGroup classObj)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"UPDATE {TableClasses} SET id_level = @id_level, id_teacher = @id_teacher, 
                            modality = @modality WHERE id_class = @id_class";
            return await db.ExecuteAsync(sql, classObj) > 0;
        }

        public async Task<bool> DeleteClassAsync(int id)
        {
            using var db = DatabaseManager.CreateConnection();
            await db.OpenAsync();
            using var transaction = db.BeginTransaction();
            try
            {
                await db.ExecuteAsync($"DELETE FROM {TableAssignments} WHERE id_class = @id", new { id }, transaction);
                await db.ExecuteAsync($"DELETE FROM {TableEnrollments} WHERE id_class = @id", new { id }, transaction);
                int rows = await db.ExecuteAsync($"DELETE FROM {TableClasses} WHERE id_class = @id", new { id }, transaction);

                await transaction.CommitAsync();
                return rows > 0;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<ClassGroup>> GetClassesByLevelAsync(int idLevel)
        {
            using var db = DatabaseManager.CreateConnection();
            return (await db.QueryAsync<ClassGroup>($"SELECT * FROM {TableClasses} WHERE id_level = @idLevel", new { idLevel })).ToList();
        }

        public async Task<List<ClassGroup>> GetClassesByDayAsync(int dayNumber)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"SELECT DISTINCT c.id_class, c.id_level, c.id_teacher, c.modality,
                                   l.level_name, (t.first_name || ' ' || t.last_name) AS teacher_name,
                                   s.start_time, r.room_name
                            FROM {TableClasses} c
                            INNER JOIN {TableLevels} l ON c.id_level = l.id_level
                            INNER JOIN {TableTeachers} t ON c.id_teacher = t.id_teacher
                            INNER JOIN {TableAssignments} ca ON c.id_class = ca.id_class
                            INNER JOIN {TableSchedules} s ON ca.id_schedule = s.id_schedule
                            INNER JOIN {TableRooms} r ON ca.id_room = r.id_room
                            WHERE ca.day_of_week = @dayNumber
                            ORDER BY s.start_time ASC";
            return (await db.QueryAsync<ClassGroup>(sql, new { dayNumber })).ToList();
        }
    }
}