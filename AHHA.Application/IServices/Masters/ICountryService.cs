using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models;

namespace AHHA.Application.IServices.Masters
{
    public interface ICountryService
    {
        public Task<CountryViewModelCount> GetCountryListAsync(short CompanyId, short pageSize, short pageNumber, int UserId);
        public Task<M_Country> GetCountryByIdAsync(short CompanyId, int CountryId, int UserId);
        public Task DeleteCountryAsync(short CompanyId, int CountryId, int UserId);
        public Task<SqlResponce> AddCountryAsync(short CompanyId, M_Country M_Country, int UserId);
        public Task<SqlResponce> UpdateCountryAsync(short CompanyId, M_Country M_Country, int UserId);

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
