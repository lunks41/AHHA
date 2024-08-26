﻿using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AHHA.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AHHA.Infra.Services.Masters
{
    public sealed class GroupCreditLimit_CustomerService : IGroupCreditLimit_CustomerService
    {
        private readonly IRepository<M_GroupCreditLimit_Customer> _repository;
        private ApplicationDbContext _context;

        public GroupCreditLimit_CustomerService(IRepository<M_GroupCreditLimit_Customer> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<GroupCreditLimit_CustomerViewModelCount> GetGroupCreditLimit_CustomerListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            GroupCreditLimit_CustomerViewModelCount groupCreditLimit_CustomerViewModelCount = new GroupCreditLimit_CustomerViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_GroupCreditLimit_Customer M_Grc INNER JOIN M_GroupCreditLimit M_Grp ON M_Grp.GroupCreditLimitId = M_Grc.GroupCreditLimitId INNER JOIN M_Customer M_Cus ON M_Cus.CustomerId = M_Grc.CustomerId WHERE (M_Grp.GroupCreditLimitName LIKE '%{searchString}%' OR M_Grp.GroupCreditLimitCode LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%' OR M_Cus.CustomerName LIKE '%{searchString}%') AND M_Grc.GroupCreditLimitId<>0 AND M_Grc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.GroupCreditLimit_Customer}))");

                var result = await _repository.GetQueryAsync<GroupCreditLimit_CustomerViewModel>(RegId, $"SELECT M_Grc.GroupCreditLimitId,M_Cus.CustomerCode,M_Cus.CustomerName,M_Grc.CompanyId,M_Grp.Remarks,M_Grp.GroupCreditLimitName,M_Grp.GroupCreditLimitCode,M_Grc.CreateById,M_Grc.CreateDate,M_Grc.EditById,M_Grc.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_GroupCreditLimit_Customer M_Grc INNER JOIN M_GroupCreditLimit M_Grp ON M_Grp.GroupCreditLimitId = M_Grc.GroupCreditLimitId INNER JOIN M_Customer M_Cus ON M_Cus.CustomerId = M_Grc.CustomerId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Grc.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Grc.EditById WHERE (M_Grp.GroupCreditLimitName LIKE '%{searchString}%' OR M_Grp.GroupCreditLimitCode LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%' OR M_Cus.CustomerName LIKE '%{searchString}%') AND M_Grc.GroupCreditLimitId<>0 AND M_Grc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.GroupCreditLimit_Customer})) ORDER BY M_Grp.GroupCreditLimitName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                groupCreditLimit_CustomerViewModelCount.responseCode = 200;
                groupCreditLimit_CustomerViewModelCount.responseMessage = "success";
                groupCreditLimit_CustomerViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                groupCreditLimit_CustomerViewModelCount.data = result == null ? null : result.ToList();

                return groupCreditLimit_CustomerViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.GroupCreditLimit,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GroupCreditLimit_Customer",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_GroupCreditLimit_Customer> GetGroupCreditLimit_CustomerByIdAsync(string RegId, Int16 CompanyId, Int32 GroupCreditLimitId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_GroupCreditLimit_Customer>(RegId, $"SELECT GroupCreditLimitId,GroupCreditLimit_CustomerName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_GroupCreditLimit_Customer WHERE GroupCreditLimitId={GroupCreditLimitId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Modules.Master,
                    TransactionId = (short)Master.GroupCreditLimit,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GroupCreditLimit_Customer",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddGroupCreditLimit_CustomerAsync(string RegId, Int16 CompanyId, M_GroupCreditLimit_Customer GroupCreditLimit_Customer, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_GroupCreditLimit_Customer WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({GroupCreditLimit_Customer.CompanyId},{(short)Modules.Master},{(short)Master.GroupCreditLimit_Customer})) UNION ALL SELECT 2 AS IsExist FROM dbo.M_GroupCreditLimit_Customer WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({GroupCreditLimit_Customer.CompanyId},{(short)Modules.Master},{(short)Master.GroupCreditLimit_Customer}))'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "GroupCreditLimit_Customer Code Exist" };
                        }
                        else if (StrExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "GroupCreditLimit_Customer Name Exist" };
                        }
                    }

                    //Take the Missing Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (GroupCreditLimitId + 1) FROM dbo.M_GroupCreditLimit_Customer WHERE (GroupCreditLimitId + 1) NOT IN (SELECT GroupCreditLimitId FROM dbo.M_GroupCreditLimit_Customer)),1) AS MissId");
                    if (sqlMissingResponce != null && sqlMissingResponce.MissId > 0)
                    {
                        #region Saving GroupCreditLimit_Customer

                        GroupCreditLimit_Customer.GroupCreditLimitId = Convert.ToInt32(sqlMissingResponce.MissId);

                        var entity = _context.Add(GroupCreditLimit_Customer);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var GroupCreditLimit_CustomerToSave = _context.SaveChanges();

                        #endregion Saving GroupCreditLimit_Customer

                        #region Save AuditLog

                        if (GroupCreditLimit_CustomerToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.GroupCreditLimit,
                                DocumentId = GroupCreditLimit_Customer.GroupCreditLimitId,
                                DocumentNo = "",
                                TblName = "M_GroupCreditLimit_Customer",
                                ModeId = (short)Mode.Create,
                                Remarks = "Group Credit Limit Save Successfully",
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
                        return new SqlResponce { Result = -1, Message = "GroupCreditLimitId Should not be zero" };
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
                        TransactionId = (short)Master.GroupCreditLimit,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_GroupCreditLimit_Customer",
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

        public async Task<SqlResponce> UpdateGroupCreditLimit_CustomerAsync(string RegId, Int16 CompanyId, M_GroupCreditLimit_Customer GroupCreditLimit_Customer, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (GroupCreditLimit_Customer.GroupCreditLimitId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_GroupCreditLimit_Customer WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({GroupCreditLimit_Customer.CompanyId},{(short)Modules.Master},{(short)Master.GroupCreditLimit_Customer}))  AND GroupCreditLimitId <>{GroupCreditLimit_Customer.GroupCreditLimitId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "GroupCreditLimit_Customer Name Exist" };
                            }
                        }

                        #region Update GroupCreditLimit_Customer

                        var entity = _context.Update(GroupCreditLimit_Customer);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update GroupCreditLimit_Customer

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.GroupCreditLimit,
                                DocumentId = GroupCreditLimit_Customer.GroupCreditLimitId,
                                DocumentNo = "",
                                TblName = "M_GroupCreditLimit_Customer",
                                ModeId = (short)Mode.Update,
                                Remarks = "GroupCreditLimit_Customer Update Successfully",
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
                        return new SqlResponce { Result = -1, Message = "GroupCreditLimitId Should not be zero" };
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
                        TransactionId = (short)Master.GroupCreditLimit,
                        DocumentId = GroupCreditLimit_Customer.GroupCreditLimitId,
                        DocumentNo = "",
                        TblName = "M_GroupCreditLimit_Customer",
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

        public async Task<SqlResponce> DeleteGroupCreditLimit_CustomerAsync(string RegId, Int16 CompanyId, M_GroupCreditLimit_Customer GroupCreditLimit_Customer, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (GroupCreditLimit_Customer.GroupCreditLimitId > 0)
                    {
                        var GroupCreditLimit_CustomerToRemove = _context.M_GroupCreditLimit_Customer.Where(x => x.GroupCreditLimitId == GroupCreditLimit_Customer.GroupCreditLimitId).ExecuteDelete();

                        if (GroupCreditLimit_CustomerToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Modules.Master,
                                TransactionId = (short)Master.GroupCreditLimit,
                                DocumentId = GroupCreditLimit_Customer.GroupCreditLimitId,
                                DocumentNo = "",
                                TblName = "M_GroupCreditLimit_Customer",
                                ModeId = (short)Mode.Delete,
                                Remarks = "GroupCreditLimit_Customer Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "GroupCreditLimitId Should be zero" };
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
                        TransactionId = (short)Master.GroupCreditLimit,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_GroupCreditLimit_Customer",
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
    }
}