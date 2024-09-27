using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ISupplierBankService
    {
        public Task<IEnumerable<SupplierBankViewModel>> GetSupplierBankBySupplierIdAsync(string RegId, Int16 CompanyId, Int32 SupplierId, Int16 UserId);

        public Task<SupplierBankViewModel> GetSupplierBankByIdAsync(string RegId, Int16 CompanyId, Int32 SupplierId, Int16 SupplierBankId, Int16 UserId);

        public Task<SqlResponce> SaveSupplierBankAsync(string RegId, Int16 CompanyId, M_SupplierBank m_SupplierBank, Int16 UserId);

        public Task<SqlResponce> DeleteSupplierBankAsync(string RegId, Int16 CompanyId, Int32 SupplierId, Int16 BaSupplierBankIdnkId, Int16 UserId);
    }
}