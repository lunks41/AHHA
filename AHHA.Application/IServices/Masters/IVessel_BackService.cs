using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IVessel_BackService
    {
        public Task<Vessel_BackViewModelCount> GetVessel_BackListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_Vessel_Back> GetVessel_BackByIdAsync(Int16 CompanyId, Int32 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddVessel_BackAsync(Int16 CompanyId, M_Vessel_Back M_Vessel_Back, Int32 UserId);
        public Task<SqlResponce> UpdateVessel_BackAsync(Int16 CompanyId, M_Vessel_Back M_Vessel_Back, Int32 UserId);
        public Task<SqlResponce> DeleteVessel_BackAsync(Int16 CompanyId, M_Vessel_Back M_Vessel_Back, Int32 UserId);
    }
}
