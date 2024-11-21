﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IDesignationService
    {
        public Task<DesignationViewModelCount> GetDesignationListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_Designation> GetDesignationByIdAsync(string RegId, Int16 CompanyId, Int16 DesignationId, Int16 UserId);

        public Task<SqlResponce> SaveDesignationAsync(string RegId, Int16 CompanyId, M_Designation M_Designation, Int16 UserId);

        public Task<SqlResponce> AddDesignationAsync(string RegId, Int16 CompanyId, M_Designation M_Designation, Int16 UserId);

        public Task<SqlResponce> UpdateDesignationAsync(string RegId, Int16 CompanyId, M_Designation M_Designation, Int16 UserId);

        public Task<SqlResponce> DeleteDesignationAsync(string RegId, Int16 CompanyId, M_Designation M_Designation, Int16 UserId);
    }
}