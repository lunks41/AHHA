using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICustomeGroupCreditLimtService
    {
        public Task<CustomeGroupCreditLimtViewModelCount> GetCustomeGroupCreditLimtListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);

        public Task<M_CustomeGroupCreditLimt> GetCustomeGroupCreditLimtByIdAsync(string RegId, Int16 CompanyId, Int32 COACategoryId, Int32 UserId);

        public Task<SqlResponce> AddCustomeGroupCreditLimtAsync(string RegId, Int16 CompanyId, M_CustomeGroupCreditLimt M_CustomeGroupCreditLimt, Int32 UserId);

        public Task<SqlResponce> UpdateCustomeGroupCreditLimtAsync(string RegId, Int16 CompanyId, M_CustomeGroupCreditLimt M_CustomeGroupCreditLimt, Int32 UserId);

        public Task<SqlResponce> DeleteCustomeGroupCreditLimtAsync(string RegId, Int16 CompanyId, M_CustomeGroupCreditLimt M_CustomeGroupCreditLimt, Int32 UserId);
    }
}