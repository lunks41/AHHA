﻿using AHHA.Application.CommonServices;
using AHHA.Application.IServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Helper;
using AHHA.Core.Models.Masters;
using AHHA.Infra.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AHHA.Infra.Services.Masters
{
    public sealed class BankContactService : IBankContactService
    {
        private readonly IRepository<M_BankContact> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public BankContactService(IRepository<M_BankContact> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<IEnumerable<BankContactViewModel>> GetBankContactByBankIdAsync(string RegId, Int16 CompanyId, Int32 BankId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<BankContactViewModel>(RegId, $"SELECT M_BanCon.BankId,M_BanCon.ContactId,M_BanCon.ContactName,M_BanCon.OtherName,M_BanCon.MobileNo,M_BanCon.OffNo,M_BanCon.FaxNo,M_BanCon.EmailAdd,M_BanCon.MessId,M_BanCon.ContactMessType,M_BanCon.IsDefault,M_BanCon.IsFinance,M_BanCon.IsSales,M_BanCon.IsActive,M_Ban.BankCode,M_Ban.BankName,M_BanCon.CreateById,M_BanCon.CreateDate,M_BanCon.EditById,M_BanCon.EditDate FROM dbo.M_BankContact M_BanCon INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = M_BanCon.BankId WHERE M_BanCon.BankId = {BankId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Bank,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_BankContact",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<BankContactViewModel> GetBankContactByIdAsync(string RegId, Int16 CompanyId, Int32 BankId, Int16 ContactId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<BankContactViewModel>(RegId, $"SELECT M_BanCon.BankId,M_BanCon.ContactId,M_BanCon.ContactName,M_BanCon.OtherName,M_BanCon.MobileNo,M_BanCon.OffNo,M_BanCon.FaxNo,M_BanCon.EmailAdd,M_BanCon.MessId,M_BanCon.ContactMessType,M_BanCon.IsDefault,M_BanCon.IsFinance,M_BanCon.IsSales,M_BanCon.IsActive,M_Ban.BankCode,M_Ban.BankName,M_BanCon.CreateById,M_BanCon.CreateDate,M_BanCon.EditById,M_BanCon.EditDate FROM dbo.M_BankContact M_BanCon INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = M_BanCon.BankId WHERE M_BanCon.BankId = {BankId} AND ContactId={ContactId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Bank,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_BankContact",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveBankContactAsync(string RegId, Int16 CompanyId, M_BankContact m_BankContact, Int16 UserId)
        {
            bool IsEdit = false;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (m_BankContact.BankId != 0 && m_BankContact.ContactId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponseIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_BankContact where BankId = {m_BankContact.BankId} And ContactName = '{m_BankContact.ContactName}' And ContactId<>{m_BankContact.ContactId}");

                        if (DataExist.Count() > 0 && (DataExist.ToList()[0].IsExist == 1 || DataExist.ToList()[0].IsExist == 2))
                            return new SqlResponse { Result = -1, Message = "Bank Contact Name Exist" };
                    }
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(m_BankContact);
                        entityHead.Property(b => b.CreateById).IsModified = false;
                        entityHead.Property(b => b.BankId).IsModified = false;
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(RegId, $"SELECT ISNULL((SELECT TOP 1(ContactId + 1) FROM dbo.M_BankContact WHERE BankId = {m_BankContact.BankId} AND (ContactId + 1) NOT IN(SELECT ContactId FROM dbo.M_BankContact where BankId= {m_BankContact.BankId})),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_BankContact.ContactId = Convert.ToInt16(sqlMissingResponse.NextId);

                            m_BankContact.EditDate = null;
                            m_BankContact.EditById = null;
                            _context.Add(m_BankContact);
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "Id Should not be zero" };
                    }

                    var BankToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (BankToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.Bank,
                            DocumentId = m_BankContact.ContactId,
                            DocumentNo = "",
                            TblName = "M_Bank",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Bank Save Successfully",
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
            }
            catch (SqlException sqlEx)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Bank,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_BankContact",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponse
                {
                    Result = -1,
                    Message = errorMessage
                };
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Bank,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_BankContact",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> DeleteBankContactAsync(string RegId, Int16 CompanyId, Int32 BankId, Int16 ContactId, Int16 UserId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (BankId > 0 && ContactId > 0)
                    {
                        var BankContactToRemove = _context.M_BankContact.Where(x => x.BankId == BankId && x.ContactId == ContactId).ExecuteDelete();

                        if (BankContactToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Bank,
                                DocumentId = BankId,
                                DocumentNo = "",
                                TblName = "M_BankContact",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "BankContact Delete Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();
                            if (auditLogSave > 0)
                            {
                                TScope.Complete();
                                return new SqlResponse { Result = 1, Message = "Delete Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Delete Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = -1, Message = "BankId Should be zero" };
                    }
                    return new SqlResponse();
                }
            }
            catch (SqlException sqlEx)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Bank,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_BankContact",

                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponse
                {
                    Result = -1,
                    Message = errorMessage
                };
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Bank,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_BankContact",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
    }
}