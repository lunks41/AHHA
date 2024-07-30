

using AHHA.Application.CommonServices;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models;
using Dapper;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.Json.Nodes;

namespace AHHA.Application.Services.Masters.Countries
{
    public sealed class CountryService : ICountryService
    {
        private readonly IQueryRepository<M_Country> _queryRepository;
        private readonly IRepository<M_Country> _repository;
        private readonly IAuditLogServices _auditLogServices;
        private readonly IErrorLogServices _errorLogServices;
        
        public CountryService(IQueryRepository<M_Country> queryRepository, IRepository<M_Country> repository, IAuditLogServices auditLogServices, IErrorLogServices errorLogServices)
        {
            _queryRepository = queryRepository;
            _repository = repository;
            _auditLogServices = auditLogServices;
            _errorLogServices = errorLogServices;
        }

        public async Task<CountryViewModelCount> GetCountryListAsync(byte CompanyId, byte pageSize, byte pageNumber)
        {
            var parameters = new DynamicParameters();
            CountryViewModelCount countryViewModelCount = new CountryViewModelCount();
            try
            {
                var totalcount = await _queryRepository.GetAllFromSqlQueryAsyncV1<SqlMissingResponce,dynamic>("SELECT COUNT(*) AS MissId FROM M_Country", parameters);

                var result = await _queryRepository.GetAllFromSqlQueryAsync<CountryViewModel, dynamic>($"SELECT M_Cou.CountryId,M_Cou.CountryCode,M_Cou.CountryName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Country M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{((byte)Master.Country)},{((byte)Modules.Master)})) ORDER BY M_Cou.CountryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY", parameters);

                countryViewModelCount.Total_records = totalcount.MissId;
                countryViewModelCount.countryViewModels = result.ToList();

                return countryViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = ((byte)Master.Country),
                    TransactionId = ((byte)Modules.Master),
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Country",
                    ModeId = 0,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = 0,
                    //CreateDate = DateTime.Now

                };

                await _errorLogServices.AddErrorLogAsync(errorLog);
                throw new Exception(ex.ToString());
            }

        }
        public async Task<M_Country> GetCountryByIdAsync(int CountryId, byte CompanyId)
        {
            var parameters = new DynamicParameters();
            try
            {
                var result = await _queryRepository.QueryDetailDtoAsyncV1<M_Country>($"SELECT CountryId,CountryCode,CountryName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Country WHERE CountryId={CountryId} AND CompanyId={CompanyId}", parameters);

                return result;

                //var parameters = new DynamicParameters();
                //parameters.Add("Id", 1, DbType.Int32);

                //IEnumerable<M_Country> countries = await _queryRepository.GetByIdAsync<M_Country, dynamic>("USP_Country", parameters);
                //return countries.FirstOrDefault();
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = ((byte)Master.Country),
                    TransactionId = ((byte)Modules.Master),
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Country",
                    ModeId = 0,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = 0,
                    //CreateDate = DateTime.Now

                };

                await _errorLogServices.AddErrorLogAsync(errorLog);

                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> AddCountryAsyncV1(M_Country country,byte CompanyId)
        {
            var parameters = new DynamicParameters();
            bool isExist = false;

            //using (var transaction = _context.Database.BeginTransaction())
            //{
                try
                {
                    ////Check Code & Name
                    //var StrExist = await _queryRepository.ExecuteDataSetQuery($"SELECT 1 AS IsExist FROM dbo.M_Country WHERE CountryId IN (SELECT DISTINCT CountryId FROM dbo.Fn_Adm_GetShareCompany ({country.CompanyId},{((byte)Master.Country)},{((byte)Modules.Master)})) AND CountryCode='{country.CountryCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Country WHERE CountryId IN (SELECT DISTINCT CountryId FROM dbo.Fn_Adm_GetShareCompany ({country.CompanyId},{((byte)Master.Country)},{((byte)Modules.Master)})) AND CountryName='{country.CountryName}'");

                    //if (StrExist.Tables.Count > 0 && StrExist != null)
                    //{
                    //    for (int i = 0; i < StrExist.Tables.Count; i++)
                    //    {
                    //        DataTable dataTable = StrExist.Tables[i];
                    //        if (dataTable.Rows.Count > 0)
                    //        {
                    //            DataRow row = dataTable.Rows[0];
                    //            var value = row["IsExist"];
                    //            isExist = Convert.ToBoolean(value);
                    //            if (isExist)
                    //            {
                    //                return new SqlResponce { Id = -1, Msg = "Code Or Name are same into database" };
                    //            }
                    //        }
                    //    }
                    //}

                    var StrExist = await _queryRepository.GetAllFromSqlQueryAsync<SqlMissingResponce, dynamic>($"SELECT 1 AS IsExist FROM dbo.M_Country WHERE CountryId IN (SELECT DISTINCT CountryId FROM dbo.Fn_Adm_GetShareCompany ({country.CompanyId},{((byte)Master.Country)},{((byte)Modules.Master)})) AND CountryCode='{country.CountryCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Country WHERE CountryId IN (SELECT DISTINCT CountryId FROM dbo.Fn_Adm_GetShareCompany ({country.CompanyId},{((byte)Master.Country)},{((byte)Modules.Master)})) AND CountryName='{country.CountryName}'", parameters);

                    if (StrExist.ToList()[0].IsExist == 1)
                    {
                        isExist = true;
                        return new SqlResponce { Id = -1, Msg = "Country Code Exist" };
                    }
                    else if (StrExist.ToList()[1].IsExist == 2)
                    {
                        isExist = true;
                        return new SqlResponce { Id = -2, Msg = "Country Name Exist" };
                    }


                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _queryRepository.QueryDetailDtoAsyncV1<SqlMissingResponce>("SELECT ISNULL((SELECT TOP 1 (CountryId + 1) FROM dbo.M_Country WHERE (CountryId + 1) NOT IN (SELECT CountryId FROM dbo.M_Country)),1) AS MissId", parameters);

                        #region Saving Country
                        //Country Saving
                        country.CountryId = Convert.ToInt16(sqlMissingResponce.MissId);
                        country.CreateDate = DateTime.Now;

                        var countryA = await _repository.CreateAsync(country);

                        #endregion

                        #region Save AuditLog
                        if (countryA.CountryId > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = country.CompanyId,
                                ModuleId = ((byte)Master.Country),
                                TransactionId = ((byte)Modules.Master),
                                DocumentId = Convert.ToByte(sqlMissingResponce.MissId),
                                DocumentNo = country.CountryCode,
                                TblName = "M_Country",
                                ModeId = ((byte)Mode.Create),
                                Remarks = "Invoice Save Successfully",
                                CreateById = Convert.ToInt16(country.CreateById),
                                CreateDate = DateTime.Now

                            };

                            await _auditLogServices.AddAuditLogAsync(auditLog);
                        }

                        #endregion

                    }

                    return new SqlResponce { Id = 1, Msg = "Save Successfully" };
                }
                catch (Exception ex)
                {
                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = country.CompanyId,
                        ModuleId = ((byte)Master.Country),
                        TransactionId = ((byte)Modules.Master),
                        DocumentId = 0,
                        DocumentNo = country.CountryCode,
                        TblName = "M_Country",
                        ModeId = ((byte)Mode.Create),
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = Convert.ToInt16(country.CreateById)
                        //CreateDate = DateTime.Now
                    };

                    await _errorLogServices.AddErrorLogAsync(errorLog);

                    throw new Exception(ex.ToString());
                }
            //}
        }
        public async Task<SqlResponce> AddCountryAsync(M_Country country, byte CompanyId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CountryId", country.CountryId, DbType.Int32);
                parameters.Add("CountryCode", country.CountryCode, DbType.String);
                parameters.Add("CountryName", country.CountryName, DbType.String);

                return await _queryRepository.UpsertAsync("USP_Country", parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> UpdateCountryAsyncV1(M_Country country, byte CompanyId)
        {
            var parameters = new DynamicParameters();
            try
            {
                var countryA = await _repository.UpdateAsync(country);

                if (countryA.CountryId > 0)
                {
                    var auditLog = new AdmAuditLog
                    {
                        CompanyId = country.CompanyId,
                        ModuleId = ((byte)Master.Country),
                        TransactionId = ((byte)Modules.Master),
                        DocumentId = country.CountryId,
                        DocumentNo = country.CountryCode,
                        TblName = "M_Country",
                        ModeId = ((byte)Mode.Update),
                        Remarks = "Invoice Save Successfully",
                        CreateById = Convert.ToInt16(country.EditById),
                        //CreateDate = DateTime.Now

                    };

                    await _auditLogServices.AddAuditLogAsync(auditLog);
                }

                return new SqlResponce { Id = 1, Msg = "Save Successfully" };

            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = country.CompanyId,
                    ModuleId = ((byte)Master.Country),
                    TransactionId = ((byte)Modules.Master),
                    DocumentId = country.CountryId,
                    DocumentNo = country.CountryCode,
                    TblName = "M_Country",
                    ModeId = ((byte)Mode.Update),
                    Remarks = ex.Message,
                    CreateById = Convert.ToInt16(country.EditById)
                    //CreateDate = DateTime.Now

                };

                await _errorLogServices.AddErrorLogAsync(errorLog);

                throw new Exception(ex.ToString());

            }
        }
        public async Task<SqlResponce> UpdateCountryAsync(M_Country country, byte CompanyId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CountryId", country.CountryId, DbType.Int32);
                parameters.Add("CountryCode", country.CountryCode, DbType.String);
                parameters.Add("CountryName", country.CountryName, DbType.String);

                return await _queryRepository.UpsertAsync("USP_Country", parameters);
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = country.CompanyId,
                    ModuleId = ((byte)Master.Country),
                    TransactionId = ((byte)Modules.Master),
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Country",
                    ModeId = 0,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = 1,
                    //CreateDate = DateTime.Now

                };

                await _errorLogServices.AddErrorLogAsync(errorLog);
                throw new Exception(ex.ToString());
            }
        }
        public async Task DeleteCountryAsync(int CountryId, byte CompanyId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Id", CountryId, DbType.Int32);

                await _queryRepository.UpsertAsync("USP_Country", parameters);
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = ((byte)Master.Country),
                    TransactionId = ((byte)Modules.Master),
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Country",
                    ModeId = 0,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = 1,
                    //CreateDate = DateTime.Now

                };

                await _errorLogServices.AddErrorLogAsync(errorLog);

                throw new Exception(ex.ToString());
            }
        }
        public async Task<DataSet> GetTrainingByIdsAsync(int Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Type", "GET_BY_TRAINING_ID", DbType.String);
                parameters.Add("Id", Id, DbType.Int32);
                return await _queryRepository.ExecuteDataSetStoredProcedure("USP_LMS_Training", parameters);
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine($"Exception: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }



        //private readonly IDapperRepository<LS_Agent> _dapperRepository;
        //public AgentServices(IDapperRepository<LS_Agent> dapperRepository)
        //{
        //    _dapperRepository = dapperRepository;
        //}

        #region METHODS
        //public async Task<LS_Agent> GetByIdAsync(int Id)
        //{
        //    var parameters = new CustomDynamicParameters();
        //    parameters.Add("Type", "GET_BY_ID", DbType.String);
        //    parameters.Add("Id", Id, DbType.Int32);
        //    return await _dapperRepository.QuerySingleOrDefaultAsync("USP_Agent", parameters);
        //}

        //public async Task<IEnumerable<LS_Agent>> GetAllAsync(string SpName)
        //{
        //    var parameters = new DynamicParameters();
        //    parameters.Add("Type", SpName, DbType.String);
        //    return await _dapperRepository.QueryAsync("USP_Agent", parameters);
        //}
        //public async Task<DapperPagedList<AgentDTO>> GetAllByPagingAsync(PagingModel pagingModel)
        //{
        //    var parameters = new DynamicParameters();
        //    parameters.Add("Type", "GET_ALL_BY_PAGING", DbType.String);
        //    parameters.Add("PageNum", pagingModel.Page, DbType.Int32);
        //    parameters.Add("PageSize", pagingModel.PageSize, DbType.Int32);
        //    parameters.Add("@SearchText", pagingModel.SearchText);
        //    var (CustomerCategories, totalCount) = await _dapperRepository.QueryMultipleDtoPagingAsync<AgentDTO>("USP_Agent", parameters);
        //    var pagedList = new DapperPagedList<AgentDTO>
        //    {
        //        Items = CustomerCategories,
        //        TotalCount = totalCount,
        //        PageNumber = (int)pagingModel.Page,
        //        PageSize = (int)pagingModel.PageSize
        //    };
        //    return pagedList;
        //}
        //public async Task<SqlResponseModel> UpsertAsync(LS_Agent model)
        //{
        //    var parameters = new DynamicParameters();
        //    parameters.Add("Type", "INSERT_UPDATE", DbType.String);
        //    parameters.Add("Id", model.Id, DbType.Int32);
        //    parameters.Add("CustomerId", model.CustomerId, DbType.Int32);
        //    parameters.Add("Agent", model.Agent, DbType.String);
        //    parameters.Add("AgentCode", model.AgentCode, DbType.String);
        //    parameters.Add("ContactName", model.ContactName, DbType.String);
        //    parameters.Add("ContactPhone", model.ContactPhone, DbType.String);
        //    parameters.Add("ContactFax", model.ContactFax, DbType.String);
        //    parameters.Add("AgentEmail", model.AgentEmail, DbType.String);
        //    parameters.Add("Street1", model.Street1, DbType.String);
        //    parameters.Add("Street2", model.Street2, DbType.String);
        //    parameters.Add("City", model.City, DbType.String);
        //    parameters.Add("StateId", model.StateId, DbType.Int32);
        //    parameters.Add("ZipCode", model.ZipCode, DbType.String);
        //    parameters.Add("IsActive", model.IsActive, DbType.Boolean);
        //    parameters.Add("CreatedBy", model.CreatedBy, DbType.Int32);

        //    return await _dapperRepository.QuerySqlResponseModelAsync("USP_Agent", parameters);
        //}

        //public async Task<bool> UpdateStatusAsync(int Id, int CreatedBy)
        //{
        //    var parameters = new DynamicParameters();
        //    parameters.Add("Type", "UPDATE_STATUS", DbType.String);
        //    parameters.Add("Id", Id, DbType.Int32);
        //    parameters.Add("CreatedBy", CreatedBy, DbType.Int32);
        //    return await _dapperRepository.ExecuteScalarAsync("USP_Agent", parameters);
        //}

        //public async Task<bool> DeleteAsync(int Id, int CreatedBy)
        //{
        //    var parameters = new DynamicParameters();
        //    parameters.Add("Type", "DELETE", DbType.String);
        //    parameters.Add("Id", Id, DbType.Int32);
        //    parameters.Add("CreatedBy", CreatedBy, DbType.Int32);
        //    return await _dapperRepository.ExecuteScalarAsync("USP_Agent", parameters);
        //}
        //public async Task<IEnumerable<DropDownModel>> GetDropDownAsync(string TypeName)
        //{
        //    var parameters = new DynamicParameters();
        //    parameters.Add("@Type", TypeName, DbType.String);
        //    return await _dapperRepository.QueryDtoAsync<DropDownModel>("USP_Agent", parameters);
        //}

        #endregion
    }
}
