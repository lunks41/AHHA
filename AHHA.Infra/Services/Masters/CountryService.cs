using AHHA.Application.CommonServices;
using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AHHA.Infra.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AHHA.Infra.Services.Masters
{
    public sealed class CountryService : ICountryService
    {
        private readonly IRepository<M_Country> _repository;
        private ApplicationDbContext _context;

        public CountryService(IRepository<M_Country> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<CountryViewModelCount> GetCountryListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, Int32 UserId)
        {
            var parameters = new DynamicParameters();
            CountryViewModelCount countryViewModelCount = new CountryViewModelCount();
            try
            {
                var totalcount = await _repository.QuerySingleORDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_Country WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Country},{(short)Modules.Master}))", parameters);

                var result = await _repository.QueryIEnumerableAsync<CountryViewModel, dynamic>($"SELECT M_Cou.CountryId,M_Cou.CountryCode,M_Cou.CountryName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Country M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Country},{(short)Modules.Master})) ORDER BY M_Cou.CountryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY", parameters);

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

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }

        }
        public async Task<M_Country> GetCountryByIdAsync(Int16 CompanyId, Int32 CountryId, Int32 UserId)
        {
            var parameters = new DynamicParameters();
            try
            {
                var result = await _repository.QuerySingleORDefaultAsync<M_Country>($"SELECT CountryId,CountryCode,CountryName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Country WHERE CountryId={CountryId}", parameters);

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

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> AddCountryAsync(Int16 CompanyId, M_Country country, Int32 UserId)
        {
            var parameters = new DynamicParameters();
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.QueryIEnumerableAsync<SqlResponceIds, dynamic>($"SELECT 1 AS IsExist FROM dbo.M_Country WHERE CompanyId IN (SELECT DISTINCT CountryId FROM dbo.Fn_Adm_GetShareCompany ({country.CompanyId},{(short)Master.Country},{(short)Modules.Master})) AND CountryCode='{country.CountryCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Country WHERE CompanyId IN (SELECT DISTINCT CountryId FROM dbo.Fn_Adm_GetShareCompany ({country.CompanyId},{(short)Master.Country},{(short)Modules.Master})) AND CountryName='{country.CountryName}'", parameters);

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "Country Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "Country Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.QuerySingleORDefaultAsync<SqlResponceIds>("SELECT ISNULL((SELECT TOP 1 (CountryId + 1) FROM dbo.M_Country WHERE (CountryId + 1) NOT IN (SELECT CountryId FROM dbo.M_Country)),1) AS MissId", parameters);

                        #region Saving Country

                        country.CountryId = Convert.ToInt32(sqlMissingResponce.MissId);

                        _context.Add(country);
                        var countryToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (countryToSave > 0)
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
                            {
                                transaction.Commit();
                                sqlResponce = new SqlResponce { Id = 1, Message = "Save Successfully" };
                            }
                        }
                        #endregion

                    }
                    else
                    {
                        sqlResponce = new SqlResponce { Id = -1, Message = "CountryId Should not be zero" };
                    }
                    return sqlResponce;
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
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }
        public async Task<SqlResponce> UpdateCountryAsync(Int16 CompanyId, M_Country country, Int32 UserId)
        {
            var parameters = new DynamicParameters();
            int IsActive = country.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (country.CountryId > 0)
                    {
                        //var StrExist = _context.M_Country.FromSqlInterpolated($"SELECT 2 AS IsExist FROM dbo.M_Country WHERE CompanyId IN (SELECT DISTINCT CountryId FROM dbo.Fn_Adm_GetShareCompany ({country.CompanyId},{(short)Master.Country},{(short)Modules.Master})) AND CountryName='{country.CountryName}'").AsNoTracking().First();
                        ////Check the Name exist or not
                        var StrExist = await _repository.QueryIEnumerableAsync<SqlResponceIds, dynamic>($"SELECT 2 AS IsExist FROM dbo.M_Country WHERE CompanyId IN (SELECT DISTINCT CountryId FROM dbo.Fn_Adm_GetShareCompany ({country.CompanyId},{(short)Master.Country},{(short)Modules.Master})) AND CountryName='{country.CountryName} AND CountryId <>{country.CountryId}'", parameters);

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "Country Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update Country

                            var entity = _context.Update(country);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.CountryCode).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
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
                            sqlResponce = new SqlResponce { Id = 1, Message = "Update Successfully" };
                        }
                    }
                    else
                    {
                        sqlResponce = new SqlResponce { Id = -1, Message = "CountryId Should not be zero" };
                    }
                    return sqlResponce;
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
        public async Task<SqlResponce> DeleteCountryAsync(Int16 CompanyId, M_Country country, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (country.CountryId > 0)
                {
                    var countryToRemove = _context.M_Country.Where(x => x.CountryId == country.CountryId).ExecuteDelete();

                    if (countryToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Master.Country,
                            TransactionId = (short)Modules.Master,
                            DocumentId = country.CountryId,
                            DocumentNo = country.CountryCode,
                            TblName = "M_Country",
                            ModeId = (short)Mode.Delete,
                            Remarks = "Country Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "CountryId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
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

                _context.Add(errorLog);
                _context.SaveChanges();

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
                return await _repository.ExecuteDataSetStoredProcedure("USP_LMS_Training", parameters);
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine($"Exception: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }

    }
}
