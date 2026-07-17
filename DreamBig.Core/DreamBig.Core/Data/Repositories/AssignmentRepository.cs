using Dapper;
using DreamBigManagement.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private const string TableName = "class_assignments";

        public async Task<List<ClassAssignment>> GetAssignmentsByClassAsync(int idClass)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"SELECT a.*, r.room_name, s.start_time, s.end_time
                           FROM {TableName} a
                           INNER JOIN rooms r ON a.id_room = r.id_room
                           INNER JOIN schedules s ON a.id_schedule = s.id_schedule
                           WHERE a.id_class = @idClass
                           ORDER BY a.day_of_week, s.start_time";
            return (await db.QueryAsync<ClassAssignment>(sql, new { idClass })).ToList();
        }

        public async Task<bool> AddAssignmentAsync(ClassAssignment assignment)
        {
            using var db = DatabaseManager.CreateConnection();
            await db.OpenAsync();

            string sqlCheckClase = $@"SELECT COUNT(1) FROM {TableName} 
                                     WHERE id_class = @id_class AND day_of_week = @day_of_week AND id_schedule = @id_schedule";
            if (await db.ExecuteScalarAsync<int>(sqlCheckClase, assignment) > 0)
                throw new Exception("Esta clase ya tiene un aula asignada en este día y horario.");

            string sqlCheckProfe = $@"SELECT COUNT(1) FROM {TableName} ca
                                     INNER JOIN classes c ON ca.id_class = c.id_class
                                     WHERE c.id_teacher = (SELECT id_teacher FROM classes WHERE id_class = @id_class)
                                     AND ca.day_of_week = @day_of_week AND ca.id_schedule = @id_schedule";
            if (await db.ExecuteScalarAsync<int>(sqlCheckProfe, assignment) > 0)
                throw new Exception("El profesor ya tiene otra clase asignada en este mismo horario.");

            try
            {
                string sql = $@"INSERT INTO {TableName} (id_class, id_room, id_schedule, day_of_week) 
                               VALUES (@id_class, @id_room, @id_schedule, @day_of_week)";
                return await db.ExecuteAsync(sql, assignment) > 0;
            }
            catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                throw new Exception("¡Conflicto! El aula ya está ocupada por otra clase en ese horario.");
            }
        }

        public async Task<bool> DeleteAssignmentAsync(int id)
        {
            using var db = DatabaseManager.CreateConnection();
            return await db.ExecuteAsync($"DELETE FROM {TableName} WHERE id_assignment = @id", new { id }) > 0;
        }

        public async Task<List<WeeklyScheduleItem>> GetFullWeeklyScheduleAsync(int? idRoom = null)
        {
            using var db = DatabaseManager.CreateConnection();

            string sql = $@"SELECT 
                            ca.id_assignment, 
                            ca.day_of_week, 
                            ca.id_schedule, 
                            c.id_class, 
                            l.level_name, 
                            r.room_name, 
                            t.first_name || ' ' || t.last_name AS teacher_name
                            FROM {TableName} ca
                            JOIN classes c ON ca.id_class = c.id_class
                            JOIN levels l ON c.id_level = l.id_level
                            JOIN rooms r ON ca.id_room = r.id_room
                            JOIN teachers t ON c.id_teacher = t.id_teacher";

            if (idRoom.HasValue)
                sql += " WHERE ca.id_room = @idRoom";

            return (await db.QueryAsync<WeeklyScheduleItem>(sql, new { idRoom })).ToList();
        }

        public async Task<int> GetTodayClassesCountAsync(int dayOfWeek)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"SELECT COUNT(*) FROM {TableName} WHERE day_of_week = @dayOfWeek";
            return await db.ExecuteScalarAsync<int>(sql, new { dayOfWeek });
        }

        public async Task<List<WeeklyScheduleItem>> GetTodayClassesAsync(int dayOfWeek)
        {
            using var db = DatabaseManager.CreateConnection();

            string sql = $@"SELECT
                            ca.id_assignment,
                            ca.day_of_week,
                            ca.id_schedule,
                            c.id_class,
                            l.level_name,
                            r.room_name,
                            t.first_name || ' ' || t.last_name AS teacher_name,
                            s.start_time,
                            s.end_time
                        FROM {TableName} ca
                        INNER JOIN classes c ON ca.id_class = c.id_class
                        INNER JOIN levels l ON c.id_level = l.id_level
                        INNER JOIN rooms r ON ca.id_room = r.id_room
                        INNER JOIN teachers t ON c.id_teacher = t.id_teacher
                        INNER JOIN schedules s ON ca.id_schedule = s.id_schedule
                        WHERE ca.day_of_week = @dayOfWeek
                        ORDER BY s.start_time;";

            return (await db.QueryAsync<WeeklyScheduleItem>(sql, new { dayOfWeek })).ToList();
        }
    }
}