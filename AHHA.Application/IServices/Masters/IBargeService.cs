using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IBargeService
    {
        public Task<BargeViewModelCount> GetBargeListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_Barge> GetBargeByIdAsync(Int16 CompanyId, Int16 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddBargeAsync(Int16 CompanyId, M_Barge M_Barge, Int32 UserId);
        public Task<SqlResponce> UpdateBargeAsync(Int16 CompanyId, M_Barge M_Barge, Int32 UserId);
        public Task<SqlResponce> DeleteBargeAsync(Int16 CompanyId, M_Barge M_Barge, Int32 UserId);
    }
}
