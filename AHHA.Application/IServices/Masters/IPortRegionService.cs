﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IPortRegionService
    {
        public Task<PortRegionViewModelCount> GetPortRegionListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<PortRegionViewModel> GetPortRegionByIdAsync(string RegId, Int16 CompanyId, Int16 PortRegionId, Int16 UserId);

        public Task<SqlResponse> SavePortRegionAsync(string RegId, Int16 CompanyId, M_PortRegion M_PortRegion, Int16 UserId);

        public Task<SqlResponse> DeletePortRegionAsync(string RegId, Int16 CompanyId, M_PortRegion M_PortRegion, Int16 UserId);
    }
}