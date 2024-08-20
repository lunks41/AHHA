using AHHA.Core.Models.Admin;

namespace AHHA.Application.IServices.Masters
{
    public interface IErrorLogService
    {
        public Task<IEnumerable<ErrorLogViewModel>> GetErrorLogListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
    }
}