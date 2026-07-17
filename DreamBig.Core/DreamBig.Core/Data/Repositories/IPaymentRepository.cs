using DreamBigManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public interface IPaymentRepository
    {
        Task<List<Payment>> GetPaymentsByStudentAsync(int idStudent, int year);
        Task<bool> SavePaymentAsync(Payment payment);
        Task<bool> DeletePaymentAsync(int idPayment);
        Task<List<Payment>> GetPaymentsByMonthAsync(int month, int year);
        Task<int> GetDebtorsCountAsync();
        Task<List<Student>> GetDebtorsAsync();
    }
}