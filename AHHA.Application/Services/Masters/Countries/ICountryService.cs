using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models;

namespace AHHA.Application.Services.Masters.Countries
{
    public interface ICountryService
    {
        public Task<CountryViewModelCount> GetCountryListAsync(byte CompanyId, byte pageSize, byte pageNumber);
        public Task<M_Country> GetCountryByIdAsync(int Id, byte CompanyId);
        public Task<SqlResponce> AddCountryAsync(M_Country M_Country, byte CompanyId);
        public Task<SqlResponce> UpdateCountryAsync(M_Country M_Country, byte CompanyId);
        public Task DeleteCountryAsync(int Id, byte CompanyId);

        //Multi Repository
        public Task<SqlResponce> AddCountryAsyncV1(M_Country M_Country, byte CompanyId);
        public Task<SqlResponce> UpdateCountryAsyncV1(M_Country M_Country, byte CompanyId);


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
