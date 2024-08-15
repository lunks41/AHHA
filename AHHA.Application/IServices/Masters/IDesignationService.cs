using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IDesignationService
    {
        public Task<DesignationViewModelCount> GetDesignationListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_Designation> GetDesignationByIdAsync(Int16 CompanyId, Int16 DesignationId, Int32 UserId);
        public Task<SqlResponce> AddDesignationAsync(Int16 CompanyId, M_Designation M_Designation, Int32 UserId);
        public Task<SqlResponce> UpdateDesignationAsync(Int16 CompanyId, M_Designation M_Designation, Int32 UserId);
        public Task<SqlResponce> DeleteDesignationAsync(Int16 CompanyId, M_Designation M_Designation, Int32 UserId);
    }
}
