using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IGroupCreditLimitService
    {
        public Task<GroupCreditLimtViewModelCount> GetGroupCreditLimitListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);

        public Task<M_GroupCreditLimt> GetGroupCreditLimitByIdAsync(string RegId, Int16 CompanyId, Int16 COACategoryId, Int32 UserId);

        public Task<SqlResponce> AddGroupCreditLimitAsync(string RegId, Int16 CompanyId, M_GroupCreditLimt M_GroupCreditLimit, Int32 UserId);

        public Task<SqlResponce> UpdateGroupCreditLimitAsync(string RegId, Int16 CompanyId, M_GroupCreditLimt M_GroupCreditLimit, Int32 UserId);

        public Task<SqlResponce> DeleteGroupCreditLimitAsync(string RegId, Int16 CompanyId, M_GroupCreditLimt M_GroupCreditLimit, Int32 UserId);
    }
}