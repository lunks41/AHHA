﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IDepartmentService
    {
        public Task<DepartmentViewModelCount> GetDepartmentListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId);

        public Task<M_Department> GetDepartmentByIdAsync(string RegId, Int16 CompanyId, Int32 COACategoryId, Int32 UserId);

        public Task<SqlResponce> AddDepartmentAsync(string RegId, Int16 CompanyId, M_Department M_Department, Int32 UserId);

        public Task<SqlResponce> UpdateDepartmentAsync(string RegId, Int16 CompanyId, M_Department M_Department, Int32 UserId);

        public Task<SqlResponce> DeleteDepartmentAsync(string RegId, Int16 CompanyId, M_Department M_Department, Int32 UserId);
    }
}