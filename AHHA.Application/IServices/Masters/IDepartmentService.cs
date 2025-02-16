﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IDepartmentService
    {
        public Task<DepartmentViewModelCount> GetDepartmentListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_Department> GetDepartmentByIdAsync(string RegId, Int16 CompanyId, Int16 DepartmentId, Int16 UserId);

        public Task<SqlResponse> SaveDepartmentAsync(string RegId, Int16 CompanyId, M_Department M_Department, Int16 UserId);

        public Task<SqlResponse> DeleteDepartmentAsync(string RegId, Int16 CompanyId, M_Department M_Department, Int16 UserId);
    }
}