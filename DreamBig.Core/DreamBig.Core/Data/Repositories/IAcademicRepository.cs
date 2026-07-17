using DreamBigManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DreamBig.Core.Data.Repositories
{
    public interface IAcademicRepository
    {
        Task<List<AcademicHistory>> GetHistoryByStudentAsync(int idStudent);
        Task<bool> AddGradeAsync(AcademicHistory note);
        Task<bool> UpdateGradeAsync(AcademicHistory note);
        Task<bool> DeleteGradeAsync(int idGrade);
        Task<double> GetGeneralAverageAsync(int idStudent);
    }
}