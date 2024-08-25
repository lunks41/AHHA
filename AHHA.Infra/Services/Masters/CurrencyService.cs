﻿using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AHHA.Infra.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection;

namespace AHHA.Infra.Services.Masters
{
    public sealed class CurrencyService : ICurrencyService
    {
        private readonly IRepository<M_Currency> _repository;
        private ApplicationDbContext _context;

        public CurrencyService(IRepository<M_Currency> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<CurrencyViewModelCount> GetCurrencyListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            CurrencyViewModelCount CurrencyViewModelCount = new CurrencyViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_Currency WHERE (M_Cou.CurrencyName LIKE '%{searchString}%' OR M_Cou.CurrencyCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.CurrencyId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.Currency}))");

                var result = await _repository.GetQueryAsync<CurrencyViewModel>(RegId, $"SELECT M_Cou.CurrencyId,M_Cou.CurrencyCode,M_Cou.CurrencyName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Currency M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.CurrencyName LIKE '%{searchString}%' OR M_Cou.CurrencyCode LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.CurrencyId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.Currency})) ORDER BY M_Cou.CurrencyName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                CurrencyViewModelCount.responseCode = 200;
                CurrencyViewModelCount.responseMessage = "success";
                CurrencyViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                CurrencyViewModelCount.data = result.ToList();

                return CurrencyViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Currency,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Currency",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_Currency> GetCurrencyByIdAsync(string RegId, Int16 CompanyId, Int32 CurrencyId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Currency>(RegId, $"SELECT CurrencyId,CurrencyCode,CurrencyName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Currency WHERE CurrencyId={CurrencyId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.Currency,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Currency",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddCurrencyAsync(string RegId, Int16 CompanyId, M_Currency Currency, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_Currency WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({Currency.CompanyId},{(short)Modules.Master},{(short)Master.Currency})) AND CurrencyCode='{Currency.CurrencyCode}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Currency WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({Currency.CompanyId},{(short)Modules.Master},{(short)Master.Currency})) AND CurrencyName='{Currency.CurrencyName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "Currency Code Exist" };
                        }
                        else if (StrExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "Currency Name Exist" };
                        }
                    }

                    //Take the Missing Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (CurrencyId + 1) FROM dbo.M_Currency WHERE (CurrencyId + 1) NOT IN (SELECT CurrencyId FROM dbo.M_Currency)),1) AS MissId");

                    if (sqlMissingResponce != null && sqlMissingResponce.MissId > 0)
                    {
                        #region Saving Currency

                        Currency.CurrencyId = Convert.ToInt16(sqlMissingResponce.MissId);

                        var entity = _context.Add(Currency);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var CurrencyToSave = _context.SaveChanges();

                        #endregion Saving Currency

                        #region Save AuditLog

                        if (CurrencyToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Currency,
                                DocumentId = Currency.CurrencyId,
                                DocumentNo = Currency.CurrencyCode,
                                TblName = "M_Currency",
                                ModeId = (short)Mode.Create,
                                Remarks = "Currency Save Successfully",
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            var auditLogSave = _context.SaveChanges();

                            if (auditLogSave > 0)
                            {
                                transaction.Commit();
                                return new SqlResponce { Result = 1, Message = "Save Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = 1, Message = "Save Failed" };
                        }

                        #endregion Save AuditLog
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "CurrencyId Should not be zero" };
                    }
                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)Modules.Master,
                        TransactionId = (short)Master.Currency,
                        DocumentId = 0,
                        DocumentNo = Currency.CurrencyCode,
                        TblName = "M_Currency",
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

        public async Task<SqlResponce> UpdateCurrencyAsync(string RegId, Int16 CompanyId, M_Currency Currency, Int32 UserId)
        {
            int IsActive = Currency.IsActive == true ? 1 : 0;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Currency.CurrencyId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_Currency WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({Currency.CompanyId},{(short)Modules.Master},{(short)Master.Currency})) AND CurrencyName='{Currency.CurrencyName} AND CurrencyId <>{Currency.CurrencyId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "Currency Name Exist" };
                            }
                        }

                        #region Update Currency

                        var entity = _context.Update(Currency);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.CurrencyCode).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update Currency

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Currency,
                                DocumentId = Currency.CurrencyId,
                                DocumentNo = Currency.CurrencyCode,
                                TblName = "M_Currency",
                                ModeId = (short)Mode.Update,
                                Remarks = "Currency Update Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();

                            if (auditLogSave > 0)
                            {
                                transaction.Commit();
                                return new SqlResponce { Result = 1, Message = "Update Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Update Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "CurrencyId Should not be zero" };
                    }
                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)Modules.Master,
                        TransactionId = (short)Master.Currency,
                        DocumentId = Currency.CurrencyId,
                        DocumentNo = Currency.CurrencyCode,
                        TblName = "M_Currency",
                        ModeId = (short)Mode.Update,
                        Remarks = ex.Message,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        public async Task<SqlResponce> DeleteCurrencyAsync(string RegId, Int16 CompanyId, M_Currency Currency, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Currency.CurrencyId > 0)
                    {
                        var CurrencyToRemove = _context.M_Currency.Where(x => x.CurrencyId == Currency.CurrencyId).ExecuteDelete();

                        if (CurrencyToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.Currency,
                                DocumentId = Currency.CurrencyId,
                                DocumentNo = Currency.CurrencyCode,
                                TblName = "M_Currency",
                                ModeId = (short)Mode.Delete,
                                Remarks = "Currency Delete Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();
                            if (auditLogSave > 0)
                            {
                                transaction.Commit();
                                return new SqlResponce { Result = 1, Message = "Delete Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Delete Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "CurrencyId Should be zero" };
                    }
                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)Modules.Master,
                        TransactionId = (short)Master.Currency,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_Currency",
                        ModeId = (short)Mode.Delete,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId,
                    };

                    _context.Add(errorLog);
                    _context.SaveChanges();

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
                return await _repository.GetExecuteDataSetStoredProcedure("", "USP_LMS_Training", parameters);
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