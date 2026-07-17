using Dapper;
using DreamBigManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private const string TablePayments = "payments";
        private const string TableStudents = "students";

        public async Task<List<Payment>> GetPaymentsByStudentAsync(int idStudent, int year)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"SELECT * FROM {TablePayments} WHERE id_student = @idStudent AND year = @year";
            return (await db.QueryAsync<Payment>(sql, new { idStudent, year })).ToList();
        }

        public async Task<bool> SavePaymentAsync(Payment payment)
        {
            using var db = DatabaseManager.CreateConnection();

            string checkSql = $@"SELECT id_payment FROM {TablePayments} 
                                WHERE id_student = @id_student AND month = @month AND year = @year";

            var existingId = await db.QueryFirstOrDefaultAsync<int?>(checkSql, payment);

            if (existingId.HasValue)
            {
                string updateSql = $@"UPDATE {TablePayments} 
                                     SET is_paid = @is_paid, payment_date = @payment_date, amount = @amount
                                     WHERE id_payment = @id_payment";
                payment.id_payment = existingId.Value;
                return await db.ExecuteAsync(updateSql, payment) > 0;
            }
            else
            {
                string insertSql = $@"INSERT INTO {TablePayments} (id_student, year, month, amount, is_paid, payment_date)
                                     VALUES (@id_student, @year, @month, @amount, @is_paid, @payment_date)";
                return await db.ExecuteAsync(insertSql, payment) > 0;
            }
        }

        public async Task<bool> DeletePaymentAsync(int idPayment)
        {
            using var db = DatabaseManager.CreateConnection();
            return await db.ExecuteAsync($"DELETE FROM {TablePayments} WHERE id_payment = @idPayment", new { idPayment }) > 0;
        }

        public async Task<List<Payment>> GetPaymentsByMonthAsync(int month, int year)
        {
            using var db = DatabaseManager.CreateConnection();
            string sql = $@"SELECT * FROM {TablePayments} WHERE month = @month AND year = @year AND is_paid = 1";
            return (await db.QueryAsync<Payment>(sql, new { month, year })).ToList();
        }


        public async Task<int> GetDebtorsCountAsync()
        {
            var debtors = await GetDebtorsAsync();
            return debtors.Count;
        }

        public async Task<List<Student>> GetDebtorsAsync()
        {
            using var db = DatabaseManager.CreateConnection();

            int year = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;

            if (currentMonth < 2) return new List<Student>();

            string studentSql = $@"SELECT s.*, l.level_name as current_level_name
                                  FROM {TableStudents} s
                                  LEFT JOIN enrollments e ON s.id_student = e.id_student
                                  LEFT JOIN classes c ON e.id_class = c.id_class
                                  LEFT JOIN levels l ON c.id_level = l.id_level
                                  WHERE s.status = 1";

            var students = (await db.QueryAsync<Student>(studentSql))
                           .GroupBy(x => x.id_student)
                           .Select(g => g.First())
                           .ToList();

            string paymentSql = $@"SELECT id_student, month, is_paid 
                                  FROM {TablePayments} WHERE year = @year";

            var allPayments = (await db.QueryAsync<Payment>(paymentSql, new { year }))
                              .ToLookup(p => p.id_student);

            List<Student> debtors = new List<Student>();

            foreach (var student in students)
            {
                var studentPayments = allPayments[student.id_student].ToDictionary(x => x.month);
                bool owes = false;

                for (int month = 2; month <= currentMonth; month++)
                {
                    if (!studentPayments.TryGetValue(month, out var payment) || payment.is_paid == 0)
                    {
                        owes = true;
                        break;
                    }
                }

                if (owes) debtors.Add(student);
            }

            return debtors;
        }
    }
}