﻿using AHHA.Core.Common;
using AHHA.Core.Entities.Accounts.AP;
using AHHA.Core.Models.Account.AP;

namespace AHHA.Application.IServices.Accounts.AP
{
    public interface IAPDebitNoteService
    {
        public Task<APDebitNoteViewModelCount> GetAPDebitNoteListAsync(string RegId, Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<APDebitNoteViewModel> GetAPDebitNoteByIdAsync(string RegId, Int16 CompanyId, Int64 DebitNoteId, string DebitNoteNo, Int16 UserId);

        public Task<SqlResponse> SaveAPDebitNoteAsync(string RegId, Int16 CompanyId, ApDebitNoteHd APDebitNoteHd, List<ApDebitNoteDt> APDebitNoteDt, Int16 UserId);

        public Task<SqlResponse> DeleteAPDebitNoteAsync(string RegId, Int16 CompanyId, Int64 DebitNoteId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<APDebitNoteViewModel>> GetHistoryAPDebitNoteByIdAsync(string RegId, Int16 CompanyId, Int64 DebitNoteId, string DebitNoteNo, Int16 UserId);
    }
}