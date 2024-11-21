﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICountryService
    {
        public Task<CountryViewModelCount> GetCountryListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_Country> GetCountryByIdAsync(string RegId, Int16 CompanyId, Int16 CountryId, Int16 UserId);

        public Task<SqlResponce> SaveCountryAsync(string RegId, Int16 CompanyId, M_Country M_Country, Int16 UserId);

        public Task<SqlResponce> AddCountryAsync(string RegId, Int16 CompanyId, M_Country M_Country, Int16 UserId);

        public Task<SqlResponce> UpdateCountryAsync(string RegId, Int16 CompanyId, M_Country M_Country, Int16 UserId);

        public Task<SqlResponce> DeleteCountryAsync(string RegId, Int16 CompanyId, M_Country M_Country, Int16 UserId);

        //public Task<SqlResponce> AddCountryAsync(M_Country M_Country, Int16 CompanyId);
        //public Task<SqlResponce> UpdateCountryAsync(M_Country M_Country, Int16 CompanyId);
        //public interface IAgentServices
        //{
        //    Task<LS_Agent> GetByIdAsync(int Id);
        //    Task<IEnumerable<LS_Agent>> GetAllAsync(string SpName);
        //    Task<DapperPagedList<AgentDTO>> GetAllByPagingAsync(PagingModel pagingModel);
        //    Task<SqlResponseModel> UpsertAsync(LS_Agent model);
        //    Task<bool> UpdateStatusAsync(int Id, int UpdatedBy);
        //    Task<bool> DeleteAsync(int Id, int UpdatedBy);
        //    Task<IEnumerable<DropDownModel>> GetDropDownAsync(string TypeName);
        //}
    }
}