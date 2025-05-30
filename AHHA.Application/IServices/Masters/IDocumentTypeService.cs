﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;

namespace AHHA.Application.IServices.Masters
{
    public interface IDocumentTypeService
    {
        public Task<DocumentTypeViewModelCount> GetDocumentTypeListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<DocumentTypeViewModel> GetDocumentTypeByIdAsync(string RegId, Int16 CompanyId, Int16 DocTypeId, Int16 UserId);

        public Task<SqlResponse> SaveDocumentTypeAsync(string RegId, Int16 CompanyId, M_DocumentType m_DocumentType, Int16 UserId);

        public Task<SqlResponse> DeleteDocumentTypeAsync(string RegId, Int16 CompanyId, DocumentTypeViewModel documentTypeViewModel, Int16 UserId);
    }
}