using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Accounts.AR;
using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AR;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Account.AR;
using AHHA.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace AHHA.Infra.Services.Accounts.AR
{
    public sealed class ARInvoiceService : IARInvoiceService
    {
        private readonly IRepository<ArInvoiceHd> _repository;
        private ApplicationDbContext _context;

        public ARInvoiceService(IRepository<ArInvoiceHd> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<ARInvoiceViewModelCount> GetARInvoiceListAsync(string RegId, Int16 CompanyId, short pageSize, short pageNumber, string searchString, Int16 UserId)
        {
            ARInvoiceViewModelCount countViewModel = new ARInvoiceViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM dbo.ArInvoiceHd M_Ban INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Ban.CurrencyId INNER JOIN dbo.M_ChartOfAccount M_Chr ON M_Chr.GLId = M_Ban.GLId WHERE (M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Ban.ARInvoiceName LIKE '%{searchString}%' OR M_Ban.InvoiceNo LIKE '%{searchString}%' OR M_Ban.AccountNo LIKE '%{searchString}%' OR M_Ban.SwiftCode LIKE '%{searchString}%' OR M_Ban.Remarks1 LIKE '%{searchString}%' OR M_Ban.Remarks2 LIKE '%{searchString}%' OR M_Chr.GLName LIKE '%{searchString}%' OR M_Chr.GLCode LIKE '%{searchString}%') AND M_Ban.InvoiceId<>0 AND M_Ban.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.AR},{(short)E_AR.Invoice}))");

                var result = await _repository.GetQueryAsync<ARInvoiceViewModel>(RegId, $"SELECT M_Ban.InvoiceId,M_Ban.InvoiceNo,M_Ban.ARInvoiceName,M_Cur.CurrencyId,M_Cur.CurrencyName,M_Cur.CurrencyCode,M_Ban.AccountNo,M_Ban.SwiftCode,M_Ban.Remarks1,M_Ban.Remarks2,M_Ban.GLId,M_Chr.GLCode,M_Chr.GLName,M_Ban.IsActive,M_Ban.CreateById,M_Ban.CreateDate,M_Ban.EditById,M_Ban.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy  FROM dbo.ArInvoiceHd M_Ban INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Ban.CurrencyId INNER JOIN dbo.M_ChartOfAccount M_Chr ON M_Chr.GLId = M_Ban.GLId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Ban.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Ban.EditById WHERE (M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Ban.ARInvoiceName LIKE '%{searchString}%' OR M_Ban.InvoiceNo LIKE '%{searchString}%' OR M_Ban.AccountNo LIKE '%{searchString}%' OR M_Ban.SwiftCode LIKE '%{searchString}%' OR M_Ban.Remarks1 LIKE '%{searchString}%' OR M_Ban.Remarks2 LIKE '%{searchString}%' OR M_Chr.GLName LIKE '%{searchString}%' OR M_Chr.GLCode LIKE '%{searchString}%') AND M_Ban.InvoiceId<>0 AND M_Ban.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.AR},{(short)E_AR.Invoice})) ORDER BY M_Ban.ARInvoiceName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result == null ? null : result.ToList();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AR,
                    TransactionId = (short)E_AR.Invoice,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "ArInvoiceHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<ARInvoiceViewModel> GetARInvoiceByIdAsync(string RegId, Int16 CompanyId, Int64 InvoiceId, string InvoiceNo, Int16 UserId)
        {
            ARInvoiceViewModel aRInvoiceViewModel = new ARInvoiceViewModel();
            try
            {
                aRInvoiceViewModel = await _repository.GetQuerySingleOrDefaultAsync<ARInvoiceViewModel>(RegId, $"SELECT InvoiceId,CompanyId,InvoiceNo,ARInvoiceName,CurrencyId,AccountNo,SwiftCode,Remarks1,Remarks2,GLId,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.ArInvoiceHd WHERE (InvoiceId={InvoiceId} OR {InvoiceId}=0) AND (InvoiceNo='{InvoiceNo}' OR '{InvoiceNo}'='') AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.AR},{(short)E_AR.Invoice}))");

                aRInvoiceViewModel.data_details = await _repository.GetQueryAsync<ARInvoiceDtViewModel>(RegId, $"SELECT InvoiceId,CompanyId,InvoiceNo,ARInvoiceName,CurrencyId,AccountNo,SwiftCode,Remarks1,Remarks2,GLId,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.ArInvoiceHd WHERE InvoiceId={InvoiceId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.AR},{(short)E_AR.Invoice}))");

                return aRInvoiceViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AR,
                    TransactionId = (short)E_AR.Invoice,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "ArInvoiceHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> AddARInvoiceAsync(string RegId, Int16 CompanyId, ArInvoiceHd arInvoiceHd, List<ArInvoiceDt> arInvoiceDt, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.ArInvoiceHd WHERE InvoiceNo='{arInvoiceHd.InvoiceNo}' AND  CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.AR},{(short)E_AR.Invoice}))");

                    if (StrExist.Count() > 0)
                    {
                        if (StrExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "Invoice Number Exist" };
                        }
                    }

                    //Take the Missing Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (InvoiceId + 1) FROM dbo.ArInvoiceHd WHERE (InvoiceId + 1) NOT IN (SELECT InvoiceId FROM dbo.ArInvoiceHd)),1) AS MissId");

                    //var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (InvoiceId + 1) FROM dbo.ArInvoiceHd WHERE (InvoiceId + 1) NOT IN (SELECT InvoiceId FROM dbo.ArInvoiceHd)),1) AS MissId");

                    if (sqlMissingResponce != null && sqlMissingResponce.MissId > 0)
                    {
                        arInvoiceHd.InvoiceId = Convert.ToInt16(sqlMissingResponce.DocumentId);
                        arInvoiceHd.InvoiceNo = sqlMissingResponce.DocumentNo;

                        #region Saving ARInvoiceHD

                        var entityHead = _context.Add(arInvoiceHd);
                        entityHead.Property(b => b.EditDate).IsModified = false;

                        var ARInvoiceToSave = _context.SaveChanges();

                        #endregion Saving ARInvoiceHD

                        #region Saving ARInvoiceDT

                        if (ARInvoiceToSave > 0)
                        {
                            foreach (var item in arInvoiceDt)
                            {
                                _context.Add(item);
                                _context.SaveChanges();
                            }

                            #endregion Saving ARInvoiceDT

                            #region Save AuditLog

                            if (ARInvoiceToSave > 0)
                            {
                                //Saving Audit log
                                var auditLog = new AdmAuditLog
                                {
                                    CompanyId = CompanyId,
                                    ModuleId = (short)E_Modules.AR,
                                    TransactionId = (short)E_AR.Invoice,
                                    DocumentId = arInvoiceHd.InvoiceId,
                                    DocumentNo = arInvoiceHd.InvoiceNo,
                                    TblName = "ArInvoiceHd",
                                    ModeId = (short)E_Mode.Create,
                                    Remarks = "ARInvoice Save Successfully",
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
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "InvoiceId Should not be zero" };
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
                        ModuleId = (short)E_Modules.AR,
                        TransactionId = (short)E_AR.Invoice,
                        DocumentId = 0,
                        DocumentNo = arInvoiceHd.InvoiceNo,
                        TblName = "ArInvoiceHd",
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

        public async Task<SqlResponce> UpdateARInvoiceAsync(string RegId, Int16 CompanyId, ArInvoiceHd ARInvoice, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ARInvoice.InvoiceId > 0)
                    {
                        var StrExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 2 AS IsExist FROM dbo.ArInvoiceHd WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.AR},{(short)E_AR.Invoice})) AND ARInvoiceName='{ARInvoice.InvoiceNo} AND InvoiceId <>{ARInvoice.InvoiceId}'");

                        if (StrExist.Count() > 0)
                        {
                            if (StrExist.ToList()[0].IsExist == 2)
                            {
                                return new SqlResponce { Result = -2, Message = "ARInvoice Name Exist" };
                            }
                        }

                        #region Update ARInvoice

                        var entity = _context.Update(ARInvoice);

                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.InvoiceNo).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update ARInvoice

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.AR,
                                TransactionId = (short)E_AR.Invoice,
                                DocumentId = ARInvoice.InvoiceId,
                                DocumentNo = ARInvoice.InvoiceNo,
                                TblName = "ArInvoiceHd",
                                ModeId = (short)E_Mode.Update,
                                Remarks = "ARInvoice Update Successfully",
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
                        return new SqlResponce { Result = -1, Message = "InvoiceId Should not be zero" };
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
                        ModuleId = (short)E_Modules.AR,
                        TransactionId = (short)E_AR.Invoice,
                        DocumentId = ARInvoice.InvoiceId,
                        DocumentNo = ARInvoice.InvoiceNo,
                        TblName = "ArInvoiceHd",
                        ModeId = (short)E_Mode.Update,
                        Remarks = ex.Message,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        public async Task<SqlResponce> DeleteARInvoiceAsync(string RegId, Int16 CompanyId, ARInvoiceViewModel aRInvoiceViewModel, Int16 UserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (aRInvoiceViewModel.InvoiceId > 0)
                    {
                        var ARInvoiceToRemove = _context.ArInvoiceHd.Where(x => x.InvoiceId == aRInvoiceViewModel.InvoiceId).ExecuteDelete();

                        if (ARInvoiceToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.AR,
                                TransactionId = (short)E_AR.Invoice,
                                DocumentId = aRInvoiceViewModel.InvoiceId,
                                DocumentNo = aRInvoiceViewModel.InvoiceNo,
                                TblName = "ArInvoiceHd",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "ARInvoice Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "InvoiceId Should be zero" };
                    }
                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.AR,
                        TransactionId = (short)E_AR.Invoice,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "ArInvoiceHd",
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
}