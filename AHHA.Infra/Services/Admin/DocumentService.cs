using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Admin;
using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Admin;
using AHHA.Core.Models.Masters;
using AHHA.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using BC = BCrypt.Net.BCrypt;

namespace AHHA.Infra.Services.Admin
{
    public sealed class DocumentService : IDocumentService
    {
        private readonly IRepository<AdmDocuments> _repository;
        private ApplicationDbContext _context;

        public DocumentService(IRepository<AdmDocuments> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<DocumentViewModelCount> GetDocumentListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId)
        {
            DocumentViewModelCount countViewModel = new DocumentViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, $"SELECT COUNT(*) AS CountId FROM AdmDocuments AdmDoc WHERE AdmDoc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Admin},{(short)E_Admin.Document}))");

                var result = await _repository.GetQueryAsync<DocumentViewModel>(RegId, $"SELECT AdmDoc.CompanyId,AdmDoc.ModuleId,AdmDoc.TransactionId,AdmDoc.DocumentId,AdmDoc.DocumentNo,AdmDoc.ItemNo,AdmDoc.DocTypeId,AdmDoc.DocPath,AdmDoc.Remarks,AdmDoc.CreateById,AdmDoc.CreateDate,Usr.UserName AS CreateBy FROM dbo.AdmDocuments AdmDoc  LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = AdmDoc.CreateById WHERE AdmDoc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Admin},{(short)E_Admin.Document})) ORDER BY AdmDoc.DocTypeName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.Document,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmDocuments",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<DocumentViewModel> GetDocumentByIdAsync(string RegId, Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int64 DocumentId, Int32 ItemNo, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<DocumentViewModel>(RegId, $"SELECT AdmDoc.CompanyId,AdmDoc.ModuleId,AdmDoc.TransactionId,AdmDoc.DocumentId,AdmDoc.DocumentNo,AdmDoc.ItemNo,AdmDoc.DocTypeId,AdmDoc.DocPath,AdmDoc.Remarks,AdmDoc.CreateById,AdmDoc.CreateDate,Usr.UserName AS CreateBy FROM dbo.AdmDocuments AdmDoc  LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = AdmDoc.CreateById WHERE AdmDoc.ModuleId={ModuleId} AND AdmDoc.TransactionId={TransactionId} AND AdmDoc.DocumentId={DocumentId} AND AdmDoc.ItemNo={ItemNo} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Admin},{(short)E_Admin.Document}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.Document,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmDocuments",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveDocumentAsync(string RegId, Int16 CompanyId, AdmDocuments admDocuments, Int16 UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var DataExist = await _repository.GetQueryAsync<SqlResponceIds>(RegId, $"SELECT 1 AS IsExist FROM dbo.AdmDocuments WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Admin},{(short)E_Admin.Document})) AND DocTypeCode='{admDocuments.DocumentId}' UNION ALL SELECT 2 AS IsExist FROM dbo.AdmDocuments WHERE CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Admin},{(short)E_Admin.Document})) AND DocTypeName='{admDocuments.DocumentId}'");

                    if (DataExist.Count() > 0)
                    {
                        if (DataExist.ToList()[0].IsExist == 1)
                        {
                            return new SqlResponce { Result = -1, Message = "Document Code Exist" };
                        }
                        else if (DataExist.ToList()[0].IsExist == 2)
                        {
                            return new SqlResponce { Result = -2, Message = "Document Name Exist" };
                        }
                    }

                    //Take the Next Id From SQL
                    var sqlMissingResponce = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(RegId, "SELECT ISNULL((SELECT TOP 1 (DocTypeId + 1) FROM dbo.AdmDocuments WHERE (DocTypeId + 1) NOT IN (SELECT DocTypeId FROM dbo.AdmDocuments)),1) AS NextId");

                    if (sqlMissingResponce != null)
                    {
                        #region Saving Document

                        admDocuments.DocTypeId = Convert.ToInt16(sqlMissingResponce.NextId);

                        var entity = _context.Add(admDocuments);

                        var DocumentToSave = _context.SaveChanges();

                        #endregion Saving Document

                        #region Save AuditLog

                        if (DocumentToSave > 0)
                        {
                            //Saving Audit log
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Admin,
                                TransactionId = (short)E_Admin.Document,
                                DocumentId = Convert.ToInt64(admDocuments.DocumentId),
                                DocumentNo = admDocuments.DocumentNo,
                                TblName = "AdmDocuments",
                                ModeId = (short)E_Mode.Create,
                                Remarks = "Document Save Successfully",
                                CreateById = UserId,
                                CreateDate = DateTime.Now
                            };

                            _context.Add(auditLog);
                            var auditLogSave = _context.SaveChanges();

                            if (auditLogSave > 0)
                            {
                                TScope.Complete();
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
                        return new SqlResponce { Result = -1, Message = "DocTypeId Should not be zero" };
                    }
                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Admin,
                        TransactionId = (short)E_Admin.Document,
                        DocumentId = Convert.ToInt64(admDocuments.DocumentId),
                        DocumentNo = admDocuments.DocumentNo,
                        TblName = "AdmDocuments",
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

        public async Task<SqlResponce> DeleteDocumentAsync(string RegId, Int16 CompanyId, DocumentViewModel documentViewModel, Int16 UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (documentViewModel.DocTypeId > 0)
                    {
                        var DocumentToRemove = _context.AdmDocuments.Where(x => x.ModuleId == documentViewModel.ModuleId && x.TransactionId == documentViewModel.TransactionId && x.DocumentId == Convert.ToInt64(documentViewModel.DocumentId) && x.CompanyId == CompanyId).ExecuteDelete();

                        if (DocumentToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Admin,
                                TransactionId = (short)E_Admin.Document,
                                DocumentId = Convert.ToInt64(documentViewModel.DocumentId),
                                DocumentNo = documentViewModel.DocumentNo,
                                TblName = "AdmDocuments",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Document Delete Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();

                            if (auditLogSave > 0)
                            {
                                TScope.Complete();
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
                        return new SqlResponce { Result = -1, Message = "DocTypeId Should be zero" };
                    }
                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Admin,
                        TransactionId = (short)E_Admin.Document,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "AdmDocuments",
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