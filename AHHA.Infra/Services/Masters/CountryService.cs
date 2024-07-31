using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models;
using AHHA.Infra.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.Json.Nodes;
using System.Transactions;
using static Dapper.SqlMapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AHHA.Infra.Services.Masters
{
    public sealed class CountryService : ICountryService
    {
        private readonly IQueryRepository<M_Country> _queryRepository;
        private readonly IRepository<M_Country> _repository;
        private readonly IAuditLogServices _auditLogServices;
        private readonly IErrorLogServices _errorLogServices;
        private ApplicationDbContext _context;

        public CountryService(IQueryRepository<M_Country> queryRepository, IRepository<M_Country> repository, IAuditLogServices auditLogServices, IErrorLogServices errorLogServices, ApplicationDbContext context)
        {
            _queryRepository = queryRepository;
            _repository = repository;
            _auditLogServices = auditLogServices;
            _errorLogServices = errorLogServices;
            _context = context;
        }

        public async Task<CountryViewModelCount> GetCountryListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, Int32 UserId)
        {
            var parameters = new DynamicParameters();
            CountryViewModelCount countryViewModelCount = new CountryViewModelCount();
            try
            {
                var totalcount = await _queryRepository.GetAllFromSqlQueryAsyncV1<SqlResponceIds, dynamic>($"SELECT COUNT(*) AS CountId FROM M_Country WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Country},{(short)Modules.Master}))", parameters);

                var result = await _queryRepository.GetAllFromSqlQueryAsync<CountryViewModel, dynamic>($"SELECT M_Cou.CountryId,M_Cou.CountryCode,M_Cou.CountryName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Country M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Country},{(short)Modules.Master})) ORDER BY M_Cou.CountryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY", parameters);

                countryViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                countryViewModelCount.countryViewModels = result == null ? null : result.ToList();

                return countryViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Country,
                    TransactionId = (short)Modules.Master,
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
        public async Task<M_Country> GetCountryByIdAsync(Int16 CompanyId, Int32 CountryId, Int32 UserId)
        {
            var parameters = new DynamicParameters();
            try
            {
                var result = await _queryRepository.QueryDetailDtoAsyncV1<M_Country>($"SELECT CountryId,CountryCode,CountryName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Country WHERE CountryId={CountryId}", parameters);

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Country,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Country",
                    ModeId = 0,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                    //CreateDate = DateTime.Now
                };

                await _errorLogServices.AddErrorLogAsync(errorLog);

                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> AddCountryAsync(Int16 CompanyId, M_Country country, Int32 UserId)
        {
            var parameters = new DynamicParameters();
            bool isExist = false;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _queryRepository.GetAllFromSqlQueryAsync<SqlResponceIds, dynamic>($"SELECT 1 AS IsExist FROM dbo.M_Country WHERE CountryId IN (SELECT DISTINCT CountryId FROM dbo.Fn_Adm_GetShareCompany ({country.CompanyId},{(short)Master.Country},{(short)Modules.Master})) AND CountryCode='{country.CountryCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Country WHERE CountryId IN (SELECT DISTINCT CountryId FROM dbo.Fn_Adm_GetShareCompany ({country.CompanyId},{(short)Master.Country},{(short)Modules.Master})) AND CountryName='{country.CountryName}'", parameters);

                    if (StrExist.Count() > 0)
                    {
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
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _queryRepository.QueryDetailDtoAsyncV1<SqlResponceIds>("SELECT ISNULL((SELECT TOP 1 (CountryId + 1) FROM dbo.M_Country WHERE (CountryId + 1) NOT IN (SELECT CountryId FROM dbo.M_Country)),1) AS MissId", parameters);


                        #region Saving Country
                        
                        country.CountryId = Convert.ToInt32(sqlMissingResponce.MissId);

                        _context.Add(country);
                        var countrySave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (countrySave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.Country,
                                TransactionId = (short)Modules.Master,
                                DocumentId = country.CountryId,
                                DocumentNo = country.CountryCode,
                                TblName = "M_Country",
                                ModeId = (short)Mode.Create,
                                Remarks = "Invoice Save Successfully",
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            var auditLogSave = _context.SaveChanges();

                            //await _auditLogServices.AddAuditLogAsync(auditLog);
                            if (auditLogSave > 0)
                                transaction.Commit();
                        }
                        #endregion
                    }
                    return new SqlResponce { Id = 1, Msg = "Save Successfully" };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)Master.Country,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = country.CountryCode,
                        TblName = "M_Country",
                        ModeId = (short)Mode.Create,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId
                    };
                    await _errorLogServices.AddErrorLogAsync(errorLog);

                    throw new Exception(ex.ToString());
                }
            }
        }
        public async Task<SqlResponce> UpdateCountryAsync(Int16 CompanyId, M_Country country, Int32 UserId)
        {
            var parameters = new DynamicParameters();
            int IsActive = country.IsActive == true ? 1 : 0;

            using (var transaction1 = new TransactionScope()) ;
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    #region Update Country
                    
                    country.CreateDate = DateTime.Now;

                    var entity = _context.Update(country);

                    entity.Property(b => b.CreateDate).IsModified = false;
                    entity.Property(b => b.CreateById).IsModified = false;
                    entity.Property(b => b.CountryCode).IsModified = false;
                    entity.Property(b => b.CompanyId).IsModified = false;

                    var countryUpdate = _context.SaveChanges();

                    #endregion

                    if (countryUpdate > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Master.Country,
                            TransactionId = (short)Modules.Master,
                            DocumentId = country.CountryId,
                            DocumentNo = country.CountryCode,
                            TblName = "M_Country",
                            ModeId = (short)Mode.Update,
                            Remarks = "Country Update Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();

                        if (auditLogSave > 0)
                            transaction.Commit();
                    }

                    return new SqlResponce { Id = 1, Msg = "Update Successfully" };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)Master.Country,
                        TransactionId = (short)Modules.Master,
                        DocumentId = country.CountryId,
                        DocumentNo = country.CountryCode,
                        TblName = "M_Country",
                        ModeId = (short)Mode.Update,
                        Remarks = ex.Message,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    //await _errorLogServices.AddErrorLogAsync(errorLog);

                    throw new Exception(ex.ToString());
                }
            }
        }
        public async Task<SqlResponce> DeleteCountryAsync(Int16 CompanyId, Int32 CountryId, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    #region Delete Country
                    //Get the country deteils
                    var countryToRemove = _context.M_Country.SingleOrDefault(x => x.CountryId == CountryId); //returns a single item.

                    if (countryToRemove != null)
                    {
                        //
                        _context.M_Country.Remove(countryToRemove);
                        var countryRemove = _context.SaveChanges();

                        #endregion

                        if (countryRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.Country,
                                TransactionId = (short)Modules.Master,
                                DocumentId = CountryId,
                                DocumentNo = countryToRemove.CountryCode,
                                TblName = "M_Country",
                                ModeId = (short)Mode.Delete,
                                Remarks = "Country Delete Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();

                            if (auditLogSave > 0)
                                transaction.Commit();
                        }
                    }

                    return new SqlResponce { Id = 1, Msg = "Delete Successfully" };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)Master.Country,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_Country",
                        ModeId = (short)Mode.Delete,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId,
                    };

                    await _errorLogServices.AddErrorLogAsync(errorLog);

                    throw new Exception(ex.ToString());
                }
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

        //public async Task<SqlResponce> UpdateCountryAsync(M_Country country, Int16 CompanyId)
        //{
        //    using (TransactionScope TScope = new TransactionScope())
        //    {
        //        try
        //        {
        //            var parameters = new DynamicParameters();
        //            parameters.Add("CountryId", country.CountryId, DbType.Int32);
        //            parameters.Add("CountryCode", country.CountryCode, DbType.String);
        //            parameters.Add("CountryName", country.CountryName, DbType.String);

        //            return await _queryRepository.UpsertAsync("USP_Country", parameters);
        //        }
        //        catch (Exception ex)
        //        {
        //            var errorLog = new AdmErrorLog
        //            {
        //                CompanyId = country.CompanyId,
        //                ModuleId = ((Int16)Master.Country),
        //                TransactionId = ((Int16)Modules.Master),
        //                DocumentId = 0,
        //                DocumentNo = "",
        //                TblName = "M_Country",
        //                ModeId = 0,
        //                Remarks = ex.Message + ex.InnerException,
        //                CreateById = 1,
        //                //CreateDate = DateTime.Now

        //            };

        //            await _errorLogServices.AddErrorLogAsync(errorLog);
        //            throw new Exception(ex.ToString());
        //        }
        //    }
        //}

        //public async Task<SqlResponce> AddCountryAsync(M_Country country, Int16 CompanyId)
        //{
        //    using (TransactionScope TScope = new TransactionScope())
        //    {
        //        try
        //        {
        //            var parameters = new DynamicParameters();
        //            parameters.Add("CountryId", country.CountryId, DbType.Int32);
        //            parameters.Add("CountryCode", country.CountryCode, DbType.String);
        //            parameters.Add("CountryName", country.CountryName, DbType.String);

        //            return await _queryRepository.UpsertAsync("USP_Country", parameters);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception(ex.ToString());
        //        }
        //    }
        //}


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

        ////Check Code & Name
        //var StrExist = await _queryRepository.ExecuteDataSetQuery($"SELECT TOP 1 1 AS IsExist FROM dbo.M_Country WHERE CountryId IN (SELECT DISTINCT CountryId FROM dbo.Fn_Adm_GetShareCompany ({country.CompanyId},{((Int16)Master.Country)},{((Int16)Modules.Master)})) AND CountryCode='{country.CountryCode}'; SELECT TOP 1 1 AS IsExist FROM dbo.M_Country WHERE CountryId IN (SELECT DISTINCT CountryId FROM dbo.Fn_Adm_GetShareCompany ({country.CompanyId},{((Int16)Master.Country)},{((Int16)Modules.Master)})) AND CountryName='{country.CountryName}'");

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
    }
}
