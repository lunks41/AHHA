﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IPortService
    {
        public Task<PortViewModelCount> GetPortListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<PortViewModel> GetPortByIdAsync(string RegId, Int16 CompanyId, Int32 PortId, Int16 UserId);

        public Task<SqlResponce> AddPortAsync(string RegId, Int16 CompanyId, M_Port m_Port, Int16 UserId);

        public Task<SqlResponce> UpdatePortAsync(string RegId, Int16 CompanyId, M_Port m_Port, Int16 UserId);

        public Task<SqlResponce> DeletePortAsync(string RegId, Int16 CompanyId, M_Port m_Port, Int16 UserId);
    }
}