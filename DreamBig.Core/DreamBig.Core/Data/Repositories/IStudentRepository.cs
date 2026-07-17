using DreamBigManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAllStudentsAsync(bool includeInactive = false);
        Task<Student> GetStudentByDNIAsync(string dni);
        Task<int> AddStudentAsync(Student student);
        Task<bool> UpdateStudentAsync(Student student);
        Task<bool> DeleteStudentAsync(int id);
        Task<Student> GetStudentByIDAsync(int id_student);
    }
}