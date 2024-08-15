using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IVoyageService
    {
        public Task<VoyageViewModelCount> GetVoyageListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_Voyage> GetVoyageByIdAsync(Int16 CompanyId, Int32 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddVoyageAsync(Int16 CompanyId, M_Voyage M_Voyage, Int32 UserId);
        public Task<SqlResponce> UpdateVoyageAsync(Int16 CompanyId, M_Voyage M_Voyage, Int32 UserId);
        public Task<SqlResponce> DeleteVoyageAsync(Int16 CompanyId, M_Voyage M_Voyage, Int32 UserId);
    }
}
