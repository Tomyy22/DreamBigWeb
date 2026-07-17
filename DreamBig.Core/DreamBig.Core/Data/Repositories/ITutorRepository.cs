using DreamBigManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public interface ITutorRepository
    {
        Task<List<Tutor>> GetAllTutorsAsync();
        Task<Tutor> GetTutorByIdAsync(int id);
        Task<int> AddTutorAsync(Tutor tutor);
        Task<bool> UpdateTutorAsync(Tutor tutor);
        Task<bool> DeleteTutorAsync(int id);
        Task<bool> LinkTutorToStudentAsync(int idStudent, int idTutor);
        Task UnassignTutorAsync(int idStudent, int idTutor);
        Task<List<Tutor>> GetTutorsByStudentAsync(int idStudent);
        Task<bool> RemoveAllLinksAsync(int idStudent);
    }
}