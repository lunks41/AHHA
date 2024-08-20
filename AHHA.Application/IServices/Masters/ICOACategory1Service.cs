using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICOACategory1Service
    {
        public Task<COACategoryViewModelCount> GetCOACategory1ListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);

        public Task<M_COACategory1> GetCOACategory1ByIdAsync(string RegId, Int16 CompanyId, Int16 COACategoryId, Int32 UserId);

        public Task<SqlResponce> AddCOACategory1Async(string RegId, Int16 CompanyId, M_COACategory1 M_COACategory1, Int32 UserId);

        public Task<SqlResponce> UpdateCOACategory1Async(string RegId, Int16 CompanyId, M_COACategory1 M_COACategory1, Int32 UserId);

        public Task<SqlResponce> DeleteCOACategory1Async(string RegId, Int16 CompanyId, M_COACategory1 M_COACategory1, Int32 UserId);
    }
}