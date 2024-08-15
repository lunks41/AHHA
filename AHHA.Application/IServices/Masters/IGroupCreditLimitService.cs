using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IGroupCreditLimitService
    {
        public Task<GroupCreditLimtViewModelCount> GetGroupCreditLimitListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_GroupCreditLimt> GetGroupCreditLimitByIdAsync(Int16 CompanyId, Int16 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddGroupCreditLimitAsync(Int16 CompanyId, M_GroupCreditLimt M_GroupCreditLimit, Int32 UserId);
        public Task<SqlResponce> UpdateGroupCreditLimitAsync(Int16 CompanyId, M_GroupCreditLimt M_GroupCreditLimit, Int32 UserId);
        public Task<SqlResponce> DeleteGroupCreditLimitAsync(Int16 CompanyId, M_GroupCreditLimt M_GroupCreditLimit, Int32 UserId);
    }
}
