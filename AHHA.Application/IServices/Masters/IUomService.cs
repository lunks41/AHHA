using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IUomService
    {
        public Task<UomViewModelCount> GetUomListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_Uom> GetUomByIdAsync(string RegId, Int16 CompanyId, Int16 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddUomAsync(string RegId, Int16 CompanyId, M_Uom M_Uom, Int32 UserId);
        public Task<SqlResponce> UpdateUomAsync(string RegId, Int16 CompanyId, M_Uom M_Uom, Int32 UserId);
        public Task<SqlResponce> DeleteUomAsync(string RegId, Int16 CompanyId, M_Uom M_Uom, Int32 UserId);
    }
}
