using DreamBigManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public interface IEnrollmentRepository
    {
        Task<List<Enrollment>> GetStudentsInClassAsync(int idClass);
        Task<bool> AddEnrollmentAsync(Enrollment enrollment);
        Task<bool> DeleteEnrollmentAsync(int id);
    }
}