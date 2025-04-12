﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface ICountryService
    {
        public Task<CountryViewModelCount> GetCountryListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<M_Country> GetCountryByIdAsync(string RegId, Int16 CompanyId, Int16 CountryId, Int16 UserId);

        public Task<SqlResponse> SaveCountryAsync(string RegId, Int16 CompanyId, M_Country M_Country, Int16 UserId);

        public Task<SqlResponse> DeleteCountryAsync(string RegId, Int16 CompanyId, M_Country M_Country, Int16 UserId);
    }
}