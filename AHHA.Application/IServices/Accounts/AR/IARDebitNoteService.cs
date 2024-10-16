﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AR;
using AHHA.Core.Models.Account.AR;

namespace AHHA.Application.IServices.Accounts.AR
{
    public interface IARDebitNoteService
    {
        public Task<ARDebitNoteViewModelCount> GetARDebitNoteListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<ARDebitNoteViewModel> GetARDebitNoteByIdAsync(string RegId, Int16 CompanyId, Int64 InvoiceId, string InvoiceNo, Int16 UserId);

        public Task<SqlResponce> SaveARDebitNoteAsync(string RegId, Int16 CompanyId, ArDebitNoteHd ARDebitNoteHd, List<ArDebitNoteDt> ARDebitNoteDt, Int16 UserId);

        public Task<SqlResponce> DeleteARDebitNoteAsync(string RegId, Int16 CompanyId, Int64 InvoiceId, string CanacelRemarks, Int16 UserId);
    }
}