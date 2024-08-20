using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IVesselService
    {
        public Task<VesselViewModelCount> GetVesselListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);

        public Task<M_Vessel> GetVesselByIdAsync(string RegId, Int16 CompanyId, Int32 COACategoryId, Int32 UserId);

        public Task<SqlResponce> AddVesselAsync(string RegId, Int16 CompanyId, M_Vessel M_Vessel, Int32 UserId);

        public Task<SqlResponce> UpdateVesselAsync(string RegId, Int16 CompanyId, M_Vessel M_Vessel, Int32 UserId);

        public Task<SqlResponce> DeleteVesselAsync(string RegId, Int16 CompanyId, M_Vessel M_Vessel, Int32 UserId);
    }
}