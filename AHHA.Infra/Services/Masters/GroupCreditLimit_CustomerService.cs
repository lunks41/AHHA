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
    public sealed class GroupCreditLimit_CustomerService : IGroupCreditLimit_CustomerService
    {
        private readonly IRepository<M_GroupCreditLimt_Customer> _repository;
        private ApplicationDbContext _context;

        public GroupCreditLimit_CustomerService(IRepository<M_GroupCreditLimt_Customer> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<GroupCreditLimt_CustomerViewModelCount> GetGroupCreditLimit_CustomerListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            GroupCreditLimt_CustomerViewModelCount GroupCreditLimit_CustomerViewModelCount = new GroupCreditLimt_CustomerViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_GroupCreditLimt_Customer WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.GroupCreditLimt_Customer},{(short)Modules.Master}))");

                var result = await _repository.GetQueryAsync<GroupCreditLimt_CustomerViewModel>($"SELECT M_Cou.GroupCreditLimitId,M_Cou.,M_Cou.GroupCreditLimit_CustomerName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_GroupCreditLimt_Customer M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.GroupCreditLimit_CustomerName LIKE '%{searchString}%' OR M_Cou. LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.GroupCreditLimitId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Master.GroupCreditLimt_Customer},{(short)Modules.Master})) ORDER BY M_Cou.GroupCreditLimit_CustomerName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                GroupCreditLimit_CustomerViewModelCount.Total_records = totalcount == null ? 0 : totalcount.CountId;
                GroupCreditLimit_CustomerViewModelCount.groupCreditLimtCustomerViewModels = result == null ? null : result.ToList();

                return GroupCreditLimit_CustomerViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.GroupCreditLimt_Customer,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GroupCreditLimt_Customer",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }

        }
        public async Task<M_GroupCreditLimt_Customer> GetGroupCreditLimit_CustomerByIdAsync(Int16 CompanyId, Int32 GroupCreditLimitId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_GroupCreditLimt_Customer>($"SELECT GroupCreditLimitId,,GroupCreditLimit_CustomerName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_GroupCreditLimt_Customer WHERE GroupCreditLimitId={GroupCreditLimitId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.GroupCreditLimt_Customer,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GroupCreditLimt_Customer",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
        public async Task<SqlResponce> AddGroupCreditLimit_CustomerAsync(Int16 CompanyId, M_GroupCreditLimt_Customer GroupCreditLimt_Customer, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_GroupCreditLimt_Customer WHERE CompanyId IN (SELECT DISTINCT GroupCreditLimitId FROM dbo.Fn_Adm_GetShareCompany ({GroupCreditLimt_Customer.CompanyId},{(short)Master.GroupCreditLimt_Customer},{(short)Modules.Master})) UNION ALL SELECT 2 AS IsExist FROM dbo.M_GroupCreditLimt_Customer WHERE CompanyId IN (SELECT DISTINCT GroupCreditLimitId FROM dbo.Fn_Adm_GetShareCompany ({GroupCreditLimt_Customer.CompanyId},{(short)Master.GroupCreditLimt_Customer},{(short)Modules.Master}))'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -1, Message = "GroupCreditLimt_Customer Code Exist" };
                        }
                        else if (StrExist.ToList()[1].IsExist == 2)
                        {
                            isExist = true;
                            return new SqlResponce { Id = -2, Message = "GroupCreditLimt_Customer Name Exist" };
                        }
                    }
                    else
                    {
                        isExist = false;
                    }

                    if (!isExist)
                    {
                        //Take the Missing Id From SQL
                        var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>("SELECT ISNULL((SELECT TOP 1 (GroupCreditLimitId + 1) FROM dbo.M_GroupCreditLimt_Customer WHERE (GroupCreditLimitId + 1) NOT IN (SELECT GroupCreditLimitId FROM dbo.M_GroupCreditLimt_Customer)),1) AS MissId");

                        #region Saving GroupCreditLimt_Customer

                        GroupCreditLimt_Customer.GroupCreditLimitId = Convert.ToInt32(sqlMissingResponce.MissId);

                        var entity = _context.Add(GroupCreditLimt_Customer);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var GroupCreditLimit_CustomerToSave = _context.SaveChanges();

                        #endregion

                        #region Save AuditLog
                        if (GroupCreditLimit_CustomerToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.GroupCreditLimt_Customer,
                                TransactionId = (short)Modules.Master,
                                DocumentId = GroupCreditLimt_Customer.GroupCreditLimitId,
                                DocumentNo = "",
                                TblName = "M_GroupCreditLimt_Customer",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "GroupCreditLimitId Should not be zero" };
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
                        ModuleId = (short)Master.GroupCreditLimt_Customer,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_GroupCreditLimt_Customer",
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
        public async Task<SqlResponce> UpdateGroupCreditLimit_CustomerAsync(Int16 CompanyId, M_GroupCreditLimt_Customer GroupCreditLimt_Customer, Int32 UserId)
        {
            bool isExist = false;
            var sqlResponce = new SqlResponce();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (GroupCreditLimt_Customer.GroupCreditLimitId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 2 AS IsExist FROM dbo.M_GroupCreditLimt_Customer WHERE CompanyId IN (SELECT DISTINCT GroupCreditLimitId FROM dbo.Fn_Adm_GetShareCompany ({GroupCreditLimt_Customer.CompanyId},{(short)Master.GroupCreditLimt_Customer},{(short)Modules.Master}))  AND GroupCreditLimitId <>{GroupCreditLimt_Customer.GroupCreditLimitId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                isExist = true;
                                return new SqlResponce { Id = -2, Message = "GroupCreditLimt_Customer Name Exist" };
                            }
                        }
                        else
                        {
                            isExist = false;
                        }

                        if (!isExist)
                        {
                            #region Update GroupCreditLimt_Customer

                            var entity = _context.Update(GroupCreditLimt_Customer);

                            entity.Property(b => b.CreateById).IsModified = false;
                            entity.Property(b => b.CompanyId).IsModified = false;

                            var counToUpdate = _context.SaveChanges();

                            #endregion

                            if (counToUpdate > 0)
                            {
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)Master.GroupCreditLimt_Customer,
                                    TransactionId = (short)Modules.Master,
                                    DocumentId = GroupCreditLimt_Customer.GroupCreditLimitId,
                                    DocumentNo = "",
                                    TblName = "M_GroupCreditLimt_Customer",
                                    ModeId = (short)Mode.Update,
                                    Remarks = "GroupCreditLimt_Customer Update Successfully",
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
                        sqlResponce = new SqlResponce { Id = -1, Message = "GroupCreditLimitId Should not be zero" };
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
                        ModuleId = (short)Master.GroupCreditLimt_Customer,
                        TransactionId = (short)Modules.Master,
                        DocumentId = GroupCreditLimt_Customer.GroupCreditLimitId,
                        DocumentNo = "",
                        TblName = "M_GroupCreditLimt_Customer",
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
        public async Task<SqlResponce> DeleteGroupCreditLimit_CustomerAsync(Int16 CompanyId, M_GroupCreditLimt_Customer GroupCreditLimt_Customer, Int32 UserId)
        {
            var sqlResponce = new SqlResponce();
            try
            {
                if (GroupCreditLimt_Customer.GroupCreditLimitId > 0)
                {
                    var GroupCreditLimit_CustomerToRemove = _context.M_GroupCreditLimt_Customer.Where(x => x.GroupCreditLimitId == GroupCreditLimt_Customer.GroupCreditLimitId).ExecuteDelete();

                    if (GroupCreditLimit_CustomerToRemove > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)Master.GroupCreditLimt_Customer,
                            TransactionId = (short)Modules.Master,
                            DocumentId = GroupCreditLimt_Customer.GroupCreditLimitId,
                            DocumentNo = "",
                            TblName = "M_GroupCreditLimt_Customer",
                            ModeId = (short)Mode.Delete,
                            Remarks = "GroupCreditLimt_Customer Delete Successfully",
                            CreateById = UserId
                        };
                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();
                    }

                    sqlResponce = new SqlResponce { Id = 1, Message = "Delete Successfully" };
                }
                else
                {
                    sqlResponce = new SqlResponce { Id = -1, Message = "GroupCreditLimitId Should be zero" };
                }
                return sqlResponce;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.GroupCreditLimt_Customer,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GroupCreditLimt_Customer",
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
