﻿using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Setting;
using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AP;
using AHHA.Core.Entities.Accounts.AR;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Setting;
using AHHA.Core.Models.Setting;
using AHHA.Infra.Data;

namespace AHHA.Infra.Services.Setting
{
    public sealed class NumberFormatServices : INumberFormatServices
    {
        private readonly IRepository<S_NumberFormat> _repository;
        private ApplicationDbContext _context;

        public NumberFormatServices(IRepository<S_NumberFormat> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        // add the number id
        public async Task<ModelNameViewModelCount> GetNumberFormatListAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            ModelNameViewModelCount countViewModel = new ModelNameViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM AdmTransaction AdmTrn INNER JOIN AdmModule AdmMod on AdmMod.ModuleId=AdmTrn.ModuleId where AdmMod.IsActive=1 And AdmTrn.IsActive=1 And AdmTrn.IsNumber=1");

                var result = await _repository.GetQueryAsync<ModelNameViewModel>(RegId, $"SELECT AdmMod.ModuleId,AdmMod.ModuleName,AdmTrn.TransactionId,AdmTrn.TransactionName FROM AdmTransaction AdmTrn INNER JOIN AdmModule AdmMod on AdmMod.ModuleId=AdmTrn.ModuleId where AdmMod.IsActive=1 And AdmTrn.IsActive=1 And AdmTrn.IsNumber=1  ORDER BY AdmMod.SeqNo,AdmTrn.SeqNo");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result == null ? null : result.ToList();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DocumentNo,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_NumberFormat",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<NumberSettingViewModel> GetNumberFormatByIdAsync(string RegId, Int16 CompanyId, Int32 ModuleId, Int32 TransactionId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<NumberSettingViewModel>(RegId, $"SELECT S_No.NumberId,S_No.CompanyId,S_No.ModuleId,S_No.TransactionId,S_No.Prefix,S_No.PrefixSeq,S_No.PrefixDelimiter,S_No.IncludeYear,S_No.YearSeq,S_No.YearFormat,S_No.YearDelimiter,S_No.IncludeMonth,S_No.MonthFormat,S_No.MonthDelimiter,S_No.NoDIgits,S_No.DIgitSeq,S_No.ResetYearly,S_No.CreateById,S_No.CreateDate,S_No.EditById,S_No.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.S_NumberFormat S_No LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = S_No.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = S_No.EditById WHERE S_No.ModuleId={ModuleId} AND S_No.TransactionId={TransactionId} AND S_No.CompanyId={CompanyId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DocumentNo,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_NumberFormat",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<NumberSettingDtViewModel> GetNumberFormatByYearAsync(string RegId, Int16 CompanyId, Int32 NumberId, Int32 NumYear, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<NumberSettingDtViewModel>(RegId, $"SELECT NumberId,NumYear,Month1,Month2,Month3,Month4,Month5,Month6,Month7,Month8,Month9,Month10,Month11,Month12,LastNumber FROM dbo.S_NumberFormatDt WHERE NumberId={NumberId} AND NumYear={NumYear}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DocumentNo,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_NumberFormatDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveNumberFormatAsync(string RegId, Int16 CompanyId, S_NumberFormat s_NumberFormat, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                bool IsEdit = false;
                try
                {
                    if (s_NumberFormat.NumberId != 0)
                    {
                        IsEdit = true;
                    }

                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.S_NumberFormat WHERE ModuleId<>{s_NumberFormat.ModuleId} AND TransactionId<>{s_NumberFormat.TransactionId} AND CompanyId={CompanyId} AND Prefix = '{s_NumberFormat.Prefix}'");

                        if (DataExist.Count() > 0 && DataExist.ToList()[0].IsExist == 1)
                            return new SqlResponce { Result = -1, Message = "Invoice Not Exist" };
                    }

                    if (IsEdit)
                    {
                        var entityHead = _context.Update(s_NumberFormat);
                        entityHead.Property(b => b.CreateById).IsModified = false;
                        entityHead.Property(b => b.CompanyId).IsModified = false;
                    }
                    else
                    {
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (NumberId + 1) FROM dbo.S_NumberFormat WHERE (NumberId + 1) NOT IN (SELECT NumberId FROM dbo.S_NumberFormat)),1) AS NextId");

                        if (sqlMissingResponce != null && sqlMissingResponce.NextId > 0)
                        {
                            s_NumberFormat.NumberId = Convert.ToInt32(sqlMissingResponce.NextId);

                            s_NumberFormat.EditById = null;
                            s_NumberFormat.EditDate = null;
                            var entity = _context.Add(s_NumberFormat);
                        }
                    }

                    var NumberFormatToSave = _context.SaveChanges();

                    if (NumberFormatToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Setting,
                            TransactionId = (short)E_Setting.DocumentNo,
                            DocumentId = s_NumberFormat.NumberId,
                            DocumentNo = "",
                            TblName = "S_NumberFormat",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "NumberFormat Save Successfully",
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

                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Setting,
                        TransactionId = (short)E_Setting.DocumentNo,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "S_NumberFormat",
                        ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }
    }
}