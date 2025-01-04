﻿using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Setting;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Setting;
using AHHA.Core.Models.Setting;
using AHHA.Infra.Data;
using System.Transactions;

namespace AHHA.Infra.Services.Setting
{
    public sealed class DecimalSettingServices : IDecimalSettingService
    {
        private readonly IRepository<S_DecSettings> _repository;
        private ApplicationDbContext _context;

        public DecimalSettingServices(IRepository<S_DecSettings> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<DecimalSettingViewModel> GetDecSettingAsync(string RegId, Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<DecimalSettingViewModel>(RegId, $"SELECT AmtDec,LocAmtDec,CtyAmtDec,PriceDec,QtyDec,ExhRateDec,DateFormat,LongDateFormat FROM dbo.S_DecSettings WHERE CompanyId={CompanyId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DecSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_DecSettings",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveDecSettingAsync(string RegId, Int16 CompanyId, S_DecSettings s_DecSettings, Int16 UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var DataExist = await _repository.GetQueryAsync<SqlResponseIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.S_DecSettings WHERE CompanyId = {s_DecSettings.CompanyId}");

                    if (DataExist.Count() > 0 && DataExist.ToList()[0].IsExist == 1)
                    {
                        var entity = _context.Update(s_DecSettings);
                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;
                    }
                    else
                    {
                        s_DecSettings.EditById = null;
                        s_DecSettings.EditDate = null;
                        _context.Add(s_DecSettings);
                    }

                    var FinSettingsToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (FinSettingsToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Setting,
                            TransactionId = (short)E_Setting.DecSetting,
                            DocumentId = 0,
                            DocumentNo = "",
                            TblName = "S_DecSettings",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "FinSettings Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = _context.SaveChanges();

                        if (auditLogSave > 0)
                        {
                            TScope.Complete();
                            return new SqlResponse { Result = 1, Message = "Save Successfully" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = 1, Message = "Save Failed" };
                    }

                    #endregion Save AuditLog

                    return new SqlResponse();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Setting,
                        TransactionId = (short)E_Setting.DecSetting,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "S_DecSettings",
                        ModeId = (short)E_Mode.Create,
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