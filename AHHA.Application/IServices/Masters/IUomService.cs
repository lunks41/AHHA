using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IUomService
    {
        public Task<UomViewModelCount> GetUomListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_Uom> GetUomByIdAsync(Int16 CompanyId, Int16 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddUomAsync(Int16 CompanyId, M_Uom M_Uom, Int32 UserId);
        public Task<SqlResponce> UpdateUomAsync(Int16 CompanyId, M_Uom M_Uom, Int32 UserId);
        public Task<SqlResponce> DeleteUomAsync(Int16 CompanyId, M_Uom M_Uom, Int32 UserId);
    }
}
