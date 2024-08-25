using AHHA.Application.CommonServices;
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
    public sealed class SupplierContactService : ISupplierContactService
    {
        private readonly IRepository<M_SupplierContact> _repository;
        private ApplicationDbContext _context;

        public SupplierContactService(IRepository<M_SupplierContact> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<SupplierContactViewModelCount> GetSupplierContactListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int32 UserId)
        {
            SupplierContactViewModelCount supplierContactViewModelCount = new SupplierContactViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM M_SupplierContact WHERE CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.SupplierContact}))");

                var result = await _repository.GetQueryAsync<SupplierContactViewModel>(RegId, $"SELECT M_Cou.ContactId,M_Cou.,M_Cou.ContactName,M_Cou.CompanyId,M_Cou.Remarks,M_Cou.IsActive,M_Cou.CreateById,M_Cou.CreateDate,M_Cou.EditById,M_Cou.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_SupplierContact M_Cou LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cou.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cou.EditById WHERE (M_Cou.ContactName LIKE '%{searchString}%' OR M_Cou. LIKE '%{searchString}%' OR M_Cou.Remarks LIKE '%{searchString}%') AND M_Cou.ContactId<>0 AND M_Cou.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)Modules.Master},{(short)Master.SupplierContact})) ORDER BY M_Cou.ContactName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                supplierContactViewModelCount.responseCode = 200;
                supplierContactViewModelCount.responseMessage = "success";
                supplierContactViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                supplierContactViewModelCount.data = result == null ? null : result.ToList();

                return supplierContactViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.SupplierContact,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_SupplierContact",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_SupplierContact> GetSupplierContactByIdAsync(string RegId, Int16 CompanyId, Int32 ContactId, Int32 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_SupplierContact>(RegId, $"SELECT ContactId,,ContactName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_SupplierContact WHERE ContactId={ContactId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)Master.SupplierContact,
                    TransactionId = (short)Modules.Master,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_SupplierContact",
                    ModeId = (short)Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddSupplierContactAsync(string RegId, Int16 CompanyId, M_SupplierContact SupplierContact, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.M_SupplierContact WHERE CompanyId IN (SELECT DISTINCT ContactId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.SupplierContact})) UNION ALL SELECT 2 AS IsExist FROM dbo.M_SupplierContact WHERE CompanyId IN (SELECT DISTINCT ContactId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.SupplierContact})) AND ContactName='{SupplierContact.ContactName}'");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "SupplierContact Code Exist" };
                        }
                        else if (StrExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "SupplierContact Name Exist" };
                        }
                    }

                    //Take the Missing Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (ContactId + 1) FROM dbo.M_SupplierContact WHERE (ContactId + 1) NOT IN (SELECT ContactId FROM dbo.M_SupplierContact)),1) AS MissId");
                    if (sqlMissingResponce != null && sqlMissingResponce.MissId > 0)
                    {
                        #region Saving SupplierContact

                        SupplierContact.ContactId = Convert.ToInt32(sqlMissingResponce.MissId);

                        var entity = _context.Add(SupplierContact);
                        entity.Property(b => b.EditDate).IsModified = false;

                        var SupplierContactToSave = _context.SaveChanges();

                        #endregion Saving SupplierContact

                        #region Save AuditLog

                        if (SupplierContactToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.SupplierContact,
                                TransactionId = (short)Modules.Master,
                                DocumentId = SupplierContact.ContactId,
                                DocumentNo = SupplierContact.ContactName,
                                TblName = "M_SupplierContact",
                                ModeId = (short)Mode.Create,
                                Remarks = "Supplier Contact Save Successfully",
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
                        return new SqlResponce { Result = -1, Message = "ContactId Should not be zero" };
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
                        ModuleId = (short)Master.SupplierContact,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = SupplierContact.ContactName,
                        TblName = "M_SupplierContact",
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

        public async Task<SqlResponce> UpdateSupplierContactAsync(string RegId, Int16 CompanyId, M_SupplierContact SupplierContact, Int32 UserId)
        {
            int IsActive = SupplierContact.IsActive == true ? 1 : 0;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (SupplierContact.ContactId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.M_SupplierContact WHERE CompanyId IN (SELECT DISTINCT ContactId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)Modules.Master},{(short)Master.SupplierContact})) AND ContactName='{SupplierContact.ContactName} AND ContactId <>{SupplierContact.ContactId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "SupplierContact Name Exist" };
                            }
                        }

                        #region Update SupplierContact

                        var entity = _context.Update(SupplierContact);

                        entity.Property(b => b.CreateById).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update SupplierContact

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.SupplierContact,
                                TransactionId = (short)Modules.Master,
                                DocumentId = SupplierContact.ContactId,
                                DocumentNo = SupplierContact.ContactName,
                                TblName = "M_SupplierContact",
                                ModeId = (short)Mode.Update,
                                Remarks = "SupplierContact Update Successfully",
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
                        return new SqlResponce { Result = -1, Message = "ContactId Should not be zero" };
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
                        ModuleId = (short)Master.SupplierContact,
                        TransactionId = (short)Modules.Master,
                        DocumentId = SupplierContact.ContactId,
                        DocumentNo = SupplierContact.ContactName,
                        TblName = "M_SupplierContact",
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

        public async Task<SqlResponce> DeleteSupplierContactAsync(string RegId, Int16 CompanyId, M_SupplierContact SupplierContact, Int32 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (SupplierContact.ContactId > 0)
                    {
                        var SupplierContactToRemove = _context.M_SupplierContact.Where(x => x.ContactId == SupplierContact.ContactId).ExecuteDelete();

                        if (SupplierContactToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)Master.SupplierContact,
                                TransactionId = (short)Modules.Master,
                                DocumentId = SupplierContact.ContactId,
                                DocumentNo = SupplierContact.ContactName,
                                TblName = "M_SupplierContact",
                                ModeId = (short)Mode.Delete,
                                Remarks = "SupplierContact Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "ContactId Should be zero" };
                    }
                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)Master.SupplierContact,
                        TransactionId = (short)Modules.Master,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_SupplierContact",
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