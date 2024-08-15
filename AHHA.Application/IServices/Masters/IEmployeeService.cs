using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IEmployeeService
    {
        public Task<EmployeeViewModelCount> GetEmployeeListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);
        public Task<M_Employee> GetEmployeeByIdAsync(Int16 CompanyId, Int32 COACategoryId, Int32 UserId);
        public Task<SqlResponce> AddEmployeeAsync(Int16 CompanyId, M_Employee M_Employee, Int32 UserId);
        public Task<SqlResponce> UpdateEmployeeAsync(Int16 CompanyId, M_Employee M_Employee, Int32 UserId);
        public Task<SqlResponce> DeleteEmployeeAsync(Int16 CompanyId, M_Employee M_Employee, Int32 UserId);
    }
}
