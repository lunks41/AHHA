﻿using AHHA.Application.CommonServices;
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
    public sealed class VoyageService : IVoyageService
    {
        private readonly IRepository<M_Voyage> _repository;
        private ApplicationDbContext _context;

        public VoyageService(IRepository<M_Voyage> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<VoyageViewModelCount> GetVoyageListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            VoyageViewModelCount VoyageViewModelCount = new VoyageViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_Voyage WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Voyage},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<VoyageViewModel>($"SELECT M_Cou.VoyageId,M_Cou.VoyageNo,M_Cou.VoyageName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Voyage M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.VoyageName LIKE '%{searchString}%' OR M_Cou.VoyageNo LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.VoyageId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.Voyage},{(short)Modules.Master})) ORDER BY M_Cou.VoyageName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                VoyageViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                VoyageViewModelCount.voyageViewModels = result == null ? null : result.ToList();

                return VoyageViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Voyage,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Voyage",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }

        }
        public async Task<M_Voyage> GetVoyageByIdAsync(Int16 CompanyId, Int32 VoyageId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Voyage>($"SELECT VoyageId,VoyageNo,VoyageName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Voyage WHERE VoyageId={VoyageId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Voyage,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Voyage",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> AddVoyageAsync(Int16 CompanyId, M_Voyage Voyage, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_Voyage WHERE CompanyId IN (SELECT DISTINCT VoyageId FROM dbo.Fn_Adm_GetShareCompany ({Voyage.CompanyId},{(short)Master.Voyage},{(short)Modules.Master})) AND VoyageNo='{Voyage.VoyageNo}' UNION ALL SELECT 2 AS IsExist FROM dbo.M_Voyage WHERE CompanyId IN (SELECT DISTINCT VoyageId FROM dbo.Fn_Adm_GetShareCompany ({Voyage.CompanyId},{(short)Master.Voyage},{(short)Modules.Master})) ");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "Voyage Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "Voyage Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>("SELECT ISNULL((SELECT TOP 1 (VoyageId + 1) FROM dbo.M_Voyage WHERE (VoyageId + 1) NOT IN (SELECT VoyageId FROM dbo.M_Voyage)),1) AS MissId");

                        #region Saving Voyage

                        Voyage.VoyageId = Convert.ToInt32(sqlMissingResponce.MissId);

                        var entity = _context.Add(Voyage);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var VoyageToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (VoyageToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.Voyage,
                                TransactionId = (short)Modules.Master,
                                DocumentId = Voyage.VoyageId,
                                DocumentNo = Voyage.VoyageNo,
                                TblName = "M_Voyage",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "VoyageId Should not be zero" };
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
                        ModuleId = (short)Master.Voyage,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = Voyage.VoyageNo,
                        TblName = "M_Voyage",
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
        public async Task<SqlResponce> UpdateVoyageAsync(Int16 CompanyId, M_Voyage Voyage, Int32 UserId)
        {
            int IsActive = Voyage.IsActive == true ? 1 : 0;
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (Voyage.VoyageId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 2 AS IsExist FROM dbo.M_Voyage WHERE CompanyId IN (SELECT DISTINCT VoyageId FROM dbo.Fn_Adm_GetShareCompany ({Voyage.CompanyId},{(short)Master.Voyage},{(short)Modules.Master})) AND VoyageNo='{Voyage.VoyageNo} AND VoyageId <>{Voyage.VoyageId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "Voyage Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update Voyage

                            var entity = _context.Update(Voyage);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.VoyageNo).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.Voyage,
                                    TransactionId = (short)Modules.Master,
                                    DocumentId = Voyage.VoyageId,
                                    DocumentNo = Voyage.VoyageNo,
                                    TblName = "M_Voyage",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "Voyage Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "VoyageId Should not be zero" };
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
                        ModuleId = (short)Master.Voyage,
                        TransactionId = (short)Modules.Master,
                        DocumentId = Voyage.VoyageId,
                        DocumentNo = Voyage.VoyageNo,
                        TblName = "M_Voyage",
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
        public async Task<SqlResponce> DeleteVoyageAsync(Int16 CompanyId, M_Voyage Voyage, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (Voyage.VoyageId > 0)
                {
                    var VoyageToRemove = _context.M_Voyage.Where(x => x.VoyageId == Voyage.VoyageId).ExecuteDelete();

                    if (VoyageToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Master.Voyage,
                            TransactionId = (short)Modules.Master,
                            DocumentId = Voyage.VoyageId,
                            DocumentNo = Voyage.VoyageNo,
                            TblName = "M_Voyage",
                            ModeId = (short)Mode.Delete,
                            Remarks = "Voyage Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "VoyageId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.Voyage,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Voyage",
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
                return await _repository.GetExecuteDataSetStoredProcedure("USP_LMS_Training", parameters);
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