using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICurrencyService
    {
        public Task<CurrencyViewModelCount> GetCurrencyListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);

        public Task<M_Currency> GetCurrencyByIdAsync(string RegId, Int16 CompanyId, Int32 CurrencyId, Int32 UserId);

        public Task<SqlResponce> AddCurrencyAsync(string RegId, Int16 CompanyId, M_Currency M_Currency, Int32 UserId);

        public Task<SqlResponce> UpdateCurrencyAsync(string RegId, Int16 CompanyId, M_Currency M_Currency, Int32 UserId);

        public Task<SqlResponce> DeleteCurrencyAsync(string RegId, Int16 CompanyId, M_Currency M_Currency, Int32 UserId);
    }
}