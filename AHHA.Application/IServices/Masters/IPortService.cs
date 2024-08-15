using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IPortService
    {
        public Task<PortViewModelCount> GetPortListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_Port> GetPortByIdAsync(Int16 CompanyId, Int32 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddPortAsync(Int16 CompanyId, M_Port M_Port, Int32 UserId);
        public Task<SqlResponce> UpdatePortAsync(Int16 CompanyId, M_Port M_Port, Int32 UserId);
        public Task<SqlResponce> DeletePortAsync(Int16 CompanyId, M_Port M_Port, Int32 UserId);
    }
}
