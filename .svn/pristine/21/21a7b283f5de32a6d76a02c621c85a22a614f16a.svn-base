using AHHA.Core.Common;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Models.Admin;

namespace AHHA.Application.IServices.Admin
{
    public interface IDocumentService
    {
        public Task<DocumentViewModelCount> GetDocumentListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<DocumentViewModel> GetDocumentByIdAsync(string RegId, Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int64 DocumentId, Int32 ItemNo, Int16 UserId);

        public Task<SqlResponce> SaveDocumentAsync(string RegId, Int16 CompanyId, AdmDocuments admDocuments, Int16 UserId);

        public Task<SqlResponce> DeleteDocumentAsync(string RegId, Int16 CompanyId, DocumentViewModel documentViewModel, Int16 UserId);
    }
}