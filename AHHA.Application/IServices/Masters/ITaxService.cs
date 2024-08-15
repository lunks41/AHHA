using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ITaxService
    {
        public Task<TaxViewModelCount> GetTaxListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_Tax> GetTaxByIdAsync(Int16 CompanyId, Int16 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddTaxAsync(Int16 CompanyId, M_Tax M_Tax, Int32 UserId);
        public Task<SqlResponce> UpdateTaxAsync(Int16 CompanyId, M_Tax M_Tax, Int32 UserId);
        public Task<SqlResponce> DeleteTaxAsync(Int16 CompanyId, M_Tax M_Tax, Int32 UserId);
    }
}
